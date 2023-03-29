using System.Reflection;
using System.Text;
using System.Text.Json;
using Allard.Eventing.Abstractions;
using Allard.Eventing.Abstractions.Model;

namespace Allard.Eventing.Dispatcher.ParameterExtractors;

public class DeserializedBodyExtractor : IParameterExtractor
{
    public DeserializedBodyExtractor(ParameterInfo parameterInfo)
    {
        ParameterInfo = parameterInfo;
    }

    public ParameterInfo ParameterInfo { get; }
    public object ExtractParameter(MessageContext messageContext)
    {
        var json = Encoding.UTF8.GetString(messageContext.Message.Body);
        return JsonSerializer.Deserialize(json, ParameterInfo.ParameterType) ??
               throw new InvalidOperationException("unable to deserialize parameter");
    }
}