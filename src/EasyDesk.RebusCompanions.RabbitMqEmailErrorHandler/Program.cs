using EasyDesk.Commons;
using EasyDesk.Extensions.Configuration;
using EasyDesk.RebusCompanions.Core.Config;
using EasyDesk.RebusCompanions.Core.Consumer;
using EasyDesk.RebusCompanions.Email;
using FluentEmail.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Retry.Simple;
using Rebus.ServiceProvider;

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var rabbitMqConnection = configuration.RequireValue<string>("RabbitMqConnection");
var endpoint = configuration.GetValueAsOption<string>("RebusEndpoint").OrElse(SimpleRetryStrategySettings.DefaultErrorQueueName);

var rebusConfig = new RebusConfiguration()
    .WithTransport((t, e) => t.UseRabbitMq(rabbitMqConnection, e));

var emailSection = configuration.RequireSection("Email");
var recipients = emailSection.RequireValue<IEnumerable<string>>("Recipients");

var services = new ServiceCollection()
    .AddEmailErrorHandler(emailSection, (email, message, context) =>
    {
        email.To(recipients.Select(x => new Address(x)));
    })
    .BuildServiceProvider();

var activator = new DependencyInjectionHandlerActivator(services);
var consumer = new RebusConsumer(activator, endpoint, rebusConfig);

consumer.Start();
