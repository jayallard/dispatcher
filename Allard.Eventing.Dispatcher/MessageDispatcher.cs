using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Allard.Eventing.Dispatcher;

public class MessageDispatcher
{
    public int SubscriptionCount => _subscriptionsPerMessageType.Sum(s => s.Value.Count);
    private readonly ConcurrentDictionary<string, List<Subscription>> _subscriptionsPerMessageType = new();
    private readonly Channel<Func<Task>> _commandChannel = Channel.CreateUnbounded<Func<Task>>(
        new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

    public async Task Dispatch(DispatchEnvelope message)
    {
        await Task.Yield();
        if (!_subscriptionsPerMessageType.TryGetValue(message.MessageType, out var subscriptions))
        {
            return;
        }

        foreach (var subscription in subscriptions)
        {
            await subscription.SubscriptionChannel.Writer.WriteAsync(message);
        }
    }
    
    public async Task<MessageDispatcher> Subscribe(Subscription subscription)
    {
        await _commandChannel.Writer.WriteAsync(() =>
        {
            foreach (var messageType in subscription.MessageTypes)
            {
                _subscriptionsPerMessageType
                    .GetOrAdd(messageType, mt => new List<Subscription>())
                    .Add(subscription);
            }

            return Task.CompletedTask;
        });
        return this;
    }

    public async Task Start(CancellationToken stoppingToken)
    {
        try
        {
            var reader = _commandChannel.Reader;
            while (await reader.WaitToReadAsync(stoppingToken))
            {
                while (reader.TryRead(out var method))
                {
                    await method();
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}