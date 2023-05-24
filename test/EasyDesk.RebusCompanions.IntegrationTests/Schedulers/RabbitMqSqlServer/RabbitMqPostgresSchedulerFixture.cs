using Rebus.Config;
using Rebus.Timeouts;
using Rebus.Transport;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;

namespace EasyDesk.RebusCompanions.IntegrationTests.Schedulers.RabbitMqPostgres;

public class RabbitMqSqlServerSchedulerFixture : RebusSchedulerFixture
{
    private readonly RabbitMqContainer _rabbitMqContainer;
    private readonly MsSqlContainer _sqlServerContainer;

    public RabbitMqSqlServerSchedulerFixture()
    {
        _rabbitMqContainer = new RabbitMqBuilder()
            .WithUsername("admin")
            .WithPassword("admin")
            .Build();

        _sqlServerContainer = new MsSqlBuilder()
            .WithPassword("admin.123")
            .Build();
    }

    protected override void ConfigureTransport(StandardConfigurer<ITransport> transport, string endpoint) =>
        transport.UseRabbitMq(_rabbitMqContainer.GetConnectionString(), endpoint);

    protected override void ConfigureTimeouts(StandardConfigurer<ITimeoutManager> timeouts) =>
        timeouts.StoreInSqlServer(_sqlServerContainer.GetConnectionString(), "timeouts");

    protected override async Task Start()
    {
        await _rabbitMqContainer.StartAsync();
        await _sqlServerContainer.StartAsync();
    }

    protected override async Task Stop()
    {
        await _rabbitMqContainer.DisposeAsync();
        await _sqlServerContainer.DisposeAsync();
    }
}
