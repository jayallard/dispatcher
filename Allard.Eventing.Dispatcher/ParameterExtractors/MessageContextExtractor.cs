using System.Reflection;
using Allard.Eventing.Abstractions;
using Allard.Eventing.Abstractions.Model;

namespace Allard.Eventing.Dispatcher.ParameterExtractors;

public class MessageContextExtractor : IParameterExtractor
{
    public object ExtractParameter(MessageContext messageContext)
    {
        return messageContext;
    }
}