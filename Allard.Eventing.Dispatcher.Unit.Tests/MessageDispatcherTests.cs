using FluentAssertions;
using static Allard.Eventing.Dispatcher.Subscription;

namespace Allard.Eventing.Dispatcher.Unit.Tests;

public class MessageDispatcherTests
{
    [Fact]
    public async Task Subscribe()
    {
        var cancellation = new CancellationTokenSource();
        var dispatcher = new MessageDispatcher();
        var runner = dispatcher.Start(cancellation.Token);
        await dispatcher.Subscribe(new Subscription(SingleReaderChannel(), new[] { "a" }));
        await dispatcher.Subscribe(new Subscription(SingleReaderChannel(), new[] { "a" }));
        await dispatcher.Subscribe(new Subscription(SingleReaderChannel(), new[] { "a" }));
        while (dispatcher.SubscriptionCount < 3) Thread.Sleep(10);
        dispatcher.SubscriptionCount.Should().Be(3);
        cancellation.Cancel();
        await runner;
    }
}