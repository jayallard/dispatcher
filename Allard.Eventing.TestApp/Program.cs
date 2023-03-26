using System.Diagnostics;
using Allard.Eventing.Dispatcher;
using static Allard.Eventing.Abstractions.MessageEnvelopeBuilder;

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
var sources = new[]
{
    new MessageSource(
        "uno", 
        _ =>
        {
            counter.Signal();
            // if (counter.CurrentCount % 100_000 == 0)
            // {
            //     Console.WriteLine(counter.CurrentCount);
            // }

            return Task.CompletedTask;
        }, 
        new DirectSource())
};

var messageA = CreateMessage("a")
    .SetKey("key")
    .SetMessage("hi there")
    .SetOrigin("test", "test", 1)
    .Build();

var writeWatch = Stopwatch.StartNew();
var source = (DirectSource)sources[0].Source;
for (var i = 0; i < sendCount; i++)
{
    source.Send(messageA);
}

writeWatch.Stop();

var cancellationSource = new CancellationTokenSource();
var dispatcher = new MessageDispatcher(sources);

var readWatch = Stopwatch.StartNew();
var runner = dispatcher.Start(cancellationSource.Token);

counter.Wait();
readWatch.Stop();

Console.WriteLine("Write Time: " + writeWatch.ElapsedMilliseconds.ToString("#,###"));
Console.WriteLine("Read Time: " + readWatch.ElapsedMilliseconds.ToString("#,###"));
await runner;
