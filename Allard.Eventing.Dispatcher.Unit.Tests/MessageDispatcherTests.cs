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
        var source2 = new DirectSource();
        var received1 = 0;
        var received2 = 0;

        var sub1 = SubscriberBuilder
            .CreateSubscription("test1")
            .SetCondition(m => m.MessageType == "a")
            .SetScopeLifetime(new ScopeMaxCountOrDuration( 10_000, FromSeconds(1)))
            .SetHandler(m =>
            {
                if (!m.Current.Message.IsDispatchMessage())
                {
                    received1++;
                }

                return Task.CompletedTask;
            })
            .Build();

        var sub2 =
            SubscriberBuilder
                .CreateSubscription("test2")
                .SetCondition(m => m.MessageType == "b")
                .SetHandler(m =>
                {
                    // Thread.Sleep(100);
                    if (!m.Current.Message.IsDispatchMessage())
                    {
                        received2++;
                    }

                    return Task.CompletedTask;
                })
                .Build();

        var dispatcher = new MessageDispatcher(
            new[]
            {
                new Source("direct", source1),
                new Source("direct2", source2)
            },
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

        var taskA = Task.Run(async () =>
        {
            while (!stopper.IsCancellationRequested)
            {
                await source1.Write(message1);
            }
        });
        
        var taskB = Task.Run(async () =>
        {
            while (!stopper.IsCancellationRequested)
            {
                await source2.Write(message2);
            }
        });
        
        await Task.WhenAll(taskA, taskB);
        await runner;
        _testOutputHelper.WriteLine("Received 1: " + received1);
        _testOutputHelper.WriteLine("Received 2: " + received2);
        _testOutputHelper.WriteLine("Commits: " + SubscriberConsumer.CommitCount);
    }
}