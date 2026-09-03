using AutoMapper;
using Byohar.Application.Interfaces.Identity;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Responses.Identity;
using Byohar.Domain.Entities.Identity;
using Byohar.Infrastructure.Extensions;
using Byohar.Infrastructure.Helpers;
using Byohar.Infrastructure.Specifications;
using Byohar.Persistance.Contexts;
using Byohar.Shared.Wrapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using static Byohar.Shared.Constants.Common.ApplicationConstants;

namespace Byohar.Infrastructure.Services.Identity
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IStringLocalizer<RoleService> _localizer;
        private readonly IDistributedCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public RoleService(
            RoleManager<Role> roleManager,
            IMapper mapper,
            IStringLocalizer<RoleService> localizer,
            IDistributedCache cache,
            IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _localizer = localizer;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<Result<string>> DeleteAsync(Guid id)
        {
            var existingRole = await _roleManager.FindByIdAsync(id.ToString());

            if (existingRole == null)
            {
                return await Result<string>.FailAsync(string.Format(_localizer["Role not found"]));
            }

            if (existingRole.Name != RoleConstants.AdministratorRole)
            {
                var roleIsNotUsed = _context.UserRoles.Include(x => x.User).Any(r => r.RoleId.Equals(existingRole.Id) && r.User.IsActive);

                if (!roleIsNotUsed)
                {
                    await _roleManager.DeleteAsync(existingRole);
                    return await Result<string>.SuccessAsync(string.Format(_localizer["Role {0} Deleted."], existingRole.Name));
                }
                else
                {
                    return await Result<string>.FailAsync(string.Format(_localizer["Not allowed to delete {0} Role as it is being used."], existingRole.Name));
                }
            }
            else
            {
                return await Result<string>.FailAsync(string.Format(_localizer["Not allowed to delete {0} Role."], existingRole.Name));
            }
        }

        public async Task<Result<PaginatedResult<RoleResponse>>> GetAllAsync(RolePagingRequest request)
        {
            try
            {
                var tenantId = GetTenantId();
                RoleFilterSpecification spec = new RoleFilterSpecification(request.StringSearch);
                var query = _roleManager.Roles.Where(x => x.TenantId == tenantId).Specify(spec);
                if (!string.IsNullOrEmpty(request.SortColumn))
                {
                    query = query.ApplySorting(request.SortColumn, request.SortOrder);
                }
                var roleQuery = await query.ToPaginatedListAsync(request.PageNumber, request.PageSize);
                var roles = _mapper.Map<PaginatedResult<RoleResponse>>(roleQuery);
                return Result<PaginatedResult<RoleResponse>>.Success(roles);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<RoleResponse>>.Fail(ex.Message);
            }
        }

        public async Task<Result<PermissionResponse>> GetAllPermissionsAsync(Guid roleId)
        {
            var model = new PermissionResponse();
            var role = await _context.Roles.Include(x => x.RolePermissions).FirstOrDefaultAsync(x => x.Id.Equals(roleId));

            if (role != null)
            {
                model.RoleId = role.Id;
                model.RoleName = role.Name;

                var rolePermissions = role.RolePermissions.ToList();
                var defaultPermissions = GetDefaultPermissions();

                model.RolePermissions = rolePermissions.Join(defaultPermissions,
                    r => r.ClaimValue,
                    p => p.Value,
                    (rp, p) => new RolePermissionResponse
                    {
                        Id = rp.Id,
                        Value = p.Value,
                        Description = p.Description,
                        Group = p.Group,
                        RoleId = rp.RoleId,
                        Selected = true,
                        Type = p.Type
                    }).ToList();
            }

            return await Result<PermissionResponse>.SuccessAsync(model);
        }

        private List<RolePermissionResponse> GetDefaultPermissions()
        {
            var allPermissions = new List<RolePermissionResponse>();

            #region GetPermissions

            allPermissions.GetAllPermissions();

            #endregion GetPermissions

            return allPermissions;
        }

        public async Task<Result<RoleResponse>> GetByIdAsync(Guid id)
        {
            var tenantId = GetTenantId();
            var role = await _roleManager.Roles.SingleOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);
            return await Result<RoleResponse>.SuccessAsync(_mapper.Map<RoleResponse>(role));
        }

        public async Task<Result<string>> SaveAsync(RoleRequest request)
        {
            if (request.Id == Guid.Empty)
            {
                var existingRole = await _roleManager.FindByNameAsync(request.Name);

                if (existingRole != null) return await Result<string>.FailAsync(_localizer["Similar Role already exists."]);

                var response = await _roleManager.CreateAsync(new Role(request.Name, request.Description));

                if (response.Succeeded)
                {
                    return await Result<string>.SuccessAsync(string.Format(_localizer["Role {0} Created."], request.Name));
                }
                else
                {
                    return await Result<string>.FailAsync(response.Errors.Select(e => _localizer[e.Description].ToString()).ToList());
                }
            }
            else
            {
                var existingRole = await _roleManager.FindByIdAsync(request.Id.ToString());

                if (existingRole == null) return await Result<string>.FailAsync(_localizer["Role does not exists."]);

                if (existingRole.Name == RoleConstants.AdministratorRole)
                {
                    return await Result<string>.FailAsync(string.Format(_localizer["Not allowed to modify {0} Role."], existingRole.Name));
                }

                existingRole.Name = request.Name;
                existingRole.NormalizedName = request.Name.ToUpper();
                existingRole.Description = request.Description;

                await _roleManager.UpdateAsync(existingRole);

                return await Result<string>.SuccessAsync(string.Format(_localizer["Role {0} Updated."], existingRole.Name));
            }
        }

        public async Task<Result<string>> UpdatePermissionsAsync(PermissionRequest request)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());

                if (role == null) return await Result<string>.FailAsync(_localizer["Role does not exists."]);

                if (role.Name == RoleConstants.AdministratorRole)
                {
                    return await Result<string>.FailAsync(_localizer["Not allowed to modify Permissions for this Role."]);
                }

                var selectedClaims = request.RolePermissions.Where(a => a.Selected).ToList();

                var claims = _context.RolePermissions
                    .Where(x => x.RoleId.Equals(request.RoleId))
                    .ToList();

                var outdatedPermissions = claims
                    .Where(x => !selectedClaims.Any(r => r.Value.Equals(x.ClaimValue)))
                    .ToList();

                if (outdatedPermissions.Any())
                    _context.RolePermissions.RemoveRange(outdatedPermissions);

                var usersWithRoleOrPermission = await _context.UserRoles.Where(x => x.RoleId == request.RoleId).ToListAsync();

                var userIdsToUpdate = new List<string>();

                if (usersWithRoleOrPermission.Count > 0)
                {
                    foreach (var user in usersWithRoleOrPermission)
                    {
                        var cachedPermissions = await _cache.GetStringAsync(user.UserId.ToString());
                        if (!string.IsNullOrEmpty(cachedPermissions))
                        {
                            await _cache.RemoveAsync(user.UserId.ToString());
                            userIdsToUpdate.Add(user.UserId.ToString());
                        }
                    }
                }

                var unassignedClaims = selectedClaims
                    .Where(x => !claims.Any(r => r.ClaimValue.Equals(x.Value)))
                    .Select(x => new RolePermission
                    {
                        ClaimType = PermissionConstants.Name,
                        ClaimValue = x.Value,
                        RoleId = request.RoleId
                    })
                    .ToList();

                if (unassignedClaims.Any())
                {
                    await _context.RolePermissions.AddRangeAsync(unassignedClaims);
                }

                await _context.SaveChangesAsync();

                return await Result<string>.SuccessAsync(_localizer["Permissions Updated."]);
            }
            catch (Exception ex)
            {
                return await Result<string>.FailAsync(ex.Message);
            }
        }

        public async Task<int> GetCountAsync()
        {
            var tenantId = GetTenantId();
            return await _roleManager.Roles.Where(x => x.TenantId == tenantId).CountAsync();
        }

        public async Task<Result<string>> AddUpdateRolePermission(CreateUpdateRoleRequest request)
        {
            try
            {

                var usersWithRoleOrPermission = await _context.UserRoles.Where(x => x.RoleId == request.Id).ToListAsync();

                var userIdsToUpdate = new List<string>();

                if (usersWithRoleOrPermission.Count > 0)
                {
                    foreach (var user in usersWithRoleOrPermission)
                    {
                        var cachedPermissions = await _cache.GetStringAsync(user.UserId.ToString());
                        if (!string.IsNullOrEmpty(cachedPermissions))
                        {
                            await _cache.RemoveAsync(user.UserId.ToString());
                            userIdsToUpdate.Add(user.UserId.ToString());
                        }
                    }
                }

                if (request.Id == Guid.Empty)
                {
                    var existingRole = await _roleManager.FindByNameAsync(request.Name);

                    if (existingRole != null) return await Result<string>.FailAsync(_localizer["Similar Role already exists."]);

                    var rolePermission = _mapper.Map<CreateUpdateRoleRequest, Role>(request);
                    var response = await _roleManager.CreateAsync(rolePermission);

                    if (response.Succeeded)
                    {
                        return await Result<string>.SuccessAsync(string.Format(_localizer["Role {0} Created."], request.Name));
                    }
                    else
                    {
                        return await Result<string>.FailAsync(response.Errors.Select(e => _localizer[e.Description].ToString()).ToList());
                    }
                }
                else
                {
                    var existingRole = await _roleManager.FindByIdAsync(request.Id.ToString());

                    if (existingRole == null) return await Result<string>.FailAsync(_localizer["Role does not exists."]);

                    if (existingRole.Name == RoleConstants.AdministratorRole)
                    {
                        return await Result<string>.FailAsync(string.Format(_localizer["Not allowed to modify {0} Role."], existingRole.Name));
                    }
                    existingRole.Name = request.Name;
                    existingRole.NormalizedName = request.Name.ToUpper();
                    existingRole.Description = request.Description;
                    if (request.RolePermissions != null)
                    {
                        var existingPermission = _context.RolePermissions.Where(x => x.RoleId == request.Id).ToList();
                        _context.RolePermissions.RemoveRange(existingPermission);
                    }

                    var rolePermission = _mapper.Map(request, existingRole);

                    await _roleManager.UpdateAsync(rolePermission);

                    return await Result<string>.SuccessAsync(string.Format(_localizer["Role {0} Updated."], existingRole.Name));
                }
            }
            catch (Exception ex)
            {
                return await Result<string>.FailAsync(ex.Message);
            }
        }

        public async Task<Result<string>> DeleteMany(List<Guid> ids)
        {
            try
            {
                var tenantId = GetTenantId();
                var existingRoles = await _roleManager.Roles
                                        .Include(x => x.RolePermissions)
                                        .WhereIf(ids != null && ids.Count > 0, x => ids.Contains(x.Id) && x.TenantId == tenantId)
                                        .ToListAsync();

                if (existingRoles == null || existingRoles.Count <= 0)
                {
                    return await Result<string>.FailAsync(string.Format(_localizer["Role not found"]));
                }

                var admin = existingRoles.FirstOrDefault(x => x.Name == RoleConstants.AdministratorRole);

                if (admin != null)
                {
                    var roleIsNotUsed = _context.UserRoles.Include(x => x.User).Any(r => r.RoleId.Equals(admin.Id) && r.User.IsActive);

                    if (!roleIsNotUsed)
                    {
                        await _roleManager.DeleteAsync(admin);
                        return await Result<string>.SuccessAsync(string.Format(_localizer["Role {0} Deleted."], admin.Name));
                    }
                    else
                    {
                        return await Result<string>.FailAsync(string.Format(_localizer["Not allowed to delete {0} Role as it is being used."], admin.Name));
                    }
                }

                foreach (var role in existingRoles)
                {
                    var usersWithRoleOrPermission = await _context.UserRoles.Where(x => x.RoleId == role.Id).ToListAsync();

                    var userIdsToUpdate = new List<string>();

                    if (usersWithRoleOrPermission.Count > 0)
                    {
                        foreach (var user in usersWithRoleOrPermission)
                        {
                            var cachedPermissions = await _cache.GetStringAsync(user.UserId.ToString());
                            if (!string.IsNullOrEmpty(cachedPermissions))
                            {
                                await _cache.RemoveAsync(user.UserId.ToString());
                                userIdsToUpdate.Add(user.UserId.ToString());
                            }
                        }
                    }
                }

                _context.Roles.RemoveRange(existingRoles);
                await _context.SaveChangesAsync();

                return await Result<string>.SuccessAsync("Role deleted successfully");
            }
            catch (Exception ex)
            {
                return await Result<string>.FailAsync(ex.Message);
            }

        }

        private Guid? GetTenantId()
        {
            if (_httpContextAccessor.HttpContext?.Items?.TryGetValue("TenantId", out object tenantIdValue) ?? false)
            {
                if (Guid.TryParse(tenantIdValue?.ToString(), out Guid guidValue))
                {
                    return guidValue;
                }
            }

            return null;
        }
    }
}