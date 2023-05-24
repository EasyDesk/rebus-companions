using Rebus.Config;
using Rebus.Transport;
using Testcontainers.RabbitMq;

namespace EasyDesk.RebusCompanion.IntegrationTests.Consumers.RabbitMq;

public class RabbitMqConsumerFixture : RebusConsumerFixture
{
    private readonly RabbitMqContainer _rabbitMqContainer;

    public RabbitMqConsumerFixture()
    {
        _rabbitMqContainer = new RabbitMqBuilder()
            .WithUsername("admin")
            .WithPassword("admin")
            .Build();
    }

    protected override void ConfigureTransport(StandardConfigurer<ITransport> transport, string endpoint) =>
        transport.UseRabbitMq(_rabbitMqContainer.GetConnectionString(), endpoint);

    protected override Task Start() => _rabbitMqContainer.StartAsync();

    protected override Task Stop() => _rabbitMqContainer.StopAsync();
}
