using Byohar.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Byohar.Persistence.Extensions
{
    public static class DbContextModelCreatingExtension
    {
        public static void ConfigureConnexus(this ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()

            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.Name is "LastModifiedBy" or "CreatedBy" or "DeletedBy"))
            {
                property.SetColumnType("nvarchar(128)");
            }

            #region UserAndRoles

            builder.Entity<RolePermission>(b =>
            {
                b.ToTable("RolePermissions");

                b.HasOne(x => x.Role).WithMany(x => x.RolePermissions).HasForeignKey(x => x.RoleId);
            });

            builder.Entity<UserPermission>().ToTable("UserPermissions");

            builder.Entity<UserRole>(b =>
            {
                b.ToTable("UserRoles");

                b.HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId);
                b.HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId);
            });

            builder.Entity<Permission>(b =>
            {
                b.ToTable("Permissions");

                b.HasIndex(c => c.Group);
            });

            builder.Entity<UserLoginDeviceHistory>(b =>
            {
                b.ToTable("UserLoginDeviceHistories");

                b.HasIndex(c => c.UserId);

                b.HasOne(x => x.User).WithMany(x => x.LoginDeviceHistories).HasForeignKey(x => x.UserId);
            });

            #endregion

         

          

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        var converter = new ValueConverter<DateTime, DateTime>(
                            v => v,
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                        );
                        property.SetValueConverter(converter);
                    }
                }
            }
        }
    }
}