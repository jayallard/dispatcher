using System.Diagnostics;
using System.Text;
using System.Threading.Channels;
using Allard.Eventing.Abstractions;
using Allard.Eventing.Dispatcher;
using Allard.Eventing.TestApp;
using static System.TimeSpan;
using static Allard.Eventing.Abstractions.MessageEnvelopeBuilder;
using static Allard.Eventing.Abstractions.SubscriptionBuilder;

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

var received = new CountdownEvent(20_000);
await dispatcher
    .Subscribe(
        CreateSubscription("test1")
            .AddMessageType("a")
            .SetHandler(c =>
            {
                if (!c.Current.Message.IsDispatchMessage())
                {
                    received.Signal();
                }
                
                // Console.WriteLine(
                //     "received: " + Encoding.UTF8.GetString(c.Current?.Message.Body ?? Array.Empty<byte>()));
                // Console.WriteLine("\t\t" + received.CurrentCount);
                //Thread.Sleep(2_000);
                return Task.CompletedTask;
            })
            // .AddTrigger(new TimeOrCountTrigger(FromSeconds(10), 7), new TriggerActionCommit())
            .SetScopeLifetime(new ScopeMaxCountOrDuration(FromSeconds(10), 7))
            .Build());


await dispatcher
    .Subscribe(
        CreateSubscription("test1")
            .AddMessageType("a")
            .SetHandler(c =>
            {
                if (!c.Current.Message.IsDispatchMessage())
                {
                    received.Signal();
                }

                // Console.WriteLine("commitable 2: " + c.Current.Message.MessageType + " - " +
                //                   Encoding.UTF8.GetString(c.Current.Message.Body));
                return Task.CompletedTask;
            })
            .Build());

var runner = dispatcher.Start(default);
Console.WriteLine("Waiting for subscriptions");
while (dispatcher.SubscriptionCount != 2)
{
    Thread.Sleep(10);
}

Console.WriteLine("dispatching");
var message = CreateMessage("a")
    .SetKey("key")
    .SetMessage("hi there")
    .SetOrigin("test", "test", 1)
    .Build();

var watch = Stopwatch.StartNew();
for (var i = 0; i < 10_000; i++)
{
    await dispatcher.Dispatch(message);
}

received.Wait();
watch.Stop();
Console.WriteLine("ELAPSED: " + watch.ElapsedMilliseconds);



await runner;