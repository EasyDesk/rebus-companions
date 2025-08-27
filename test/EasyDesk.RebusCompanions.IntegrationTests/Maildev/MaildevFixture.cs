using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using EasyDesk.CleanArchitecture.Application.Json;

namespace EasyDesk.RebusCompanions.IntegrationTests.Maildev;

public sealed class MaildevFixture : IAsyncLifetime
{
    public const string User = "maildev-user";
    public const string Password = "maildev.123";
    private readonly IContainer _maildevContainer;

    public MaildevClient Client { get; private set; } = default!;

    public int Port { get; private set; }

    public MaildevFixture()
    {
        _maildevContainer = new ContainerBuilder()
            .WithHostname("maildev")
            .WithImage("maildev/maildev:2.1.0")
            .WithEnvironment(new Dictionary<string, string>()
            {
                ["MAILDEV_IP"] = "::",
                ["MAILDEV_INCOMING_USER"] = User,
                ["MAILDEV_INCOMING_PASS"] = Password,
            })
            .WithPortBinding(1025, true)
            .WithPortBinding(1080, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy())
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        await _maildevContainer.StartAsync();
        Client = new MaildevClient(new UriBuilder("http", "localhost", _maildevContainer.GetMappedPublicPort(1080)).Uri, c => JsonDefaults.ApplyDefaultConfiguration(c));
        Port = _maildevContainer.GetMappedPublicPort(1025);
    }

    public async ValueTask DisposeAsync()
    {
        await _maildevContainer.StopAsync();
        await _maildevContainer.DisposeAsync();
    }

    public async Task Reset()
    {
        await Client.DeleteAllEmails();
    }
}
