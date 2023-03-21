using System.Threading.Channels;
using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher.Unit.Tests;

public class DirectSource : ISource
{
    private readonly Channel<MessageEnvelope> _channel = Channel.CreateBounded<MessageEnvelope>(
        new BoundedChannelOptions(100)
        {
            SingleReader = true,
            SingleWriter = true
        });

    public async Task Write(MessageEnvelope message)
    {
        await _channel.Writer.WriteAsync(message);
    }
    
    public async Task Start(Func<MessageEnvelope, Task> writer, CancellationToken stoppingToken)
    {
        while (await _channel.Reader.WaitToReadAsync(stoppingToken))
        {
            while (_channel.Reader.TryRead(out var message))
            {
                await writer(message);
            }
        }
    }
}