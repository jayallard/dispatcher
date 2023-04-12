using Allard.Eventing.Abstractions.Model;

namespace Allard.Eventing.Abstractions.Source;

public interface ISourceHandler
{
    Task HandleCommand(HandlerCommand command);
    
    Task HandleMessage(MessageEnvelope message, CancellationToken cancellationToken);
}