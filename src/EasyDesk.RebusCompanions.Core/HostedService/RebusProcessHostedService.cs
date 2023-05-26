using Microsoft.Extensions.Hosting;

namespace EasyDesk.RebusCompanions.Core.HostedService;

public class RebusProcessHostedService<T> : IHostedService
    where T : RebusProcess
{
    private readonly T _rebusProcess;

    public RebusProcessHostedService(T rebusProcess)
    {
        _rebusProcess = rebusProcess;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _rebusProcess.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _rebusProcess.Dispose();
        return Task.CompletedTask;
    }
}
