using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Rebus.Config;
using Rebus.Routing;
using Rebus.Time;
using Rebus.Transport;

namespace EasyDesk.RebusCompanions.Core.Config;

public class RebusConfiguration
{
    private Action<RebusConfigurer, string>? _configure;
    private IClock _clock;

    public RebusConfiguration(IServiceProvider serviceProvider)
    {
        _clock = serviceProvider.GetRequiredService<IClock>();
    }

    public RebusConfiguration()
    {
        _clock = SystemClock.Instance;
    }

    public RebusConfiguration Configure(Action<RebusConfigurer> configure)
    {
        _configure += (rebus, _) => configure(rebus);
        return this;
    }

    public RebusConfiguration WithTransport(Action<StandardConfigurer<ITransport>, string> transport)
    {
        _configure += (rebus, endpoint) => rebus.Transport(t => transport(t, endpoint));
        return this;
    }

    public RebusConfiguration WithClock(IClock clock)
    {
        _clock = clock;
        return this;
    }

    public RebusConfiguration WithOptions(Action<OptionsConfigurer> options)
    {
        _configure += (rebus, _) => rebus.Options(options);
        return this;
    }

    public RebusConfiguration WithLogging(Action<RebusLoggingConfigurer> logging)
    {
        _configure += (rebus, _) => rebus.Logging(logging);
        return this;
    }

    public RebusConfiguration WithRouting(Action<StandardConfigurer<IRouter>> routing)
    {
        _configure += (rebus, _) => rebus.Routing(routing);
        return this;
    }

    public RebusConfigurer Apply(RebusConfigurer rebus, string endpoint)
    {
        _configure?.Invoke(rebus, endpoint);
        rebus.Options(o =>
        {
            o.Decorate<IRebusTime>(_ => new NodaTimeRebusClock(_clock));
        });
        return rebus;
    }
}
