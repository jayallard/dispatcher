using System.Text;
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

await dispatcher
    .Subscribe(
        CreateSubscription("test1")
            .AddMessageType("a")
            .SetHandler(c =>
            {
                Console.WriteLine("received: " + Encoding.UTF8.GetString(c.Current?.Message.Body ?? Array.Empty<byte>()));
                Thread.Sleep(2_000);
                return Task.CompletedTask;
            })
            .AddTrigger(new TimeOrCountTrigger(FromSeconds(10), 7), new TriggerActionCommit())
            .Build());

await dispatcher
    .Subscribe(
        CreateSubscription("test1")
            .AddMessageType("a")
            .SetHandler(c =>
            {
                Console.WriteLine("commitable 2: " + Encoding.UTF8.GetString(c.Current.Message.Body));
                return Task.CompletedTask;
            })
            .Build());

await dispatcher
    .Subscribe(
        CreateSubscription("commit")
            .AddMessageType("dispatch::commit")
            .SetHandler(_ =>
            {
                Console.WriteLine("commit");
                return Task.CompletedTask;
            })
            .Build()
    );

var runner = dispatcher.Start(default);
while (dispatcher.SubscriptionCount != 3)
{
    Thread.Sleep(10);
}

Console.WriteLine("dispatching");
for (var i = 0; i < 10; i++)
{
    var message = CreateMessage("a")
        .SetKey("key")
        .SetMessage("hi there")
        .SetOrigin("test", "test", 1)
        .Build();

    await dispatcher.Dispatch(message);
}

await runner;