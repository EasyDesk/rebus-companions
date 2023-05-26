using EasyDesk.Commons;
using EasyDesk.Extensions.Configuration;
using EasyDesk.RebusCompanions.Core.Config;
using EasyDesk.RebusCompanions.Core.Scheduler;
using Microsoft.Extensions.Configuration;
using Rebus.Config;

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var dbConnection = configuration.RequireValue<string>("PostgresConnection");
var rabbitMqConnection = configuration.RequireValue<string>("RabbitMqConnection");
var endpoint = configuration.GetValueAsOption<string>("RebusEndpoint").OrElse(RebusScheduler.DefaultEndpoint);
var tableName = configuration.GetValueAsOption<string>("TableName").OrElse("Timeouts");

var rebusConfig = new RebusConfiguration()
    .WithTransport((t, e) => t.UseRabbitMq(rabbitMqConnection, e));

var scheduler = new RebusScheduler(
    rebusConfig,
    t => t.StoreInPostgres(dbConnection, tableName),
    endpoint);

scheduler.Start();
