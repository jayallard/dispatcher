using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public interface ISubscriberConsumerFactory
{
    SubscriberConsumer Create(Subscription subscription, CancellationToken stoppingToken);
}