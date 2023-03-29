using System.Diagnostics;
using Allard.Eventing.Abstractions;
using Allard.Eventing.Abstractions.Model;
using Allard.Eventing.Abstractions.Source;
using Allard.Eventing.Dispatcher;
using Allard.Eventing.TestApp;

var source = new DirectSource();
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services
            .AddHostedService<Worker>()
            .SetupDispatcher(d =>
            {
                d.AddSource("source1", s =>
                {
                    s
                        .SetSource(source)
                        .SetPartitioner<PartitionByStreamId>()
                        .AddSubscriber<DemoSubscribers1>()
                        .AddSubscriber<DemoSubscribers2>();
                });
            });
    })
    .Build();

const int sendCount = 100;
var counter = new CountdownEvent(sendCount);

var messageA = MessageEnvelopeBuilder.CreateMessage("a")
    .SetKey("key")
    .SetMessage("hi there")
    .SetOrigin("test", "test", 1)
    .Build();

var writeWatch = Stopwatch.StartNew();
for (var i = 0; i < sendCount; i++)
{
    source.Send(messageA);
}

writeWatch.Stop();

var cancellationSource = new CancellationTokenSource();
var dispatcher = host.Services.GetRequiredService<MessageDispatcher2>();

var readWatch = Stopwatch.StartNew();
var runner = dispatcher.Start(cancellationSource.Token);

counter.Wait();
readWatch.Stop();

Console.WriteLine("Write Time: " + writeWatch.ElapsedMilliseconds.ToString("#,###"));
Console.WriteLine("Read Time: " + readWatch.ElapsedMilliseconds.ToString("#,###"));
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
        [MessageHandler("a")]
        public Task Blah(MessageContext context, MessageEnvelope env)
        {
            Console.WriteLine("a");
            return Task.CompletedTask;
        }
    }

    public class DemoSubscribers2 : ISubscriberMarker
    {
        [MessageHandler("a")]
        public Task Blah(MessageContext context)
        {
            Console.WriteLine("b");
            return Task.CompletedTask;
        }
    }
}