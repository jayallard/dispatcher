using System.Diagnostics;
using Allard.Eventing.Abstractions;
using Allard.Eventing.Abstractions.Model;
using Allard.Eventing.Abstractions.Source;
using Allard.Eventing.Dispatcher;
using Allard.Eventing.TestApp;

const int sendCount = 20_000;
var source = new DirectSource();
var countdown = new CountdownEvent(sendCount);
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services
            .AddSingleton(countdown)
            .AddHostedService<Worker>()
            .SetupDispatcher(d =>
            {
                d.AddSource("source1", s =>
                {
                    s
                        .AddService(sp => sp.AddSingleton(countdown))
                        .SetSource(source)
                        .SetPartitioner<PartitionByStreamId>()
                        .AddSubscriber<DemoSubscribers1>()
                        .AddSubscriber<DemoSubscribers2>();
                });
            });
    })
    .Build();

var writeWatch = Stopwatch.StartNew();
for (var i = 0; i < sendCount; i++)
{
    var messageA = MessageEnvelopeBuilder.CreateMessage("a")
        .SetKey("key")
        .SetMessage("hi there")
        .SetOrigin("test", "test", i)
        .Build();
    
    source.Send(messageA);
}

writeWatch.Stop();

var cancellationSource = new CancellationTokenSource();
var dispatcher = host.Services.GetRequiredService<MessageDispatcher2>();

var readWatch = Stopwatch.StartNew();
var runner = dispatcher.Start(cancellationSource.Token);


var counter = host.Services.GetRequiredService<CountdownEvent>();
counter.Wait();
readWatch.Stop();

Console.WriteLine("Write Time: " + writeWatch.ElapsedMilliseconds.ToString("#,###"));
Console.WriteLine("Read Time: " + readWatch.ElapsedMilliseconds.ToString("#,###"));

var reader = host.Services.GetRequiredService<MessageDispatcher2>().Readers.Single();
var tracker = reader.services.GetRequiredService<ISourcePartitionTracker>();
var complete = tracker.GetComplete().Count();
Console.WriteLine("Tracker complete: " + complete);

Console.WriteLine("--- stop");
cancellationSource.Cancel();
await runner;


namespace Allard.Eventing.TestApp
{
    public static class DependencyInjection
    {
        public static IServiceCollection SetupDispatcher(this IServiceCollection services, Action<DispatcherSetup> setup)
        {
            var s = new DispatcherSetup();
            setup(s);
            return
                services
                    .AddSingleton<MessageDispatcher2>(sp => ActivatorUtilities.CreateInstance<MessageDispatcher2>(sp, s));
        }
    }

    public class DemoSubscribers1 : ISubscriberMarker
    {
        private readonly CountdownEvent _counter;

        public DemoSubscribers1(CountdownEvent counter)
        {
            _counter = counter;
        }

        [MessageHandler("a")]
        public Task Blah(MessageContext context, MessageEnvelope env)
        {
            _counter.Signal();
            // Console.WriteLine("a");
            return Task.CompletedTask;
        }
    }

    public class DemoSubscribers2 : ISubscriberMarker
    {
        [MessageHandler("a")]
        public Task Blah(MessageContext context)
        {
            // Console.WriteLine("b");
            return Task.CompletedTask;
        }
    }
}