using EasyDesk.Commons;
using EasyDesk.Extensions.Configuration;
using EasyDesk.RebusCompanions.Core.Config;
using EasyDesk.RebusCompanions.Core.Consumer;
using EasyDesk.RebusCompanions.Core.HostedService;
using EasyDesk.RebusCompanions.Email;
using FluentEmail.Core.Models;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Retry.Simple;
using Rebus.ServiceProvider;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        var emailSection = configuration.RequireSection("Email");
        var recipients = emailSection.RequireValue<IEnumerable<string>>("Recipients");
        services.AddEmailErrorHandler(emailSection, (email, message, context) =>
        {
            email.To(recipients.Select(x => new Address(x)));
        });

        services.AddSingleton<IHandlerActivator>(sp => new DependencyInjectionHandlerActivator(sp));

        var rabbitMqConnection = configuration.RequireValue<string>("RabbitMqConnection");
        services.AddSingleton(sp => new RebusConfiguration()
            .WithTransport((t, e) => t.UseRabbitMq(rabbitMqConnection, e))
            .WithLogging(l => l.MicrosoftExtensionsLogging(sp.GetRequiredService<ILoggerFactory>())));

        var endpoint = configuration
            .GetValueAsOption<string>("RebusEndpoint")
            .OrElse(SimpleRetryStrategySettings.DefaultErrorQueueName);

        services.AddSingleton(sp => new RebusConsumer(
            sp.GetRequiredService<IHandlerActivator>(),
            endpoint,
            sp.GetRequiredService<RebusConfiguration>()));

        services.AddRebusProcess<RebusConsumer>();
    })
    .Build();

host.Run();
