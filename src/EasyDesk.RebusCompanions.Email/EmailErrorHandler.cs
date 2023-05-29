using EasyDesk.Commons.Collections;
using FluentEmail.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rebus.Handlers;
using Rebus.Pipeline;

namespace EasyDesk.RebusCompanions.Email;

public class EmailErrorHandler : IHandleMessages<JObject>
{
    private readonly IFluentEmailFactory _fluentEmailFactory;
    private readonly Action<IFluentEmail, JObject, IMessageContext> _configure;

    public EmailErrorHandler(IFluentEmailFactory fluentEmailFactory, Action<IFluentEmail, JObject, IMessageContext> configure)
    {
        _fluentEmailFactory = fluentEmailFactory;
        _configure = configure;
    }

    public async Task Handle(JObject message)
    {
        var messageContext = MessageContext.Current;

        var model = new
        {
            MessageJson = message.ToString(Formatting.Indented),
            MessageHeaders = messageContext.Headers
        };

        var bodyTemplate = $$"""
            A message was delivered to the error queue:
            <br>

            Body:
            <pre>
            @Model.MessageJson
            </pre>

            Headers:
            @foreach (var item in Model.MessageHeaders)
            {
                <b>@item.Key</b>:
                <pre>
                    @item.Value
                </pre>
                <br>
            }
            """;

        var email = _fluentEmailFactory
            .Create()
            .UsingTemplate(bodyTemplate, )
            .Body(body, isHtml: true)
            .Subject("Message delivered to error queue");
        _configure(email, message, messageContext);

        var response = await email.SendAsync();
        if (!response.Successful)
        {
            throw new Exception($"Unable to send email:\n{response.ErrorMessages.ConcatStrings("\n")}");
        }
    }
}
