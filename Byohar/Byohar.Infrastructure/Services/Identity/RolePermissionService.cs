using AutoMapper;
using Byohar.Application.Interfaces.Identity;
using Byohar.Application.Interfaces.User;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Responses.Identity;
using Byohar.Domain.Entities.Identity;
using Byohar.Persistence.Contexts;
using Byohar.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SkiaSharp;

namespace Byohar.Infrastructure.Services.Identity
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IStringLocalizer<RolePermissionService> _localizer;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly ApplicationDbContext _dbContext;

        public RolePermissionService(
            IStringLocalizer<RolePermissionService> localizer,
            IMapper mapper,
            IDistributedCache cache,
            ApplicationDbContext db)
        {
            _localizer = localizer;
            _mapper = mapper;
            _cache = cache;
            _dbContext = db;
        }

        public async Task<Result<List<RolePermissionResponse>>> GetAllAsync()
        {
            var rolePermissions = await _dbContext.RolePermissions.ToListAsync();
            return await Result<List<RolePermissionResponse>>.SuccessAsync(_mapper.Map<List<RolePermissionResponse>>(rolePermissions));
        }

        public async Task<int> GetCountAsync()
        {
            return await _dbContext.RolePermissions.CountAsync();
        }

        public async Task<Result<RolePermissionResponse>> GetByIdAsync(int id)
        {
            var rolePermission = await _dbContext.RolePermissions.SingleOrDefaultAsync(x => x.Id == id);
            return await Result<RolePermissionResponse>.SuccessAsync(_mapper.Map<RolePermissionResponse>(rolePermission));
        }

        public async Task<Result<List<RolePermissionResponse>>> GetAllByRoleIdAsync(Guid roleId)
        {
            var RolePermissions = await _dbContext.RolePermissions
                .Include(x => x.Role)
                .Where(x => x.RoleId == roleId)
                .Select(g => new RolePermissionResponse
                {

                    Id = g.Id,
                    Selected = true,
                    Group = "",
                    Description = g.ClaimValue,
                    Value = g.ClaimValue,
                    RoleId = roleId

                })
                .ToListAsync();

            var RolePermissionsResponse = _mapper.Map<List<RolePermissionResponse>>(RolePermissions);
            return await Result<List<RolePermissionResponse>>.SuccessAsync(RolePermissionsResponse);
        }

        public async Task<Result<string>> SaveAsync(RolePermissionRequest request)
        {
            if (request.RoleId == Guid.Empty)
            {
                return await Result<string>.FailAsync(_localizer["Role is required."]);
            }

            var usersWithRoleOrPermission = await _dbContext.UserRoles.Where(x => x.RoleId == request.RoleId).ToListAsync();

            var userIdsToUpdate = new List<string>();

            if(usersWithRoleOrPermission.Count > 0)
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


            if (request.Id == 0)
            {
                var existingRolePermission = await _dbContext.RolePermissions.SingleOrDefaultAsync(x =>
                            x.RoleId == request.RoleId && x.ClaimType == request.Type && x.ClaimValue == request.Value);

                if (existingRolePermission != null)
                {
                    return await Result<string>.FailAsync(_localizer["Similar Role Claim already exists."]);
                }

                var rolePermission = _mapper.Map<RolePermission>(request);
                await _dbContext.RolePermissions.AddAsync(rolePermission);
                await _dbContext.SaveChangesAsync();

                return await Result<string>.SuccessAsync(string.Format(_localizer["Role Claim {0} created."], request.Value));
            }
            else
            {
                var existingRolePermission =
                    await _dbContext.RolePermissions
                        .Include(x => x.Role)
                        .SingleOrDefaultAsync(x => x.Id == request.Id);

                if (existingRolePermission == null)
                {
                    return await Result<string>.SuccessAsync(_localizer["Role Claim does not exist."]);
                }
                else
                {
                    Permission permission = new(Guid.NewGuid(), request.Description, request.Group, request.DisplayName);

                    existingRolePermission.ClaimType = request.Type;
                    existingRolePermission.ClaimValue = request.Value;
                    existingRolePermission.RoleId = request.RoleId;

                    _dbContext.RolePermissions.Update(existingRolePermission);
                    await _dbContext.SaveChangesAsync();

                    return await Result<string>.SuccessAsync(string.Format(_localizer["Role Claim {0} for Role {1} updated."], request.Value, existingRolePermission.Role.Name));
                }
            }
        }

        public async Task<Result<string>> DeleteAsync(int id)
        {
            var existingRolePermission = await _dbContext.RolePermissions.FirstOrDefaultAsync(x => x.Id == id);

            if (existingRolePermission != null)
            {
                _dbContext.RolePermissions.Remove(existingRolePermission);
                await _dbContext.SaveChangesAsync();

                return await Result<string>.SuccessAsync(string.Format(_localizer["Role Claim {0} for {1} Role deleted."], existingRolePermission.ClaimValue, existingRolePermission.Role.Name));
            }
            else
            {
                return await Result<string>.FailAsync(_localizer["Role Claim does not exist."]);
            }
        }

        public async Task<Result<bool>> CheckRoleHasPermissionAsync(string role, string permission)
        {
            var rolePermissions = _dbContext.RolePermissions.Include(x => x.Role).Any(x => x.Role.Name == role && x.ClaimValue.Equals(permission, StringComparison.OrdinalIgnoreCase));
            return await Result<bool>.SuccessAsync(data: rolePermissions);
        }

        public async Task<Result<List<RolePermissionResponse>>> GetPermissionsByRoleIdsAndUserId(UserRolePermissionRequest request)
        {
            var RolePermissions = new List<RolePermissionResponse>();
            var UserPermissions = new List<RolePermissionResponse>();


            RolePermissions = await _dbContext.RolePermissions
           .Include(x => x.Role)
           .Where(x => request.RoleIds.Contains(x.RoleId))
           .Select(g => new RolePermissionResponse
           {

               Id = g.Id,
               Selected = true,
               Group = "",
               Description = g.ClaimValue,
               Value = g.ClaimValue,
               RoleId = g.RoleId

           })
           .GroupBy(rp => rp.Value) // Group by 'Value' property
           .Select(group => group.First())
           .ToListAsync();


            UserPermissions = await _dbContext.UserPermissions
           .Where(x => x.UserId == request.UserId)
           .Select(g => new RolePermissionResponse
           {

               Id = g.Id,
               Selected = true,
               Group = "",
               Description = g.ClaimValue,
               Value = g.ClaimValue,
               UserId = g.UserId,

           })
           .GroupBy(rp => rp.Value) // Group by 'Value' property
           .Select(group => group.First())
           .ToListAsync();
            if (UserPermissions.Count > 0)
            {
                RolePermissions.AddRange(UserPermissions);
            }
            var RolePermissionsResponse = _mapper.Map<List<RolePermissionResponse>>(RolePermissions);
            return await Result<List<RolePermissionResponse>>.SuccessAsync(RolePermissionsResponse);
        }

        public async Task<Result<List<UserPermissionResponse>>> GetPermissions(string id)
        {
            var cachedPermissions = await _cache.GetStringAsync(id);

            Dictionary<string, List<UserPermissionResponse>> permissionsMap;

            if (!string.IsNullOrEmpty(cachedPermissions))
            {
                permissionsMap = JsonConvert.DeserializeObject<Dictionary<string, List<UserPermissionResponse>>>(cachedPermissions);
            }
            else
            {
                permissionsMap = new Dictionary<string, List<UserPermissionResponse>>();

                List<UserRole> userRoles = await _dbContext.UserRoles.Where(x => x.UserId.ToString() == id).ToListAsync();

                foreach (var userRole in userRoles)
                {
                    var roleId = userRole.RoleId;

                    var rolePermissions = await _dbContext.RolePermissions
                        .Include(x => x.Role)
                        .Where(x => x.Role.Id == roleId)
                        .Select(g => new UserPermissionResponse
                        {
                            Description = g.ClaimValue
                        })
                        .Distinct()
                        .ToListAsync();

                    permissionsMap[id] = rolePermissions;
                }

                var userPermissions = await _dbContext.UserPermissions
                    .Where(u => u.UserId.ToString() == id)
                    .Select(p => new UserPermissionResponse
                    {
                        Description = p.ClaimValue
                    })
                    .Distinct()
                    .ToListAsync();

                if (!permissionsMap.ContainsKey(id))
                    permissionsMap[id] = new List<UserPermissionResponse>();

                var uniquePermissions = new HashSet<UserPermissionResponse>(permissionsMap[id]);

                uniquePermissions.UnionWith(userPermissions);

                permissionsMap[id] = uniquePermissions.ToList();

                await _cache.SetStringAsync(id, JsonConvert.SerializeObject(permissionsMap), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                });
            }

            var permissionsForModule = permissionsMap.Values
                .SelectMany(permissionList => permissionList)
                .ToList();

            return await Result<List<UserPermissionResponse>>.SuccessAsync(permissionsForModule);
        }

    }
}