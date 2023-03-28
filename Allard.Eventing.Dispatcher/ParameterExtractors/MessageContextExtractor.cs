using System.Reflection;
using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher.ParameterExtractors;

public class MessageContextExtractor : IParameterExtractor
{
    public object ExtractParameter(ParameterInfo _, MessageContext messageContext)
    {
        return messageContext;
    }
}