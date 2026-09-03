using Byohar.Domain.Entities.Addresses;
using Byohar.Domain.Entities.Guests;
using Byohar.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Byohar.Persistence.Extensions
{
    public static class DbContextModelCreatingExtension
    {
        public static void ConfigureByohar(this ModelBuilder builder)
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


            #region Address


            builder.Entity<Address>(b =>
            {
                b.ToTable("Addresses");

                b.HasKey(x => x.Id);

                b.Property(x => x.AddressLine1)
                    .HasMaxLength(250);

                b.Property(x => x.AddressLine2)
                    .HasMaxLength(250);

                b.Property(x => x.City)
                    .HasMaxLength(100);

                b.Property(x => x.State)
                    .HasMaxLength(100);

                b.Property(x => x.PinCode)
                    .HasMaxLength(20);

                b.Property(x => x.Country)
                    .HasMaxLength(100)
                    .HasDefaultValue("India");
            });

            #endregion

            #region Guest

            builder.Entity<Guest>(b =>
            {









                b.ToTable("Guests");

                b.HasKey(x => x.Id);

                b.Property(x => x.FirstName)
                    .HasMaxLength(150)
                    .IsRequired();

                b.Property(x => x.LastName)
                    .HasMaxLength(150);

                b.Property(x => x.MobileNumber)
                    .HasMaxLength(20);

                b.Property(x => x.Relation)
                    .HasMaxLength(100);

                b.Property(x => x.TotalMembers)
                    .HasDefaultValue(1);

                b.Property(x => x.IsActive)
                    .HasDefaultValue(true);

                b.HasIndex(x => x.EventId);

                b.HasIndex(x => x.MobileNumber);

                b.HasOne(x => x.Address)
                    .WithMany()
                    .HasForeignKey(x => x.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);
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