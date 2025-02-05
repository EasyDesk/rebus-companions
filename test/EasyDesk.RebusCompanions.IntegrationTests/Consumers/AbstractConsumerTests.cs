using EasyDesk.CleanArchitecture.Testing.Integration.Bus;
using EasyDesk.RebusCompanions.Core.Consumer;
using Newtonsoft.Json.Linq;
using Rebus.Activation;
using Rebus.Handlers;
using Rebus.Routing.TypeBased;

namespace EasyDesk.RebusCompanions.IntegrationTests.Consumers;

public abstract class AbstractConsumerTests : AbstractRebusTest, IDisposable
{
    public const string Endpoint = "consumer";

    private readonly BuiltinHandlerActivator _activator = new();
    private readonly RebusConsumer _consumer;

    public AbstractConsumerTests()
    {
        Sender = CreateBus("sender", rebus => rebus.Routing(r => r.TypeBased().MapFallback(Endpoint)));

        _activator.Register(GetHandler);

        _consumer = new RebusConsumer(_activator, Endpoint, Configuration);
        _consumer.Start();
    }

    protected ITestBusEndpoint Sender { get; }

    protected abstract IHandleMessages<JObject> GetHandler();

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        _consumer.Dispose();
        _activator.Dispose();
    }
}
