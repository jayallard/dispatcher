using Allard.Eventing.Abstractions.Model;

namespace Allard.Eventing.Abstractions.Source;

public interface ISourceHandler
{
    Task Handle(MessageContext message, CancellationToken cancellationToken);
}