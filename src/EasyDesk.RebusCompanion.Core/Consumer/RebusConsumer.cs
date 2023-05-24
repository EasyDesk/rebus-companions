using EasyDesk.RebusCompanion.Core.Config;
using Rebus.Activation;
using Rebus.Config;

namespace EasyDesk.RebusCompanion.Core.Consumer;

public class RebusConsumer : RebusProcess
{
    public RebusConsumer(
        IHandlerActivator handlerActivator,
        string endpoint,
        RebusConfiguration defaultConfiguration) : base(
            handlerActivator,
            endpoint,
            defaultConfiguration)
    {
    }

    protected override void ConfigureRebusBus(RebusConfigurer configurer)
    {
    }
}
