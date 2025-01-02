using EasyDesk.CleanArchitecture.Testing.Integration.Bus;
using EasyDesk.CleanArchitecture.Testing.Integration.Bus.Rebus;
using EasyDesk.CleanArchitecture.Testing.Unit.Commons;
using EasyDesk.RebusCompanions.Core.Config;
using EasyDesk.RebusCompanions.Core.Scheduler;
using NodaTime;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Timeouts;
using Rebus.Transport;
using static EasyDesk.Commons.StaticImports;

namespace EasyDesk.RebusCompanions.IntegrationTests.Schedulers;

public abstract class RebusSchedulerFixture : IAsyncLifetime
{
    public const string Endpoint = "test-timeout-manager";

    private readonly RebusConfiguration _defaultConfiguration;
    private readonly RebusScheduler _scheduler;

    public RebusSchedulerFixture()
    {
        _defaultConfiguration = new RebusConfiguration()
            .WithTransport(ConfigureTransport);
        _scheduler = new RebusScheduler(_defaultConfiguration, ConfigureTimeouts, Endpoint);
    }

    protected abstract void ConfigureTransport(StandardConfigurer<ITransport> transport, string endpoint);

    protected abstract void ConfigureTimeouts(StandardConfigurer<ITimeoutManager> timeouts);

    public ITestBusEndpoint CreateBus(string endpoint, string defaultDestination, Duration? defaultTimeout = null)
    {
        return new RebusTestBusEndpoint(
            rebus =>
            {
                _defaultConfiguration.Apply(rebus, endpoint);
                rebus.Timeouts(t => t.UseExternalTimeoutManager(Endpoint));
                rebus.Routing(r => r.TypeBased().MapFallback(defaultDestination));
            },
            new TestTenantManager(None),
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
