using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public class MessageDispatcher
{
    private Timer _timer;
    private readonly ISubscriberConsumerFactory _subscriberConsumerFactory;

    // initialization fields
    private readonly Subscriber[] _subscribers;
    private readonly Source[] _sources;
    
    // setup when START is called
    private SourceTask[] _sourceTasks;
    private SubscriberTask[] _subscriberTasks;
    public int SubscriptionCount => _sourceTasks.Length;

    public MessageDispatcher(
        IEnumerable<Source> sources,
        IEnumerable<Subscriber> subscribers,
        ISubscriberConsumerFactory subscriberConsumerFactory)
    {
        _sources = sources.ToArray();
        _subscribers = subscribers.ToArray();
        _subscriberConsumerFactory = subscriberConsumerFactory;
    }

    private void StartSubscribers()
    {
        _subscriberTasks = _subscribers
            .Select(s =>
            {
                var cancellationSource = new CancellationTokenSource();
                var consumer = _subscriberConsumerFactory.Create(s, cancellationSource.Token);
                var runner = consumer.Start();
                var subscriberTask = new SubscriberTask(consumer, runner, cancellationSource);
                return subscriberTask;
            })
            .ToArray();
    }

    private void StartSources()
    {
        _sourceTasks = _sources
            .Select(s =>
            {
                var cancel = new CancellationTokenSource();
                var handler = new Func<MessageEnvelope, Task>(async m =>
                {
                    // m.SourceId = s.SourceId
                    await Dispatch(m);
                });

                var runner = s.MessageSource.Start(handler, cancel.Token);
                return new SourceTask(s.SourceId, runner, cancel);
            })
            .ToArray();
    }

    private async Task Dispatch(MessageEnvelope message)
    {
        var subscribers = _subscriberTasks
            .Where(t => t.Consumer.Subscriber.Condition(message));
        foreach (var subscriber in subscribers)
        {
            await subscriber.Consumer.Send(message);
        }
    }

    /// <summary>
    /// A message to unblock subscribers so that they are
    /// given the opportunity to fire triggers
    /// </summary>
    private readonly MessageEnvelope _wakeUp = MessageEnvelopeBuilder
        .CreateMessage("dispatch::wakeup")
        .SetOrigin("dispatch::", "dispatch::", 0)
        .Build();

    private async Task Maintenance()
    {
        foreach (var subscriber in _subscribers)
        {
            // await subscriber.Consumer.Send(_wakeUp);
        }
    }

    public async Task Start(CancellationToken stoppingToken)
    {
        StartSubscribers();
        StartSources();
        await Task.Yield();

        // TODO: this is temp
        stoppingToken.WaitHandle.WaitOne();
    }

    private record SubscriberTask(SubscriberConsumer Consumer, Task Runner, CancellationTokenSource StoppingToken);

    private record SourceTask(string SourceId, Task Runner, CancellationTokenSource StoppingToken);
}