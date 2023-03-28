using System.Reflection;
using System.Text;
using System.Text.Json;
using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher.ParameterExtractors;

public class DeserializedBodyExtractor : IParameterExtractor
{
    public object ExtractParameter(ParameterInfo parameter, MessageContext messageContext)
    {
        var json = Encoding.UTF8.GetString(messageContext.Message.Body);
        return JsonSerializer.Deserialize(json, parameter.ParameterType) ??
               throw new InvalidOperationException("unable to deserialize paramter");
    }
}