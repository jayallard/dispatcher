using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public class SourceHandlerFactory : ISourceHandlerFactory
{
    public ISourceHandler CreateHandler(SourcePartitionKey key)
    {
        return new DispatchSourceHandler();
    }
}