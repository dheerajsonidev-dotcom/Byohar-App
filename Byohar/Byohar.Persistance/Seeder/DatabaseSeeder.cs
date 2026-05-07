using Byohar.Application.Interfaces.Common;
using Byohar.Domain.Entities.Identity;
using Byohar.Persistence.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Byohar.Persistence.Seeder;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IStringLocalizer<DatabaseSeeder> _localizer;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<DatabaseSeeder> _logger;
    private Guid? _tenantId;
    public DatabaseSeeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<Role> roleManager, ILogger<DatabaseSeeder> logger, IStringLocalizer<DatabaseSeeder> localizer, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _localizer = localizer;
        _httpContextAccessor = httpContextAccessor;
    }

    public void Initialize()
    {
        _tenantId = GetTenantId();
        //Task.Run(SeedTenantData).GetAwaiter().GetResult();
        //Task.Run(SeedDefaultAdmin).GetAwaiter().GetResult(); 
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