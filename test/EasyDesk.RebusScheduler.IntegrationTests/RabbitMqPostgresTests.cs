using EasyDesk.CleanArchitecture.Application.Cqrs.Async;
using EasyDesk.CleanArchitecture.Testing.Integration.Bus;
using NodaTime;

namespace EasyDesk.RebusScheduler.IntegrationTests;

public class RabbitMqPostgresTests : IClassFixture<RebusSchedulerFixture>
{
    private record Command(int Value) : IMessage, ICommand;

    private const string SenderAddress = "sender";
    private const string ReceiverAddress = "receiver";

    private readonly ITestBus _sender;
    private readonly ITestBus _receiver;

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
