using EasyDesk.CleanArchitecture.Infrastructure.Configuration;
using EasyDesk.Commons;
using EasyDesk.RebusCompanions.Core.Config;
using EasyDesk.RebusCompanions.Core.Scheduler;
using Microsoft.Extensions.Configuration;
using Rebus.Config;

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var postgresConnection = configuration.RequireValue<string>("SqlServerConnection");
var rabbitMqConnection = configuration.RequireValue<string>("RabbitMqConnection");
var endpoint = configuration.GetValueAsOption<string>("RebusEndpoint").OrElse(RebusScheduler.DefaultEndpoint);
var tableName = configuration.GetValueAsOption<string>("TableName").OrElse("timeouts");

var rebusConfig = new RebusConfiguration()
    .WithTransport((t, e) => t.UseRabbitMq(rabbitMqConnection, e));

var scheduler = new RebusScheduler(
    rebusConfig,
    t => t.StoreInSqlServer(postgresConnection, tableName),
    endpoint);

scheduler.Start();
