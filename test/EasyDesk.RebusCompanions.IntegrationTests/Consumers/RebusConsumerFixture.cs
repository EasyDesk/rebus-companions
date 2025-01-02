using EasyDesk.CleanArchitecture.Testing.Integration.Bus;
using EasyDesk.CleanArchitecture.Testing.Integration.Bus.Rebus;
using EasyDesk.CleanArchitecture.Testing.Unit.Commons;
using EasyDesk.RebusCompanions.Core.Config;
using EasyDesk.RebusCompanions.Core.Consumer;
using Newtonsoft.Json.Linq;
using NodaTime;
using NSubstitute;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Routing.TypeBased;
using Rebus.Transport;
using static EasyDesk.Commons.StaticImports;

namespace EasyDesk.RebusCompanions.IntegrationTests.Consumers;

public abstract class RebusConsumerFixture : IAsyncLifetime
{
    public const string Endpoint = "consumer";

    private readonly RebusConfiguration _defaultConfiguration;
    private readonly RebusConsumer _consumer;

    public RebusConsumerFixture()
    {
        Handler = Substitute.For<IHandleMessages<JObject>>();
        var activator = new BuiltinHandlerActivator().Register(() => Handler);

        _defaultConfiguration = new RebusConfiguration()
            .WithTransport(ConfigureTransport);
        _consumer = new RebusConsumer(activator, Endpoint, _defaultConfiguration);
    }

    public IHandleMessages<JObject> Handler { get; }

    public ITestBusEndpoint CreateBus(string endpoint, Duration? defaultTimeout = null)
    {
        return new RebusTestBusEndpoint(
            rebus =>
            {
                _defaultConfiguration.Apply(rebus, endpoint);
                rebus.Routing(r => r.TypeBased().MapFallback(Endpoint));
            },
            new TestTenantManager(None),
            defaultTimeout);
    }

    protected abstract void ConfigureTransport(StandardConfigurer<ITransport> transport, string endpoint);

    public async Task InitializeAsync()
    {
        await Start();
        _consumer.Start();
    }

    public async Task DisposeAsync()
    {
        _consumer.Dispose();
        await Stop();
    }

    protected abstract Task Start();

    protected abstract Task Stop();
}
