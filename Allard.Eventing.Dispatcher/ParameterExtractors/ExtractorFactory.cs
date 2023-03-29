using System.Reflection;
using Allard.Eventing.Abstractions;
using Allard.Eventing.Abstractions.Model;

namespace Allard.Eventing.Dispatcher.ParameterExtractors;


// later, convert to a class and inject it into a service
// that emits SingleMessageHandlerMethods.

public static class ExtractorFactory
{
    public static IParameterExtractor MessageContextExtractor { get; } = new MessageContextExtractor();
    public static IParameterExtractor MessageEnvelopeExtractor { get; } = new MessageEnvelopeExtractor();
    public static IParameterExtractor MessageOriginExtractor { get; } = new MessageOriginExtractor();

    public static IEnumerable<IParameterExtractor> GetExtractors(MethodInfo method)
    {
        return method.GetParameters().Select(GetExtractor);
    }

    public static IParameterExtractor GetExtractor(ParameterInfo parameter)
    {
        if (parameter.ParameterType == typeof(MessageContext))
        {
            return MessageContextExtractor;
        }

        if (parameter.ParameterType == typeof(MessageEnvelope))
        {
            return MessageEnvelopeExtractor;
        }

        if (parameter.ParameterType == typeof(MessageOrigin))
        {
            return MessageOriginExtractor;
        }

        throw new InvalidOperationException("unhandled parameter type: " + parameter.ParameterType.FullName);
    }
}