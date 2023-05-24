using EasyDesk.RebusCompanions.Core.Config;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Timeouts;

namespace EasyDesk.RebusCompanions.Core.Scheduler;

public delegate void TimeoutManagerConfiguration(StandardConfigurer<ITimeoutManager> timeouts);

public sealed class RebusScheduler : RebusProcess
{
    public const string DefaultEndpoint = "scheduler";

    private readonly TimeoutManagerConfiguration _timeoutManagerConfiguration;

    public RebusScheduler(
        RebusConfiguration defaultConfiguration,
        TimeoutManagerConfiguration timeoutManagerConfiguration,
        string endpoint = DefaultEndpoint) : base(
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
