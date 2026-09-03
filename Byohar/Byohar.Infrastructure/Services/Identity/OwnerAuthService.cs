using Byohar.Application.Interfaces.Identity;
using Byohar.Application.Requests.Identity;
using Byohar.Application.Responses.Identity;
using Byohar.Domain.Entities.Identity;
using Byohar.Domain.Entities.Tenant;
using Byohar.Persistance.Contexts;
using Byohar.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Byohar.Infrastructure.Services.Identity;

public class OwnerAuthService : IOwnerAuthService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public OwnerAuthService(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<Role> roleManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IResult<OwnerRegisterResponse>> RegisterOwnerAsync(OwnerRegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return await Result<OwnerRegisterResponse>.FailAsync("Email already registered.");
        }

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.TenantName
        };

        await _dbContext.Tenants.AddAsync(tenant);
        await _dbContext.SaveChangesAsync();

        var adminRole = await _roleManager.FindByNameAsync("Admin");

        if (adminRole == null)
        {
            adminRole = new Role
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Name = "Admin",
                NormalizedName = "ADMIN",
                CreatedOn = DateTime.UtcNow
            };

            var roleResult = await _roleManager.CreateAsync(adminRole);

            if (!roleResult.Succeeded)
            {
                return await Result<OwnerRegisterResponse>.FailAsync(
                    roleResult.Errors.Select(x => x.Description).ToList());
            }
        }

        var ownerUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            NormalizedEmail = request.Email.ToUpper(),
            NormalizedUserName = request.Email.ToUpper(),
            PhoneNumber = request.PhoneNumber,
            EmailConfirmed = true,
            IsActive = true
        };

        var userResult = await _userManager.CreateAsync(ownerUser, request.Password);

        if (!userResult.Succeeded)
        {
            return await Result<OwnerRegisterResponse>.FailAsync(
                userResult.Errors.Select(x => x.Description).ToList());
        }

        var assignRoleResult = await _userManager.AddToRoleAsync(ownerUser, "Admin");

        if (!assignRoleResult.Succeeded)
        {
            return await Result<OwnerRegisterResponse>.FailAsync(
                assignRoleResult.Errors.Select(x => x.Description).ToList());
        }

        var response = new OwnerRegisterResponse
        {
            TenantId = tenant.Id,
            UserId = ownerUser.Id,
            TenantName = tenant.Name,
            Email = ownerUser.Email,
            Role = "Admin"
        };

        return await Result<OwnerRegisterResponse>.SuccessAsync(
            response,
            "Owner registered successfully.");
    }
}