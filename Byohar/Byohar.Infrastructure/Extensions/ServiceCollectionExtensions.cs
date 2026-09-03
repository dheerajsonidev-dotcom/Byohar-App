using Byohar.Application.Interfaces.Email;
using Byohar.Application.Interfaces.Email.IEmailPopulate;
using Byohar.Application.Interfaces.Identity;
using Byohar.Infrastructure.Services.Email;
using Byohar.Infrastructure.Services.Email.EmailPopulate;
using Byohar.Infrastructure.Services.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Byohar.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRolePermissionService, RolePermissionService>();
            services.AddTransient<ITokenService, IdentityService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IOwnerAuthService, OwnerAuthService>();
            services.AddTransient<IUserService, UserService>();
            services.AddSingleton<IEmailSender, EmailSender>();



            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
