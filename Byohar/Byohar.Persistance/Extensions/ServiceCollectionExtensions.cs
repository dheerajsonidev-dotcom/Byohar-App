using Byohar.Application.Interfaces.Persistence;
using Byohar.Domain.Entities.Identity;
using Byohar.Infrastructure.Repositories;
using Byohar.Persistance.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Byohar.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>))
                .AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
        }
    }
}
