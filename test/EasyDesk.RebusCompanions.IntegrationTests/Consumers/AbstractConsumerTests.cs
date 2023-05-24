using EasyDesk.CleanArchitecture.Application.Cqrs.Async;
using EasyDesk.CleanArchitecture.Testing.Integration.Bus.Rebus;
using NSubstitute;

namespace EasyDesk.RebusCompanions.IntegrationTests.Consumers;

public abstract class AbstractConsumerTests<T> : IClassFixture<T>
    where T : RebusConsumerFixture
{
    private record Command(int Value) : ICommand;

    private readonly T _fixture;

    private readonly RebusTestBus _sender;

    public AbstractConsumerTests(T fixture)
    {
        _fixture = fixture;
        _sender = fixture.CreateBus("sender");
    }

    [Fact]
    public async Task ShouldReceiveMessages()
    {
        var command = new Command(1);
        await _sender.Send(command);
        await Task.Delay(500);

        await _fixture.Handler.Received(1).Handle(command);
    }
}
