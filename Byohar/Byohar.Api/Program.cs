using Byohar.Api.Services.User;
using Byohar.Application.Configurations;
using Byohar.Application.Extensions;
using Byohar.Application.Interfaces.Common;
using Byohar.Application.Interfaces.User;
using Byohar.Domain.Entities.Identity;
using Byohar.Infrastructure.Extensions;
using Byohar.Infrastructure.Services.Common;
using Byohar.Persistance.Contexts;
using Byohar.Persistence.Seeder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Byohar.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Layers
builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureServices();
builder.Services.AddRepositories();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Identity
builder.Services.AddIdentity<ApplicationUser, Role>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<AppConfiguration>(
    builder.Configuration.GetSection("JWTSettings"));
builder.Services.AddJwtAuthentication(
    builder.Configuration.GetSection("JWTSettings").Get<AppConfiguration>()
    ?? throw new InvalidOperationException("JWTSettings are required."));

// Common services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddTransient<IDateTimeService, DateTimeService>();
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFlutterWeb", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
                Uri.TryCreate(origin, UriKind.Absolute, out var uri) &&
                uri.IsLoopback && (uri.Scheme == "http" || uri.Scheme == "https"))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseCors("AllowFlutterWeb");
app.UseAuthentication();
app.UseMiddleware<Byohar.Api.Middlewares.TenantIdMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
