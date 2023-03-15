using Allard.Eventing.Abstractions;
using FluentAssertions;

namespace Allard.Eventing.Dispatcher.Unit.Tests;

public class MessageDispatcherTests
{
    [Fact]
    public async Task Subscribe()
    {
        var cancellation = new CancellationTokenSource();
        var dispatcher = new MessageDispatcher();
        var runner = dispatcher.Start(cancellation.Token);

        var sub = SubscriptionBuilder
            .CreateSubscription("1")
            .SetHandler(_ => Task.CompletedTask)
            .AddMessageType("a");
        
        await dispatcher.Subscribe(sub.Build());
        await dispatcher.Subscribe(sub.SetId("b").Build());
        await dispatcher.Subscribe(sub.SetId("c").Build());
        while (dispatcher.SubscriptionCount < 3) Thread.Sleep(10);
        dispatcher.SubscriptionCount.Should().Be(3);
        cancellation.Cancel();
        await runner;
    }
}