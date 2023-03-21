using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public interface ISubscriberConsumerFactory
{
    SubscriberConsumer Create(Subscriber subscriber, CancellationToken stoppingToken);
}