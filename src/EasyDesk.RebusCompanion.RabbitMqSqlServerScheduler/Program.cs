﻿using EasyDesk.RebusCompanion.Core.Config;
using EasyDesk.RebusCompanion.Core.Scheduler;
using Microsoft.Extensions.Configuration;
using Rebus.Config;

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var postgresConnection = configuration.GetRequiredSection("SqlServerConnection").Get<string>();
var rabbitMqConnection = configuration.GetRequiredSection("RabbitMqConnection").Get<string>();
var endpoint = configuration.GetRequiredSection("RebusEndpoint").Get<string>();

var rebusConfig = new RebusConfiguration()
    .WithTransport((t, e) => t.UseRabbitMq(rabbitMqConnection, e));

var scheduler = new RebusScheduler(
    endpoint,
    rebusConfig,
    t => t.StoreInSqlServer(postgresConnection, "timeouts"));

scheduler.Start();
