using Byohar.Api.Services.User;
using Byohar.Application.Configurations;
using Byohar.Application.Hubs;
using Byohar.Application.Hubs.Notification;
using Byohar.Application.Hubs.RolePermission;
using Byohar.Application.Interfaces.Common;
using Byohar.Application.Interfaces.User;
using Byohar.Infrastructure.Services.Common;
using Byohar.Persistance.Contexts;
using Byohar.Shared.Permission;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;
using System.Text;
using static Byohar.Shared.Constants.Common.ApplicationConstants;

namespace Byohar.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    internal static IApplicationBuilder UseExceptionHandling(
        this IApplicationBuilder app,
        IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        return app;
    }

    internal static AppConfiguration GetApplicationSettings(
       this IServiceCollection services,
       IConfiguration configuration)
    {
        var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
        services.Configure<AppConfiguration>(applicationSettingsConfiguration);
        return applicationSettingsConfiguration.Get<AppConfiguration>();
    }

    internal static IApplicationBuilder UseEndpoints(this IApplicationBuilder app)
            => app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
                endpoints.MapHub<ChatHub>(SignalR.ChatHubUrl);
                endpoints.MapHub<NotificationHub>(SignalR.NotificationHubUrl);
                endpoints.MapHub<IdleDetectionHub>(SignalR.IdleDetectionHubUrl);
                endpoints.MapHub<RolePermissionUpdateHub>(SignalR.RolePermissionUpdateHubUrl);
            });

    internal static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<INotificationEmailService, NotificationEmailService>();

        return services;
    }

    internal static IApplicationBuilder Initialize(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();

        var initializer = serviceScope.ServiceProvider.GetService<IDatabaseSeeder>();

        initializer?.Initialize();

        return app;
    }

    internal static void RegisterSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(async c =>
        {
            //TODO - Lowercase Swagger Documents
            //c.DocumentFilter<LowercaseDocumentFilter>();
            //Refer - https://gist.github.com/rafalkasa/01d5e3b265e5aa075678e0adfd54e23f

            // include all project's xml comments
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.IsDynamic)
                {
                    var xmlFile = $"{assembly.GetName().Name}.xml";
                    var xmlPath = Path.Combine(baseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                    {
                        c.IncludeXmlComments(xmlPath);
                    }
                }
            }

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Connexus",
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Add custom header for Client-Time-Zone
            c.AddSecurityDefinition("Client-Time-Zone", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Client-Time-Zone",
                Type = SecuritySchemeType.ApiKey,
                Description = "Client Time Zone"
            });

            // Add JWT authentication
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
            });

            // Combine security requirements under a single scheme
            
        });
    }

    internal static void AddPolicies(this IServiceCollection services)
    {
        var permissions = Permissions.GetRegisteredPermissions();

        services.AddAuthorization(options =>
        {
            foreach (var permission in permissions)
            {
                options.AddPolicy(permission, policy => policy.RequireClaim(PermissionConstants.Name, permission));
            }
        });
    }

    internal static IServiceCollection AddJwtAuthentication(
           this IServiceCollection services, AppConfiguration config)
    {
        var key = Encoding.UTF8.GetBytes(config.Secret);

        services.AddAuthentication(authentication =>
            {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearer =>
            {
                bearer.RequireHttpsMetadata = false;
                bearer.SaveToken = true;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.Zero
                };

                bearer.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/chatHub") || path.StartsWithSegments("/notificationHub") || path.StartsWithSegments("/idleDetectionHub") || path.StartsWithSegments("/rolePermissionUpdateHub")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }

                };
            });

        return services;
    }

    internal static IMvcBuilder AddValidators(this IMvcBuilder builder)
    {
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddFluentValidationClientsideAdapters();
        builder.Services.AddValidatorsFromAssemblyContaining<AppConfiguration>();

        return builder;
    }

    public static void MigrateDatabase(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();

        var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

        var pendingMigrations = dbContext.Database.GetPendingMigrations();

        if (pendingMigrations != null && pendingMigrations.Any())
        {
            dbContext.Database.Migrate();

            Console.WriteLine("Pending migrations applied successfully.");
        }
        else
        {
            Console.WriteLine("No pending migrations found.");
        }
    }
}