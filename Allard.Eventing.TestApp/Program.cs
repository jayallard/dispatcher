using System.Threading.Channels;
using Allard.Eventing.Dispatcher;
using Allard.Eventing.TestApp;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services
            .AddHostedService<Worker>()
            .AddSingleton<MessageDispatcher>();
    })
    .Build();

// host.Run();
var dispatcher = host.Services.GetRequiredService<MessageDispatcher>();
var runner = dispatcher.Start(default);

var sub = Channel.CreateUnbounded<DispatchEnvelope>(new UnboundedChannelOptions
{
    SingleReader = true,
    SingleWriter = true,
});

var sub1 = new Subscription(sub,new[] { "a" });
await dispatcher.Subscribe(sub1);

while (dispatcher.SubscriptionCount != 1)
{
    Thread.Sleep(10);
}

var x = Task.Run(async () =>
{
    Console.WriteLine("run");
    var i = 0;
    while (await sub1.SubscriptionChannel.Reader.WaitToReadAsync())
    {
        Console.WriteLine("starting inner loop");
        while (sub1.SubscriptionChannel.Reader.TryRead(out var m))
        {
            Console.WriteLine("received " + i++);
        }
    }
});

Console.WriteLine("dispatching");
for (var i = 0; i < 10; i++)
{
    await dispatcher.Dispatch(new DispatchEnvelope("a"));
    Thread.Sleep(100);
}


await runner;