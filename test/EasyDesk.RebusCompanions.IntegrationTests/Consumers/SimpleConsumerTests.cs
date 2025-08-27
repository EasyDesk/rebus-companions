using EasyDesk.CleanArchitecture.Application.Cqrs.Async;
using NSubstitute;
using Rebus.Handlers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EasyDesk.RebusCompanions.IntegrationTests.Consumers;

public sealed class SimpleConsumerTests : AbstractConsumerTests
{
    private readonly IHandleMessages<JsonNode> _handler;

    private record Command(int Value) : ICommand;

    public SimpleConsumerTests()
    {
        _handler = Substitute.For<IHandleMessages<JsonNode>>();
    }

    protected override IHandleMessages<JsonNode> GetHandler() => _handler;

    [Fact]
    public async Task ShouldReceiveMessages()
    {
        var command = new Command(1);
        await Sender.Send(command);
        await Task.Delay(500, TestContext.Current.CancellationToken);

        var serializerOptions = new JsonSerializerOptions();
        await _handler.Received(1).Handle(Arg.Is<JsonNode>(o => o.Deserialize<Command>(serializerOptions) == command));
    }
}
