using EasyDesk.Commons;
using EasyDesk.Commons.Options;
using EasyDesk.RebusCompanions.Core.Config;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using static EasyDesk.Commons.StaticImports;

namespace EasyDesk.RebusCompanions.Core;

public abstract class RebusProcess : IDisposable
{
    private Option<IBus> _bus;
    private readonly IHandlerActivator _activator;
    private readonly string _endpoint;
    private readonly RebusConfiguration _defaultConfiguration;

    public RebusProcess(IHandlerActivator activator, string endpoint, RebusConfiguration defaultConfiguration)
    {
        _activator = activator;
        _endpoint = endpoint;
        _defaultConfiguration = defaultConfiguration;
    }

    public void Start()
    {
        if (_bus.IsPresent)
        {
            throw new InvalidOperationException("Bus was already started");
        }

        var configurer = Configure.With(_activator);
        _defaultConfiguration.Apply(configurer, _endpoint);
        ConfigureRebusBus(configurer);
        _bus = Some(configurer.Start());
    }

    protected abstract void ConfigureRebusBus(RebusConfigurer configurer);

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _bus.IfPresent(bus => bus.Dispose());
    }
}
