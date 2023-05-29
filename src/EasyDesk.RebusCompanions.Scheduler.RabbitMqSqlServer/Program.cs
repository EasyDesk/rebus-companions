using EasyDesk.Commons;
using EasyDesk.Extensions.Configuration;
using EasyDesk.RebusCompanions.Core.Config;
using EasyDesk.RebusCompanions.Core.HostedService;
using EasyDesk.RebusCompanions.Core.Scheduler;
using EasyDesk.RebusCompanions.EfCore.Initializer;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Rebus.Config;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.AddSingleton<IClock>(SystemClock.Instance);

        var rabbitMqConnection = configuration.RequireValue<string>("RabbitMqConnection");
        services.AddSingleton(sp => new RebusConfiguration(sp)
            .WithTransport((t, e) => t.UseRabbitMq(rabbitMqConnection, e))
            .WithLogging(l => l.MicrosoftExtensionsLogging(sp.GetRequiredService<ILoggerFactory>())));

        var endpoint = configuration.GetValueAsOption<string>("RebusEndpoint").OrElse(RebusScheduler.DefaultEndpoint);
        var tableName = configuration.GetValueAsOption<string>("TableName").OrElse("Timeouts");
        var dbConnection = configuration.RequireValue<string>("SqlServerConnection");
        services.AddSingleton(sp => new RebusScheduler(
            sp.GetRequiredService<RebusConfiguration>(),
            t => t.StoreInSqlServer(dbConnection, tableName),
            endpoint));

        services.InitializeDb(options => options.UseSqlServer(dbConnection));

        services.AddRebusProcess<RebusScheduler>();
    })
    .Build();

host.Run();
