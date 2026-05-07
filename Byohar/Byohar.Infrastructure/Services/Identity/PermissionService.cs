using AutoMapper;
using Byohar.Application.Interfaces.Identity;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Responses.Identity;
using Byohar.Infrastructure.Extensions;
using Byohar.Persistence.Contexts;
using Byohar.Shared.Wrapper;
using Microsoft.AspNetCore.Http;
using static Byohar.Shared.Permission.Permissions;

namespace Byohar.Infrastructure.Services.Identity
{
    public class PermissionService : IPermissionService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionService(
        IMapper mapper,
            ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<PaginatedResult<PermissionsResponse>>> GetAllAsync(PermissionPagingRequest request)
        {
            try
            {
                var tenantId = GetTenantId();
                var query = _context.Permissions
                 .Where(p => !string.Equals(p.Description, SystemPreferencesPermissions.All) && !string.Equals(p.DisplayName, "Roles") && !string.Equals(p.Description, "Role Permissions") && p.TenantId == tenantId)
                 .GroupBy(x => x.Group)
                 .Select(g => new PermissionsResponse
                 {
                     Group = g.Key,
                     PermissionDisplay = g.Select(p => new PermissionDisplay
                     {
                         Name = p.DisplayName,
                         Description = p.Description
                     }).ToList()
                 });
                var PermissionQuery = await query.ToPaginatedListAsync(request.PageNumber, request.PageSize);
                var Permissions = _mapper.Map<PaginatedResult<PermissionsResponse>>(PermissionQuery);
                return Result<PaginatedResult<PermissionsResponse>>.Success(Permissions);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<PermissionsResponse>>.Fail(ex.Message);
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
