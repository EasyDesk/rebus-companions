using EasyDesk.CleanArchitecture.Application.Cqrs.Async;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Rebus.Handlers;

namespace EasyDesk.RebusCompanions.IntegrationTests.Consumers;

public sealed class SimpleConsumerTests : AbstractConsumerTests
{
    private readonly IHandleMessages<JObject> _handler;

    private record Command(int Value) : ICommand;

    public SimpleConsumerTests()
    {
        _handler = Substitute.For<IHandleMessages<JObject>>();
    }

    protected override IHandleMessages<JObject> GetHandler() => _handler;

    [Fact]
    public async Task ShouldReceiveMessages()
    {
        var command = new Command(1);
        await Sender.Send(command);
        await Task.Delay(500);

        await _handler.Received(1).Handle(Arg.Is<JObject>(o => o.ToObject<Command>() == command));
    }
}
