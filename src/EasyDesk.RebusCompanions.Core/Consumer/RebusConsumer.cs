﻿using EasyDesk.RebusCompanions.Core.Config;
using EasyDesk.RebusCompanions.Core.Json;
using Rebus.Activation;
using Rebus.Config;

namespace EasyDesk.RebusCompanions.Core.Consumer;

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
        configurer
            .Serialization(s => s.AlwaysDeserializeAnyObject());
    }
}
