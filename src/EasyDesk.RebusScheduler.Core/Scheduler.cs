using EasyDesk.Commons;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Timeouts;
using static EasyDesk.Commons.StaticImports;

namespace EasyDesk.RebusScheduler.Core;

public delegate void TimeoutManagerConfiguration(StandardConfigurer<ITimeoutManager> timeouts);

public sealed class Scheduler : IDisposable
{
    private readonly Func<IBus> _starter;
    private Option<IBus> _bus;

    public Scheduler(
        string endpoint,
        RebusConfiguration defaultConfiguration,
        TimeoutManagerConfiguration timeoutManagerConfiguration)
    {
        _starter = () => defaultConfiguration.Apply(Configure.With(new BuiltinHandlerActivator()), endpoint)
            .Timeouts(t => timeoutManagerConfiguration(t))
            .Start();
    }

    public void Start()
    {
        if (_bus.IsPresent)
        {
            throw new InvalidOperationException("Scheduler was already started");
        }

        _bus = Some(_starter());
    }

    public void Dispose() => _bus.IfPresent(bus => bus.Dispose());
}
