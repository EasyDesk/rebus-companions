using EasyDesk.RebusCompanion.Core.Config;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Timeouts;

namespace EasyDesk.RebusCompanion.Core.Scheduler;

public delegate void TimeoutManagerConfiguration(StandardConfigurer<ITimeoutManager> timeouts);

public sealed class RebusScheduler : RebusProcess
{
    private readonly TimeoutManagerConfiguration _timeoutManagerConfiguration;

    public RebusScheduler(
        string endpoint,
        RebusConfiguration defaultConfiguration,
        TimeoutManagerConfiguration timeoutManagerConfiguration) : base(
            new BuiltinHandlerActivator(),
            endpoint,
            defaultConfiguration)
    {
        _timeoutManagerConfiguration = timeoutManagerConfiguration;
    }

    protected override void ConfigureRebusBus(RebusConfigurer configurer)
    {
        configurer.Timeouts(t => _timeoutManagerConfiguration(t));
    }
}
