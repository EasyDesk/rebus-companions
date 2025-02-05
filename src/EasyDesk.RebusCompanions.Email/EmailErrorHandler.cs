using EasyDesk.Commons;
using EasyDesk.Commons.Options;
using Fluid;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;
using Rebus.Handlers;
using Rebus.Pipeline;
using System.Collections.Immutable;
using System.Text.Encodings.Web;

namespace EasyDesk.RebusCompanions.Email;

public record EmailErrorHandlerSettings
{
    public required string Host { get; init; }

    public required int Port { get; init; }

    public required bool UseSsl { get; init; }

    public required Option<EmailErrorHandlerCredentials> Credentials { get; init; }

    public required InternetAddress From { get; init; }

    public required IImmutableSet<InternetAddress> To { get; init; }
}

public record EmailErrorHandlerCredentials
{
    public required string User { get; init; }

    public required string Password { get; init; }
}

public class EmailErrorHandler : IHandleMessages<JObject>
{
    public const string DefaultBodyTemplate = """
        <h1>Message delivered to error queue</h1>
        A message was delivered to the error queue at timestamp {{ Instant }}.
        
        <section>
            <h2>Body</h2>
            <pre>
        {{ MessageJson }}
            </pre>
        </section>
        
        <section>
            <h2>Headers</h2>
            {% for item in MessageHeaders %}
                <b>{{ item.Key }}</b>:
                <pre>{{ item.Value }}</pre>
            {% endfor %}
        </section>
        """;

    private readonly IClock _clock;
    private readonly IFluidTemplate _template;
    private readonly EmailErrorHandlerSettings _settings;

    public EmailErrorHandler(
        IClock clock,
        EmailErrorHandlerSettings settings,
        IFluidTemplate template)
    {
        _clock = clock;
        _template = template;
        _settings = settings;
    }

    public async Task Handle(JObject message)
    {
        using var email = await CreateMimeMessage(message, MessageContext.Current);
        await SendEmail(email);
    }

    private async Task<MimeMessage> CreateMimeMessage(JObject message, IMessageContext? messageContext)
    {
        var bodyBuilder = new BodyBuilder()
        {
            HtmlBody = await GenerateBodyHtml(message, messageContext),
        };

        var email = new MimeMessage();
        email.From.Add(_settings.From);
        email.To.AddRange(_settings.To);
        email.Subject = "1 message delivered to error queue";
        email.Body = bodyBuilder.ToMessageBody();
        return email;
    }

    private async ValueTask<string> GenerateBodyHtml(JObject message, IMessageContext? messageContext)
    {
        var model = new
        {
            MessageJson = message.ToString(Formatting.Indented),
            MessageHeaders = (messageContext
                ?.Headers
                ?.OrderBy(x => x.Key) ?? Enumerable.Empty<KeyValuePair<string, string>>())
                .Select(x => new { x.Key, x.Value })
                .ToList(),
            Instant = _clock.GetCurrentInstant(),
        };

        var templateOptions = new TemplateOptions()
        {
            MemberAccessStrategy = UnsafeMemberAccessStrategy.Instance,
        };
        var templateContext = new TemplateContext(model, templateOptions);
        return await _template.RenderAsync(templateContext, HtmlEncoder.Default);
    }

    private async Task SendEmail(MimeMessage email)
    {
        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, _settings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.None);
        await _settings.Credentials.IfPresentAsync(c => client.AuthenticateAsync(c.User, c.Password));
        await client.SendAsync(email);
        await client.DisconnectAsync(true);
    }
}
