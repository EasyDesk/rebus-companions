using EasyDesk.Commons.Collections;
using FluentEmail.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rebus.Handlers;
using Rebus.Pipeline;
using System.Web;

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

        var body = $"""
            A message was delivered to the error queue:

            Body:
            <pre>
            {HttpUtility.HtmlEncode(message.ToString(Formatting.Indented))}
            </pre>

            Headers:
            <pre>
            {HttpUtility.HtmlEncode(messageContext.Headers.Select(x => $"<b>{x.Key}</b>: {x.Value}").ConcatStrings("\n"))}
            </pre>
            """;

        var email = _fluentEmailFactory
            .Create()
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
