using System.Threading.Channels;
using Allard.Eventing.Abstractions;
using static System.Threading.Channels.BoundedChannelFullMode;

namespace Allard.Eventing.Dispatcher;

public class SubscriberConsumer
{
    public Subscription Subscription { get; }
    public CancellationToken StoppingToken { get; }

    private readonly Channel<MessageEnvelope> _channel = Channel.CreateBounded<MessageEnvelope>(
        new BoundedChannelOptions(100)
        {
            SingleReader = true,
            SingleWriter = true,
            FullMode = Wait
        });

    public SubscriberConsumer(Subscription subscription, CancellationToken stoppingToken)
    {
        Subscription = subscription;
        StoppingToken = stoppingToken;
    }

    public async Task Send(MessageEnvelope message)
    {
        await _channel.Writer.WriteAsync(message, StoppingToken);
    }

    private DispatchContext? _currentBatch;
    
    public async Task Start()
    {
        try
        {
            // processes the command channel
            var reader = _channel.Reader;
            while (await reader.WaitToReadAsync(StoppingToken))
            {
                while (reader.TryRead(out var message))
                {
                    _currentBatch ??= new DispatchContext();
                    if (!message.IsDispatchMessage())
                    {
                        var messageContext = new MessageContext(message);
                        _currentBatch.SetCurrent(messageContext);
                        await Subscription.Handler(_currentBatch);
                    }

                    await ProcessTriggers();
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task ProcessTriggers()
    {
        if (_currentBatch == null)
        {
            return;
        }
        
        // process triggers
        foreach (var trigger in Subscription.Triggers)
        {
            var readiness = await trigger.Condition.GetReadiness(_currentBatch);
            if (!readiness.IsReady)
            {
                continue;
            }

            var triggerMessage = await trigger.Action.Trigger(_currentBatch);
            if (triggerMessage == null)
            {
                continue;
            }
                        
            await Subscription.Handler(_currentBatch);
            if (triggerMessage.MessageType == "dispatch::commit")
            {
                Console.WriteLine("Committed\n\tCount=" + _currentBatch.MessageCount + "\n\tElapsed=" + _currentBatch.Started.Elapsed().TotalMilliseconds);
                _currentBatch = null;
            }
        }
    }
}