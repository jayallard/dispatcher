using Allard.Eventing.Abstractions;
using Allard.Eventing.Abstractions.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Allard.Eventing.Dispatcher.ParameterExtractors;

public class ServiceExtractor : IParameterExtractor
{
    public Type Type { get; }
    public ServiceExtractor(Type type)
    {
        Type = type;
    }

    public object ExtractParameter(MessageContext messageContext)
    {
        return messageContext.Services.GetRequiredService(Type);
    }
}