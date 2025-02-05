using EasyDesk.Commons;
using EasyDesk.Extensions.Configuration;
using Fluid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using Newtonsoft.Json.Linq;
using NodaTime;
using Rebus.Handlers;
using System.Collections.Immutable;

namespace EasyDesk.RebusCompanions.Email;

public static class EmailErrorHandlerExtensions
{
    public static IServiceCollection AddEmailErrorHandler(
        this IServiceCollection services,
        IConfigurationSection emailConfigSection)
    {
        var credentialsSection = emailConfigSection.GetSectionAsOption("Credentials");
        var fromSection = emailConfigSection.RequireSection("From");

        var settings = new EmailErrorHandlerSettings
        {
            Host = emailConfigSection.RequireValue<string>("Host"),
            Port = emailConfigSection.RequireValue<int>("Port"),
            UseSsl = emailConfigSection.GetValueAsOption<bool>("UseSsl").OrElse(true),
            Credentials = credentialsSection
                .Map(s => new EmailErrorHandlerCredentials
                {
                    User = s.RequireValue<string>("User"),
                    Password = s.RequireValue<string>("Password"),
                })
                .Filter(cred => !(string.IsNullOrEmpty(cred.User) && string.IsNullOrEmpty(cred.Password))),
            From = new MailboxAddress(
                name: fromSection.GetValueAsOption<string>("Name").OrElseNull(),
                address: fromSection.RequireValue<string>("Address")),
            To = emailConfigSection
                .RequireValue<IEnumerable<string>>("To")
                .Select(x => new MailboxAddress(null, x))
                .Cast<InternetAddress>()
                .ToImmutableHashSet(),
        };

        services.AddSingleton(settings);

        var rawTemplate = emailConfigSection
            .GetValueAsOption<string>("RawBodyTemplate")
            .OrElse(EmailErrorHandler.DefaultBodyTemplate);

        services.AddSingleton<IHandleMessages<JObject>>(sp => CreateEmailErrorHandler(
            sp.GetRequiredService<IClock>(),
            sp.GetRequiredService<EmailErrorHandlerSettings>(),
            rawTemplate));

        return services;
    }

    public static EmailErrorHandler CreateEmailErrorHandler(IClock clock, EmailErrorHandlerSettings settings, string? rawTemplate = null)
    {
        var parser = new FluidParser();
        var template = parser.Parse(rawTemplate ?? EmailErrorHandler.DefaultBodyTemplate);

        return new EmailErrorHandler(clock, settings, template);
    }
}
