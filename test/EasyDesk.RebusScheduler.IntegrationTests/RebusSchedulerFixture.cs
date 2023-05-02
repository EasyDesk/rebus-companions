using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using EasyDesk.CleanArchitecture.Testing.Integration.Bus.Rebus;
using EasyDesk.RebusScheduler.Core;
using NodaTime;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Timeouts;

namespace EasyDesk.RebusScheduler.IntegrationTests;

public class RebusSchedulerFixture : IAsyncLifetime
{
    public const string Endpoint = "test-timeout-manager";

    private readonly RabbitMqTestcontainer _rabbitMqContainer;

    private readonly PostgreSqlTestcontainer _postgresContainer;

    private readonly RebusConfiguration _defaultConfiguration;
    private readonly TimeoutManagerConfiguration _timeoutManager;
    private readonly Scheduler _scheduler;

    public RebusSchedulerFixture()
    {
        _rabbitMqContainer = new TestcontainersBuilder<RabbitMqTestcontainer>()
            .WithMessageBroker(new RabbitMqTestcontainerConfiguration
            {
                Username = "admin",
                Password = "admin",
            })
            .Build();

        _postgresContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Username = "admin",
                Password = "admin",
                Database = "timeoutstests",
            })
            .Build();

        _defaultConfiguration = new RebusConfiguration()
            .WithTransport((t, e) => t.UseRabbitMq(_rabbitMqContainer.ConnectionString, e));
        _timeoutManager = t => t.StoreInPostgres(_postgresContainer.ConnectionString, "timeouts");
        _scheduler = new Scheduler(Endpoint, _defaultConfiguration, _timeoutManager);
    }

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
        await _rabbitMqContainer.StartAsync();
        await _postgresContainer.StartAsync();
        _scheduler.Start();
    }

    public async Task DisposeAsync()
    {
        _scheduler.Dispose();
        await _rabbitMqContainer.DisposeAsync();
        await _postgresContainer.DisposeAsync();
    }
}
