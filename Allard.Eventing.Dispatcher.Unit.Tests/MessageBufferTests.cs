using System.Diagnostics;
using Allard.Eventing.Abstractions;
using Xunit.Abstractions;

namespace Allard.Eventing.Dispatcher.Unit.Tests;

public class MessageBufferTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public MessageBufferTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task BeFast()
    {
        const int count = 10_000_000;

        var received = 0;
        var message = new MessageContext(MessageEnvelopeBuilder.CreateMessage("a").Build());
        var counter = new CountdownEvent(count);
        var buffer = new MessageBuffer(_ =>
        {
            received++;
            counter.Signal();
            return Task.CompletedTask;
        });

        var source = new CancellationTokenSource();

        var writer = Task.Run(() =>
        {
            var writeTime = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
            {
                buffer.Add(message);
            }

            writeTime.Stop();
            _testOutputHelper.WriteLine("Write time: " + writeTime.ElapsedMilliseconds);
        }, source.Token);
        await writer;

        var readTimer = Stopwatch.StartNew();
        var runner = buffer.Start(source.Token);
        counter.Wait(source.Token);
        readTimer.Stop();
        _testOutputHelper.WriteLine("Read time: " + readTimer.ElapsedMilliseconds);

        source.Cancel();
        try
        {
            await runner;
        }
        catch (OperationCanceledException)
        {
        }

        _testOutputHelper.WriteLine("Received: " + received.ToString("#,###"));
    }
}
