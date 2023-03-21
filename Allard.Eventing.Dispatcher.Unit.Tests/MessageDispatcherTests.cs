using Allard.Eventing.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using static System.TimeSpan;

namespace Allard.Eventing.Dispatcher.Unit.Tests;

public class MessageDispatcherTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public MessageDispatcherTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task X()
    {
        var di = new ServiceCollection().BuildServiceProvider();
        var factory = new SubscriberConsumerFactoryDi(di);
        var source1 = new DirectSource();
        var received1 = 0;
        var received2 = 0;

        var sub1 = SubscriberBuilder
            .CreateSubscription("test1")
            .SetCondition(m => m.MessageType == "a")
            .SetHandler(_ =>
            {
                received1++;
                return Task.CompletedTask;
            })
            .Build();

        var sub2 =
            SubscriberBuilder
                .CreateSubscription("test2")
                .SetCondition(m => m.MessageType == "b")
                .SetHandler(_ =>
                {
                    received2++;
                    return Task.CompletedTask;
                })
                .Build();

        var dispatcher = new MessageDispatcher(
            new[] { new Source("direct", source1) },
            new[] { sub1, sub2 },
            factory);

        var stopper = new CancellationTokenSource();
        stopper.CancelAfter(FromSeconds(5));
        var runner = dispatcher.Start(stopper.Token);

        var message1 = MessageEnvelopeBuilder
            .CreateMessage("a")
            .Build();

        var message2 = MessageEnvelopeBuilder
            .CreateMessage("b")
            .Build();
        
        while (!stopper.IsCancellationRequested)
        {
            await source1.Write(message1);
            await source1.Write(message2);
        }

        await runner;
        _testOutputHelper.WriteLine("Received 1: " + received1);
        _testOutputHelper.WriteLine("Received 2: " + received2);
    }
}