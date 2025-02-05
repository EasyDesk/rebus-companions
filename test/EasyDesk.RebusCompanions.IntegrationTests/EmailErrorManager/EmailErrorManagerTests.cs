using EasyDesk.CleanArchitecture.Application.Cqrs.Async;
using EasyDesk.CleanArchitecture.Testing.Integration.Polling;
using EasyDesk.RebusCompanions.Email;
using EasyDesk.RebusCompanions.IntegrationTests.Consumers;
using EasyDesk.RebusCompanions.IntegrationTests.Maildev;
using MimeKit;
using Newtonsoft.Json.Linq;
using Rebus.Handlers;
using static EasyDesk.Commons.StaticImports;

namespace EasyDesk.RebusCompanions.IntegrationTests.EmailErrorManager;

public class EmailErrorManagerTests : AbstractConsumerTests, IClassFixture<MaildevFixture>, IAsyncLifetime
{
    public record Command(int Value, string Text) : ICommand;

    private readonly MaildevFixture _maildev;

    public EmailErrorManagerTests(MaildevFixture maildev)
    {
        _maildev = maildev;
    }

    protected override IHandleMessages<JObject> GetHandler() => EmailErrorHandlerExtensions.CreateEmailErrorHandler(
        Clock,
        new EmailErrorHandlerSettings
        {
            Host = "localhost",
            Port = _maildev.Port,
            From = new MailboxAddress("Test address", "error-manager@test.com"),
            To =
            [
                new MailboxAddress(null, "dev@test.com"),
            ],
            Credentials = Some(new EmailErrorHandlerCredentials
            {
                User = MaildevFixture.User,
                Password = MaildevFixture.Password,
            }),
            UseSsl = false,
        },
        EmailErrorHandler.DefaultBodyTemplate);

    [Fact]
    public async Task ShouldSendAnEmailWithMessageInformation()
    {
        await Sender.Send(new Command(1, "Hello world"));

        var emails = await Poll
            .Async(_ => _maildev.Client.GetEmails())
            .Until(emails => emails.Any());

        await Verify(emails).ScrubInlineGuids();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _maildev.Reset();
}
