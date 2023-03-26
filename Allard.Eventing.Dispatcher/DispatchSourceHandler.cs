using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public class DispatchSourceHandler : ISourceHandler
{
    public Task Handle(MessageContext message, CancellationToken cancellationToken)
    {
        Console.WriteLine("received");
        return Task.CompletedTask;
    }
}