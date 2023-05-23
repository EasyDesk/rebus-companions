using EasyDesk.RebusCompanion.Core.Config;
using Newtonsoft.Json.Linq;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Pipeline;

namespace EasyDesk.RebusCompanion.Core.ErrorsManager;

public class RebusErrorsManager : RebusProcess
{
    public RebusErrorsManager(
        string endpoint,
        RebusConfiguration defaultConfiguration,
        Func<IMessageContext, IHandleMessages<JObject>> handler) : base(
            new BuiltinHandlerActivator().Register(handler),
            endpoint,
            defaultConfiguration)
    {
    }

    protected override void ConfigureRebusBus(RebusConfigurer configurer)
    {
    }
}
