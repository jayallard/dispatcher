using System.Collections.Immutable;
using Allard.Eventing.Abstractions.Model;
using Allard.Eventing.Abstractions.Source;
using Allard.Eventing.Dispatcher.ParameterExtractors;
using Microsoft.Extensions.DependencyInjection;

namespace Allard.Eventing.Dispatcher;

public class DispatchSourceHandler : ISourceHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ImmutableDictionary<string, ImmutableArray<SingleMessageHandlerMethod>> _subscribersByMessageType;
    private int _received;
    private readonly SourceConfig _config;

    public DispatchSourceHandler(
        IServiceProvider serviceProvider,
        SourceConfig sourceConfig)
    {
        _subscribersByMessageType = sourceConfig
            .SubscriberTypes
            .SelectMany(MethodHandlerFactory.GetHandlers)
            .SelectMany(s => s.MessageTypes.Select(mt => new { MessageType = mt, Subscriber = s }))
            .GroupBy(s => s.MessageType)
            .ToDictionary(kv => kv.Key, kv => kv.Select(x => x.Subscriber).ToImmutableArray())
            .ToImmutableDictionary();
        _serviceProvider = serviceProvider;
        _config = sourceConfig;
    }

    public Task HandleCommand(HandlerCommand command)
    {
        return Task.CompletedTask;
    }

    public async Task HandleMessage(MessageEnvelope message, CancellationToken cancellationToken)
    {
        // Console.WriteLine("received " + _received++);
        if (!_subscribersByMessageType.TryGetValue(message.MessageType, out var subscribers))
        {
            return;
        }

        foreach (var sub in subscribers)
        {
            var handler = _serviceProvider.GetRequiredService(sub.SubscriberType);
            var context = new MessageContext(message, _config.SourceId, _serviceProvider);
            await sub.Execute(context, handler);
        }
    }
}