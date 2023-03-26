using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public class MessageSource
{
    public MessageSource(
        string id, 
        ISourceHandlerFactory handlerFactory, 
        ISource source)
    {
        Id = id;
        Handler = handlerFactory;
        Source = source;
    }

    public MessageSource(
        string id,
        Func<MessageContext, Task> handler,
        ISource source
    )
    {
        var h = new SourceHandlerFunc(handler);
        var f = new SourceHandlerFactoryFunc(_ => h);
        Handler = f;
        Source = source;

    }
    
    public ISource Source { get; }
    public string Id { get; }
    public ISourceHandlerFactory Handler { get; }
}