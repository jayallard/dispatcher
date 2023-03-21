using System.Diagnostics;
using System.Text;
using Allard.Eventing.Abstractions;
using Allard.Eventing.Dispatcher;
using Allard.Eventing.TestApp;
using static System.TimeSpan;
using static Allard.Eventing.Abstractions.MessageEnvelopeBuilder;
using static Allard.Eventing.Abstractions.SubscriberBuilder;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services
            .AddHostedService<Worker>()
            .AddSingleton<MessageDispatcher>()
            .AddSingleton<ISubscriberConsumerFactory, SubscriberConsumerFactoryDi>();
    })
    .Build();


// host.Run();
var dispatcher = host
    .Services
    .GetRequiredService<MessageDispatcher>();

var received = new CountdownEvent(10_000);
var subscriber = SubscriberBuilder
    .CreateSubscription("test")
    .SetScopeLifetime(new ScopeMaxCountOrDuration(FromSeconds(10), 7))
    .SetCondition(m => m.MessageType == "a")
    .SetHandler(c =>
    {
        if (!c.Current.Message.IsDispatchMessage())
        {
            received.Signal();
        }

        Console.WriteLine(
            "sleeper received: " + Encoding.UTF8.GetString(c.Current?.Message.Body ?? Array.Empty<byte>()));
        // Console.WriteLine("\t\t" + received.CurrentCount);
        Thread.Sleep(2_000);
        return Task.CompletedTask;
    })
    .Build();


var runner = dispatcher.Start(default);
Console.WriteLine("Waiting for subscriptions");
while (dispatcher.SubscriptionCount != 2)
{
    Thread.Sleep(10);
}

Console.WriteLine("dispatching");
var messageA = CreateMessage("a")
    .SetKey("key")
    .SetMessage("hi there")
    .SetOrigin("test", "test", 1)
    .Build();
var messageB = CreateMessage("b")
    .SetKey("key")
    .SetMessage("hi there")
    .SetOrigin("test", "test", 1)
    .Build();

// var watch = Stopwatch.StartNew();
// for (var i = 0; i < 10_000; i++)
// {
//     await dispatcher.Dispatch(messageA);
//     await dispatcher.Dispatch(messageB);
// }
// received.Wait();
// watch.Stop();
// Console.WriteLine("ELAPSED: " + watch.ElapsedMilliseconds);


await runner;

/*
 *  var pipeLine = PipeLineBuilder
 *      .PartitionByStream()
 *      .PartitionBy 
*/