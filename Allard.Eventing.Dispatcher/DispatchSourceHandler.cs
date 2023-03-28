using Allard.Eventing.Abstractions;
using Allard.Eventing.Abstractions.Source;

namespace Allard.Eventing.Dispatcher;

public class DispatchSourceHandler : ISourceHandler
{
    public Task Handle(MessageContext message, CancellationToken cancellationToken)
    {
        Console.WriteLine("received");
        return Task.CompletedTask;
    }
}