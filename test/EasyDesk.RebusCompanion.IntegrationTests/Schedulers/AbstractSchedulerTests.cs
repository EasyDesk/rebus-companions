using EasyDesk.CleanArchitecture.Application.Cqrs.Async;
using EasyDesk.CleanArchitecture.Testing.Integration.Bus;
using NodaTime;

namespace EasyDesk.RebusCompanion.IntegrationTests.Schedulers;

public abstract class AbstractSchedulerTests<T> : IClassFixture<T>
    where T : RebusSchedulerFixture
{
    private record Command(int Value) : ICommand;

    private const string SenderAddress = "sender";
    private const string ReceiverAddress = "receiver";

    private readonly ITestBus _sender;
    private readonly ITestBus _receiver;

    public AbstractSchedulerTests(T fixture)
    {
        _sender = fixture.CreateBus(SenderAddress, ReceiverAddress);
        _receiver = fixture.CreateBus(ReceiverAddress, SenderAddress);
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
