using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyDesk.RebusCompanions.EfCore.Initializer;

public static class DbInitializerExtensions
{
    public static IServiceCollection InitializeDb(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
    {
        services.AddDbContext<EmptyContext>(options);
        services.AddHostedService<DbInitializer>();
        return services;
    }
}
