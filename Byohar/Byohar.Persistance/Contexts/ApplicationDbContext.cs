using Byohar.Application.Interfaces.Common;
using Byohar.Application.Interfaces.User;
using Byohar.Domain.Common;
using Byohar.Domain.Entities.Addresses;
using Byohar.Domain.Entities.Events;
using Byohar.Domain.Entities.Guests;
using Byohar.Domain.Entities.Identity;
using Byohar.Domain.Entities.Tenant;
using Byohar.Persistance.Contexts;
using Byohar.Persistence.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Byohar.Persistance.Contexts
{
    public class ApplicationDbContext : AuditableContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService, IDateTimeService dateTimeService, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Tables

        public DbSet<UserLoginDeviceHistory> UserLoginDeviceHistories { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Event> Events { get; set; }

        #endregion


        public DbSet<Guest> Guests { get; set; }
        public DbSet<Address> Addresses { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries().Where(e =>
                (e.Entity is Event or Guest or Address) &&
                (e.State == EntityState.Added || e.State == EntityState.Modified)))
            {
                var tenantId = GetTenantId()
                    ?? throw new InvalidOperationException("An authenticated wedding owner is required.");
                var audit = (IAuditableEntity)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    audit.TenantId = tenantId;
                    entry.Property("TenantId").CurrentValue = tenantId;
                    audit.CreatedOn = _dateTimeService.NowUtc;
                    audit.CreatedBy = _currentUserService.UserId ?? string.Empty;
                }
                else
                {
                    if (!Equals(entry.Property("TenantId").OriginalValue, tenantId))
                        throw new InvalidOperationException("This record belongs to another owner.");
                    entry.Property("TenantId").CurrentValue = tenantId;
                    audit.LastModifiedOn = _dateTimeService.NowUtc;
                    audit.LastModifiedBy = _currentUserService.UserId;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }



        //public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        //{
        //    var tenantId = GetTenantId();

        //    if (tenantId.HasValue)
        //    {
        //        foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
        //        {
        //            var tenantIdProperty = entry.Entity.GetType().GetProperty("TenantId");
        //            if (tenantIdProperty != null && tenantIdProperty.CanWrite)
        //                tenantIdProperty.SetValue(entry.Entity, tenantId.Value);
        //        }
        //    }

        //    //var changedEntities = ChangeTracker.Entries<IAuditableEntity>().ToList();

        //    //foreach (var entry in changedEntities)
        //    //{
        //    //    foreach (var property in entry.Properties)
        //    //    {
        //    //        if (property.Metadata.ClrType == typeof(DateTime) || property.Metadata.ClrType == typeof(DateTime?))
        //    //        {
        //    //            if (property.CurrentValue is DateTime dateTimeValue)
        //    //            {
        //    //                var offset = GetTimezoneOffset();
        //    //                property.CurrentValue = SafeConvertToUtc(dateTimeValue, offset);
        //    //            }
        //    //        }
        //    //    }

        //    //    if (entry.Entity is IFullAuditableEntity fullAuditableEntity && fullAuditableEntity.IsDeleted && entry.State != EntityState.Deleted)
        //    //    {
        //    //        fullAuditableEntity.DeletedOn = _dateTimeService.NowUtc;
        //    //        fullAuditableEntity.DeletedBy = _currentUserService.UserId;

        //    //        continue;
        //    //    }

        //    //    switch (entry.State)
        //    //    {
        //    //        case EntityState.Added:
        //    //            entry.Entity.CreatedOn = _dateTimeService.NowUtc;
        //    //            entry.Entity.CreatedBy = _currentUserService.UserId ?? string.Empty;
        //    //            break;

        //    //        case EntityState.Modified:
        //    //            entry.Entity.LastModifiedOn = _dateTimeService.NowUtc;
        //    //            entry.Entity.LastModifiedBy = _currentUserService.UserId;
        //    //            break;
        //    //    }
        //    //}

        //    if (_currentUserService.UserId == null)
        //    {
        //        return await base.SaveChangesAsync(cancellationToken);
        //    }

        //}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigureByohar();
        }

        private TimeSpan GetTimezoneOffset()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && httpContext.Items.ContainsKey("TimezoneOffset"))
            {
                var offsetValue = httpContext.Items["TimezoneOffset"]?.ToString();
                if (double.TryParse(offsetValue, out var offset))
                    return TimeSpan.FromHours(offset);
            }
            return TimeSpan.Zero;
        }

        private static DateTime SafeConvertToUtc(DateTime dateTime, TimeSpan offset)
        {
            DateTime result;
            try
            {
                if (dateTime.Kind == DateTimeKind.Utc)
                {
                    return dateTime;
                }

                result = DateTime.SpecifyKind(dateTime - offset, DateTimeKind.Utc);
            }
            catch (ArgumentOutOfRangeException)
            {
                result = dateTime.Kind == DateTimeKind.Utc ? dateTime : dateTime.ToUniversalTime();
            }
            return result;
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
