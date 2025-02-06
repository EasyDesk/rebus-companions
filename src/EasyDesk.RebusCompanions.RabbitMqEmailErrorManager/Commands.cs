using EasyDesk.Commons;
using EasyDesk.Commons.Options;
using Mono.Options;
using Rebus.Handlers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EasyDesk.RebusCompanions.RabbitMqEmailErrorManager;

public static class Commands
{
    public record SmokeTest(string Message);

    public static async Task ExecuteCommand(this IHost host, IEnumerable<string> args, Action or)
    {
        Option<SmokeTest> doSmokeTest = NoneOption.Value;
        var options = new OptionSet()
        {
            { "smoke:", arg => doSmokeTest = new SmokeTest(Message: arg.AsOption().Filter(arg => !string.IsNullOrWhiteSpace(arg)) | "Hello World!").AsSome() },
        };
        var extra = options.Parse(args);
        if (doSmokeTest)
        {
            await using (var scope = host.Services.CreateAsyncScope())
            {
                var json = JsonSerializer.SerializeToNode(doSmokeTest.Value) ?? throw new InvalidOperationException($"{doSmokeTest.Value.Message} can't be serialized.");

                var errorHandler = scope.ServiceProvider.GetRequiredService<IHandleMessages<JsonNode>>();
                await errorHandler.Handle(json);
            }
        }
        else
        {
            or();
        }
    }
}
