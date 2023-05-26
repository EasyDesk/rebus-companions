using Microsoft.Extensions.DependencyInjection;

namespace EasyDesk.RebusCompanions.Core.HostedService;

public static class RebusProcessHostedServiceExtensions
{
    public static IServiceCollection AddRebusProcess<T>(this IServiceCollection services)
        where T : RebusProcess
    {
        return services.AddHostedService<RebusProcessHostedService<T>>();
    }
}
