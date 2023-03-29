using Allard.Eventing.Abstractions.Model;

namespace Allard.Eventing.Abstractions.Source;

public interface ISource
{
    Task<MessageEnvelope?> Get(CancellationToken cancellationToken = default);
}