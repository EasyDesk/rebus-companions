using EasyDesk.Commons;
using EasyDesk.Extensions.Configuration;
using EasyDesk.RebusCompanions.Core.Config;
using EasyDesk.RebusCompanions.Core.Scheduler;
using Microsoft.Extensions.Configuration;
using Rebus.Config;

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

////var dbConnection = configuration.RequireValue<string>("SqlServerConnection");
////var rabbitMqConnection = configuration.RequireValue<string>("RabbitMqConnection");
var dbConnection = "Server=localhost;Initial Catalog=SchedulerDB;User Id=SA;TrustServerCertificate=True;Password=admin.123;";
var rabbitMqConnection = "amqp://admin:admin-123@localhost:4002";
var endpoint = configuration.GetValueAsOption<string>("RebusEndpoint").OrElse(RebusScheduler.DefaultEndpoint);
var tableName = configuration.GetValueAsOption<string>("TableName").OrElse("Timeouts");

var rebusConfig = new RebusConfiguration()
    .WithTransport((t, e) => t.UseRabbitMq(rabbitMqConnection, e));

using var scheduler = new RebusScheduler(
    rebusConfig,
    t => t.StoreInSqlServer(dbConnection, tableName),
    endpoint);

scheduler.Start();
