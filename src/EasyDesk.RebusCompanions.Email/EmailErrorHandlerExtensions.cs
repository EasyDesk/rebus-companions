using EasyDesk.Commons;
using EasyDesk.Extensions.Configuration;
using FluentEmail.Core;
using FluentEmail.MailKitSmtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using NodaTime;
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

        var defaultFromEmail = emailConfigSection.GetValueAsOption<string>("DefaultFromEmail").OrElse(smtpClientOptions.User);
        var defaultFromName = emailConfigSection.GetValueAsOption<string>("DefaultFromName").OrElse(string.Empty);

        services
            .AddFluentEmail(defaultFromEmail, defaultFromName)
            .AddMailKitSender(smtpClientOptions)
            .AddLiquidRenderer();

        services.AddTransient<IHandleMessages<JObject>>(p => new EmailErrorHandler(
            p.GetRequiredService<IFluentEmailFactory>(),
            p.GetRequiredService<IClock>(),
            configureEmail));

        return services;
    }
}
