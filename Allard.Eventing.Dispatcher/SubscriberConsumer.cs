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

    public async Task Start()
    {
        try
        {
            // processes the command channel
            var reader = _channel.Reader;
            var context = new DispatchContext();
            while (await reader.WaitToReadAsync(StoppingToken))
            {
                while (reader.TryRead(out var message))
                {
                    if (!message.IsDispatchMessage())
                    {
                        var messageContext = new MessageContext(message);
                        context.SetCurrent(messageContext);
                        await Subscription.Handler(context);
                    }
                    
                    // process triggers
                    foreach (var trigger in Subscription.Triggers)
                    {
                        var readiness = await trigger.Condition.GetReadiness(context);
                        if (!readiness.IsReady)
                        {
                            continue;
                        }

                        var triggerMessage = await trigger.Action.Trigger(context);
                        if (triggerMessage == null)
                        {
                            continue;
                        }
                        
                        await Subscription.Handler(context);
                        if (triggerMessage.MessageType == "dispatch::commit")
                        {
                            Console.WriteLine("Committed\n\tCount=" + context.MessageCount + "\n\tDuration=" + context.Started.Elapsed().TotalMilliseconds);
                            context = new DispatchContext();
                        }
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}