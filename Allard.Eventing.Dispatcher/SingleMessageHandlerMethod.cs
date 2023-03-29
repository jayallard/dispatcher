using System.Collections.Immutable;
using System.Reflection;
using Allard.Eventing.Abstractions;
using Allard.Eventing.Abstractions.Model;
using Allard.Eventing.Dispatcher.ParameterExtractors;

namespace Allard.Eventing.Dispatcher;

public class SingleMessageHandlerMethod
{
    public SingleMessageHandlerMethod(
        MethodInfo method)
    {
        Method = method;
        MessageTypes = method
            .GetCustomAttributes<MessageHandlerAttribute>()
            .Select(a => a.MessageType)
            .ToImmutableHashSet();
        if (!MessageTypes.Any())
        {
            throw new InvalidOperationException("not a message handler");
        }

        Extractors = ExtractorFactory.GetExtractors(method).ToImmutableArray();
        if (!Extractors.Any())
        {
            throw new InvalidOperationException("method doesn't have any parameters");
        }
    }

    public async Task Execute(MessageContext message, object instance)
    {
        var parameters = Extractors.Select(e => e.ExtractParameter(message)).ToArray();
        var task = (Task)Method.Invoke(instance, parameters)!;
        await task;
    }

    public MethodInfo Method { get; }

    public Type SubscriberType => Method.DeclaringType ?? throw new InvalidOperationException("type doesn't exist");
    public ImmutableHashSet<string> MessageTypes { get; }
    public ImmutableArray<IParameterExtractor> Extractors { get; }
}