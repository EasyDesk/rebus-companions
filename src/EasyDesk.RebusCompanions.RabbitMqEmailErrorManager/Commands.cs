﻿using EasyDesk.Commons;
using Mono.Options;
using Newtonsoft.Json.Linq;
using Rebus.Handlers;

namespace EasyDesk.RebusCompanions.RabbitMqEmailErrorManager;

public static class Commands
{
    public record SmokeTest(string Message);

    public static async Task ExecuteCommand(this IHost host, IEnumerable<string> args, Action or)
    {
        Option<SmokeTest> doSmokeTest = NoneOption.Value;
        var options = new OptionSet()
        {
            { "smoke", arg => doSmokeTest = new SmokeTest(Message: arg.AsSome().Filter(arg => !string.IsNullOrWhiteSpace(arg)) | "Hello World!").AsSome() }
        };
        var extra = options.Parse(args);
        if (doSmokeTest)
        {
            await using (var scope = host.Services.CreateAsyncScope())
            {
                var json = JObject.FromObject(doSmokeTest.Value) ?? throw new InvalidOperationException($"{doSmokeTest.Value.Message} can't be serialized.");

                var errorHandler = scope.ServiceProvider.GetRequiredService<IHandleMessages<JObject>>();
                await errorHandler.Handle(json);
            }
        }
        else
        {
            or();
        }
    }
}
