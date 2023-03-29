using Allard.Eventing.Abstractions.Model;

namespace Allard.Eventing.Abstractions;

public interface IParameterExtractor
{
    object ExtractParameter(MessageContext messageContext);
}