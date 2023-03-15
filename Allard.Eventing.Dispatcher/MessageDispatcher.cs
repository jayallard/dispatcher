using System.Collections.Concurrent;
using System.Threading.Channels;
using Allard.Eventing.Abstractions;
using static System.TimeSpan;

namespace Allard.Eventing.Dispatcher;

public class MessageDispatcher
{
    public int SubscriptionCount => _subscribers.Count;
    private readonly List<SubscriberTask> _subscribers = new();
    private Timer? _timer;
    private readonly ConcurrentDictionary<string, List<SubscriberTask>> _subscriptionsPerMessageType = new();
    private readonly Channel<Func<Task>> _commandChannel = Channel.CreateUnbounded<Func<Task>>(
        new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

    public async Task Dispatch(MessageEnvelope message)
    {
        if (!_subscriptionsPerMessageType.TryGetValue(message.MessageType, out var subscriptions))
        {
            return;
        }

        foreach (var subscription in subscriptions)
        {
            await subscription.Consumer.Send(message);
        }
    }
    
    public async Task<MessageDispatcher> Subscribe(Subscription subscription)
    {
        await _commandChannel.Writer.WriteAsync(() =>
        {
            foreach (var messageType in subscription.MessageTypes)
            {
                var cancellationSource = new CancellationTokenSource();
                var consumer = new SubscriberConsumer(subscription, cancellationSource.Token);
                var runner = consumer.Start(); 
                var subscriberTask = new SubscriberTask(consumer, runner, cancellationSource);
                
                _subscribers.Add(subscriberTask);
                _subscriptionsPerMessageType
                    .GetOrAdd(messageType, mt => new List<SubscriberTask>())
                    .Add(subscriberTask);
            }

            return Task.CompletedTask;
        });
        return this;
    }

    private readonly MessageEnvelope _wakeUp = MessageEnvelopeBuilder
        .CreateMessage("dispatch::wakeup")
        .SetOrigin("dispatcher", string.Empty, -1)
        .Build();

    private async Task Maintenance()
    {
        foreach (var subscriber in _subscribers)
        {
            await subscriber.Consumer.Send(_wakeUp);
        }
    }

    public async Task Start(CancellationToken stoppingToken)
    {
        try
        {
            _timer = new Timer(async _ => await Maintenance(), 
                null, 
                FromMilliseconds(100), 
                FromMilliseconds(100));
            
            // processes the command channel
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
    
    private record SubscriberTask(SubscriberConsumer Consumer, Task Runner, CancellationTokenSource Stopping);
}