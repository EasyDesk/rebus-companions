using EasyDesk.Commons;
using EasyDesk.Extensions.Configuration;
using FluentEmail.Core;
using FluentEmail.MailKitSmtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Rebus.Handlers;
using Rebus.Pipeline;

namespace EasyDesk.RebusCompanions.Email;

public static class EmailErrorHandlerExtensions
{
    public static IServiceCollection AddEmailErrorHandler(
        this IServiceCollection services,
        IConfigurationSection emailConfigSection,
        Action<IFluentEmail, JObject, IMessageContext> configureEmail)
    {
        var smtpClientOptions = new SmtpClientOptions();
        emailConfigSection.Bind(smtpClientOptions);

        var defaultFromEmail = emailConfigSection.GetValueAsOption<string>("DefaultFromEmail").OrElseNull();
        var defaultFromName = emailConfigSection.GetValueAsOption<string>("DefaultFromName").OrElse(string.Empty);

        services
            .AddFluentEmail(defaultFromEmail, defaultFromName)
            .AddMailKitSender(smtpClientOptions);

        services.AddTransient<IHandleMessages<JObject>>(p => new EmailErrorHandler(
            p.GetRequiredService<IFluentEmailFactory>(),
            configureEmail));

        return services;
    }
}
