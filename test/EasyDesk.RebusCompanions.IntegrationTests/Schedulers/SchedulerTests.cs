using EasyDesk.CleanArchitecture.Application.Cqrs.Async;
using EasyDesk.CleanArchitecture.Testing.Integration.Bus;
using EasyDesk.CleanArchitecture.Testing.Integration.Bus.Rebus.Scheduler;
using EasyDesk.RebusCompanions.Core.Config;
using EasyDesk.RebusCompanions.Core.Scheduler;
using NodaTime;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using Rebus.Time;
using Rebus.Timeouts;

namespace EasyDesk.RebusCompanions.IntegrationTests.Schedulers;

public sealed class SchedulerTests : AbstractRebusTest, IAsyncDisposable
{
    private const string Endpoint = "test-scheduler";

    private const string SenderAddress = "sender";
    private const string ReceiverAddress = "receiver";

    private static readonly Duration _pollInterval = Duration.FromSeconds(1);

    private record Command(int Value) : ICommand;

    private readonly ITestBusEndpoint _sender;
    private readonly ITestBusEndpoint _receiver;

    private readonly RebusScheduler _scheduler;

    public SchedulerTests()
    {
        _sender = CreateBus(SenderAddress, rebus => UsingScheduler(rebus).Routing(r => r.TypeBased().MapFallback(ReceiverAddress)));
        _receiver = CreateBus(ReceiverAddress, rebus => UsingScheduler(rebus));

        var store = new InMemTimeoutStore();
        _scheduler = new RebusScheduler(
            Configuration,
            t =>
            {
                t.Decorate(c => new InMemTimeoutManager(store, c.Get<IRebusTime>()));
            },
            Endpoint);
        _scheduler.Start();
    }

    private RebusConfigurer UsingScheduler(RebusConfigurer rebus)
    {
        return rebus.Timeouts(t => t.UseExternalTimeoutManager(Endpoint));
    }

    protected override void ConfigureRebus(RebusConfiguration configuration) =>
        configuration.WithOptions(o => o.SetDueTimeoutsPollInteval(_pollInterval.ToTimeSpan()));

    [Fact]
    public async Task ShouldDeliverMessagesAfterTheGivenDelay()
    {
        var command = new Command(1);
        var delay = Duration.FromSeconds(5);
        var epsilon = Duration.FromMilliseconds(1);

        await _sender.Defer(command, delay);

        Clock.Advance(delay - epsilon);

        await _receiver.FailIfMessageIsReceived(command);

        Clock.Advance(epsilon);

        await _receiver.WaitForMessageOrFail(command);
    }

    public async ValueTask DisposeAsync()
    {
        _scheduler.Dispose();
        await _sender.DisposeAsync();
        await _receiver.DisposeAsync();
    }
}
