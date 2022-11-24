using EasyDesk.CleanArchitecture.Application.Cqrs.Commands;
using EasyDesk.CleanArchitecture.Application.Messaging;
using EasyDesk.CleanArchitecture.Testing.Integration.Rebus;
using NodaTime;

namespace EasyDesk.RebusScheduler.IntegrationTests;

public class RabbitMqPostgresTests : IClassFixture<RebusSchedulerFixture>
{
    private record Command(int Value) : IMessage, ICommand;

    private const string SenderAddress = "sender";
    private const string ReceiverAddress = "receiver";

    private readonly RebusTestHelper _sender;
    private readonly RebusTestHelper _receiver;

    public RabbitMqPostgresTests(RebusSchedulerFixture schedulerFixture)
    {
        _sender = schedulerFixture.CreateBus(SenderAddress, ReceiverAddress);
        _receiver = schedulerFixture.CreateBus(ReceiverAddress, SenderAddress);
    }

    [Fact]
    public async Task ShouldDeliverMessagesAfterTheGivenDelay()
    {
        var command = new Command(1);
        var delay = Duration.FromSeconds(5);

        await _sender.Defer(command, delay);

        await _receiver.WaitForMessageAfterDelayOrFail(command, delay);
    }
}
