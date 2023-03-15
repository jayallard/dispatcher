using Allard.Eventing.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Allard.Eventing.Dispatcher;

public class SubscriberConsumerFactoryDi : ISubscriberConsumerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public SubscriberConsumerFactoryDi(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public SubscriberConsumer Create(Subscription subscription, CancellationToken stoppingToken)
    {
        return ActivatorUtilities.CreateInstance<SubscriberConsumer>(_serviceProvider, subscription, stoppingToken);
    }
}