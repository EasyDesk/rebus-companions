using EasyDesk.CleanArchitecture.Infrastructure.Configuration;
using EasyDesk.Commons;
using FluentEmail.Core;
using FluentEmail.MailKitSmtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Rebus.Handlers;
using Rebus.Pipeline;

namespace EasyDesk.RebusCompanion.Email;

public static class EmailErrorHandlerExtensions
{
    public static IServiceCollection AddEmailErrorHandler(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IFluentEmail, JObject, IMessageContext> configureEmail)
    {
        var emailConfigSection = configuration.RequireSection("Email");
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
