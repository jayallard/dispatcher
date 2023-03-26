using System.Diagnostics;
using Allard.Eventing.Abstractions;
using Allard.Eventing.Dispatcher;
using static System.TimeSpan;
using static Allard.Eventing.Abstractions.MessageEnvelopeBuilder;
using static Allard.Eventing.Abstractions.SubscriberBuilder;

// var host = Host.CreateDefaultBuilder(args)
//     .ConfigureServices(services =>
//     {
//         services
//             .AddHostedService<Worker>()
//             .AddSingleton<MessageDispatcher>()
//             .AddSingleton<ISubscriberConsumerFactory, SubscriberConsumerFactoryDi>();
//     })
//     .Build();
const int sendCount = 10_000_000;
var counter = new CountdownEvent(sendCount);
var sub1 = CreateSubscription("test")
    .SetScopeLifetime(new ScopeMaxCountOrDuration(7, FromSeconds(10)))
    .SetCondition(m => m.MessageType == "a")
    .SetHandler(c =>
    {
        if (!c.Current.Message.IsDispatchMessage())
        {
            counter.Signal();
        }

        return Task.CompletedTask;
    })
    .Build();

var sources = new[]
{
    new DirectSource()
};

var messageA = CreateMessage("a")
    .SetKey("key")
    .SetMessage("hi there")
    .SetOrigin("test", "test", 1)
    .Build();

var writeWatch = Stopwatch.StartNew();
for (var i = 0; i < sendCount; i++)
{
    sources[0].Send(messageA);
}
writeWatch.Stop();


var cancellationSource = new CancellationTokenSource();
var subscribers = new[] { sub1 };
var dispatcher = new MessageDispatcher(sources, subscribers);

var readWatch = Stopwatch.StartNew();
var runner = dispatcher.Start(cancellationSource.Token);
while (!counter.IsSet)
{
    Console.WriteLine(counter.CurrentCount);
    Thread.Sleep(1_000);
}
counter.Wait();
readWatch.Stop();

Console.WriteLine("Write Time: " + writeWatch.ElapsedMilliseconds.ToString("#,###"));
Console.WriteLine("Read Time: " + readWatch.ElapsedMilliseconds.ToString("#,###"));
await runner;
