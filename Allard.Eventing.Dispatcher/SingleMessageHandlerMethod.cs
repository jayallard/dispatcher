using System.Collections.Immutable;
using System.Reflection;
using Allard.Eventing.Abstractions;
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

    public MethodInfo Method { get; }
    public ImmutableHashSet<string> MessageTypes { get; }
    public ImmutableArray<IParameterExtractor> Extractors { get; }
}