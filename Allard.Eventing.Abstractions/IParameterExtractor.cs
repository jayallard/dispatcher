using System.Reflection;

namespace Allard.Eventing.Abstractions;

public interface IParameterExtractor
{
    object ExtractParameter(ParameterInfo parameter, MessageContext messageContext);
}