using EasyDesk.RebusScheduler.Core;
using Microsoft.Extensions.Configuration;
using Rebus.Config;

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var postgresConnection = configuration.GetRequiredSection("PostgresConnection").Get<string>();
var rabbitMqConnection = configuration.GetRequiredSection("RabbitMqConnection").Get<string>();
var endpoint = configuration.GetRequiredSection("RebusEndpoint").Get<string>();

var rebusConfig = new RebusConfiguration()
    .WithTransport((t, e) => t.UseRabbitMq(rabbitMqConnection, e));

var scheduler = new Scheduler(
    endpoint,
    rebusConfig,
    t => t.StoreInPostgres(postgresConnection, "timeouts"));

scheduler.Start();
