using EasyDesk.CleanArchitecture.Testing.Integration.Bus.Rebus;
using EasyDesk.RebusCompanion.Core.Config;
using EasyDesk.RebusCompanion.Core.Consumer;
using NodaTime;
using NSubstitute;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Routing.TypeBased;
using Rebus.Transport;

namespace EasyDesk.RebusCompanion.IntegrationTests.Consumers;

public abstract class RebusConsumerFixture : IAsyncLifetime
{
    public const string Endpoint = "consumer";

    private readonly RebusConfiguration _defaultConfiguration;
    private readonly RebusConsumer _consumer;

    public RebusConsumerFixture()
    {
        Handler = Substitute.For<IHandleMessages<object>>();
        var activator = new BuiltinHandlerActivator().Register(() => Handler);

        _defaultConfiguration = new RebusConfiguration()
            .WithTransport(ConfigureTransport);
        _consumer = new RebusConsumer(activator, Endpoint, _defaultConfiguration);
    }

    public IHandleMessages<object> Handler { get; }

    public RebusTestBus CreateBus(string endpoint, Duration? defaultTimeout = null)
    {
        return new RebusTestBus(
            rebus =>
            {
                _defaultConfiguration.Apply(rebus, endpoint);
                rebus.Routing(r => r.TypeBased().MapFallback(Endpoint));
            },
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
