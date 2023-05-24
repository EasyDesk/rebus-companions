using Rebus.Config;
using Rebus.Timeouts;
using Rebus.Transport;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace EasyDesk.RebusCompanions.IntegrationTests.Schedulers.RabbitMqPostgres;

public class RabbitMqPostgresSchedulerFixture : RebusSchedulerFixture
{
    private readonly RabbitMqContainer _rabbitMqContainer;
    private readonly PostgreSqlContainer _postgresContainer;

    public RabbitMqPostgresSchedulerFixture()
    {
        _rabbitMqContainer = new RabbitMqBuilder()
            .WithUsername("admin")
            .WithPassword("admin")
            .Build();

        _postgresContainer = new PostgreSqlBuilder()
            .WithUsername("admin")
            .WithPassword("admin")
            .WithDatabase("timeoutstests")
            .Build();
    }

    protected override void ConfigureTransport(StandardConfigurer<ITransport> transport, string endpoint) =>
        transport.UseRabbitMq(_rabbitMqContainer.GetConnectionString(), endpoint);

    protected override void ConfigureTimeouts(StandardConfigurer<ITimeoutManager> timeouts) =>
        timeouts.StoreInPostgres(_postgresContainer.GetConnectionString(), "timeouts");

    protected override async Task Start()
    {
        await _rabbitMqContainer.StartAsync();
        await _postgresContainer.StartAsync();
    }

    protected override async Task Stop()
    {
        await _rabbitMqContainer.DisposeAsync();
        await _postgresContainer.DisposeAsync();
    }
}
