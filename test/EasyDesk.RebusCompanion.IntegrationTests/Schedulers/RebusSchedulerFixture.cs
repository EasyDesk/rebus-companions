using EasyDesk.CleanArchitecture.Testing.Integration.Bus.Rebus;
using EasyDesk.RebusCompanion.Core.Config;
using EasyDesk.RebusCompanion.Core.Scheduler;
using NodaTime;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Timeouts;
using Rebus.Transport;

namespace EasyDesk.RebusCompanion.IntegrationTests.Schedulers;

public abstract class RebusSchedulerFixture : IAsyncLifetime
{
    public const string Endpoint = "test-timeout-manager";

    private readonly RebusConfiguration _defaultConfiguration;
    private readonly RebusScheduler _scheduler;

    public RebusSchedulerFixture()
    {
        _defaultConfiguration = new RebusConfiguration()
            .WithTransport(ConfigureTransport);
        _scheduler = new RebusScheduler(Endpoint, _defaultConfiguration, ConfigureTimeouts);
    }

    protected abstract void ConfigureTransport(StandardConfigurer<ITransport> transport, string endpoint);

    protected abstract void ConfigureTimeouts(StandardConfigurer<ITimeoutManager> timeouts);

    public RebusTestBus CreateBus(string endpoint, string defaultDestination, Duration? defaultTimeout = null)
    {
        return new RebusTestBus(
            rebus =>
            {
                _defaultConfiguration.Apply(rebus, endpoint);
                rebus.Timeouts(t => t.UseExternalTimeoutManager(Endpoint));
                rebus.Routing(r => r.TypeBased().MapFallback(defaultDestination));
            },
            defaultTimeout);
    }

    public async Task InitializeAsync()
    {
        await Start();
        _scheduler.Start();
    }

    public async Task DisposeAsync()
    {
        _scheduler.Dispose();
        await Stop();
    }

    protected abstract Task Start();

    protected abstract Task Stop();
}
