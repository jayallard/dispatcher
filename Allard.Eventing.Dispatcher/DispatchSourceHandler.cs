using System.Collections.Immutable;
using Allard.Eventing.Abstractions;
using Allard.Eventing.Abstractions.Model;
using Allard.Eventing.Abstractions.Source;
using Allard.Eventing.Dispatcher.ParameterExtractors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Allard.Eventing.Dispatcher;

public class DispatchSourceHandler : ISourceHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ImmutableDictionary<string, ImmutableArray<SingleMessageHandlerMethod>> _subscribersByMessageType;
    private int _received;

    public DispatchSourceHandler(IServiceProvider serviceProvider, SourceConfig sourceConfig)
    {
        _subscribersByMessageType = sourceConfig
            .SubscriberTypes
            .SelectMany(MethodHandlerFactory.GetHandlers)
            .SelectMany(s => s.MessageTypes.Select(mt => new { MessageType = mt, Subscriber = s }))
            .GroupBy(s => s.MessageType)
            .ToDictionary(kv => kv.Key, kv => kv.Select(x => x.Subscriber).ToImmutableArray())
            .ToImmutableDictionary();
        _serviceProvider = serviceProvider;
    }
    

    public async Task Handle(MessageContext message, CancellationToken cancellationToken)
    {
        Console.WriteLine("received " + _received++);
        if (!_subscribersByMessageType.TryGetValue(message.Message.MessageType, out var subscribers))
        {
            return;
        }

        foreach (var sub in subscribers)
        {
            var handler = _serviceProvider.GetRequiredService(sub.SubscriberType);
            await sub.Execute(message, handler);
        }
    }
}

