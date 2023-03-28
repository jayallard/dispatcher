using Allard.Eventing.Abstractions.Source;

namespace Allard.Eventing.Dispatcher;

public class SourceHandlerFactory : ISourceHandlerFactory
{
    public ISourceHandler CreateHandler(string key)
    {
        return new DispatchSourceHandler();
    }
}