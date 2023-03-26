namespace Allard.Eventing.Abstractions;

public interface ISourceHandler
{
    Task Handle(MessageContext message, CancellationToken cancellationToken);
}