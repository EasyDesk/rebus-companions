using EasyDesk.CleanArchitecture.Testing.Integration.Bus;
using EasyDesk.CleanArchitecture.Testing.Integration.Bus.Rebus;
using EasyDesk.CleanArchitecture.Testing.Integration.Multitenancy;
using EasyDesk.RebusCompanions.Core.Config;
using NodaTime;
using NodaTime.Testing;
using Rebus.Config;
using Rebus.Persistence.InMem;
using Rebus.Subscriptions;
using Rebus.Transport.InMem;
using static EasyDesk.Commons.StaticImports;

namespace EasyDesk.RebusCompanions.IntegrationTests;

public abstract class AbstractRebusTest
{
    protected AbstractRebusTest()
    {
        var network = new InMemNetwork();
        var subscriberStore = new InMemorySubscriberStore();

        Configuration = new RebusConfiguration()
            .WithTransport((t, e) =>
            {
                t.UseInMemoryTransport(network, e, registerSubscriptionStorage: false);
                t.OtherService<ISubscriptionStorage>().StoreInMemory(subscriberStore);
            })
            .WithClock(Clock);

        ConfigureRebus(Configuration);
    }

    protected FakeClock Clock { get; } = new(Instant.FromUtc(2025, 01, 01, 16, 00));

    protected RebusConfiguration Configuration { get; }

    protected virtual void ConfigureRebus(RebusConfiguration configuration)
    {
    }

    protected ITestBusEndpoint CreateBus(string endpoint, Action<RebusConfigurer>? configure = null, Duration? defaultTimeout = null)
    {
        return new RebusTestBusEndpoint(
            rebus =>
            {
                Configuration.Apply(rebus, endpoint);
                configure?.Invoke(rebus);
            },
            new TestTenantManager(new(None)),
            defaultTimeout);
    }
}
