using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public class MessageSource
{
    public MessageSource(
        string id, 
        ISourceHandlerFactory handlerFactory, 
        ISource source,
        ISourcePartitioner partitioner)
    {
        Id = id;
        Handler = handlerFactory;
        Source = source;
        Partitioner = partitioner;
    }

    public MessageSource(
        string id,
        Func<MessageContext, Task> handler,
        ISource source,
        ISourcePartitioner partitioner
    )
    {
        var h = new SourceHandlerFunc(handler);
        var f = new SourceHandlerFactoryFunc(_ => h);
        Handler = f;
        Source = source;
        Partitioner = partitioner;
    }
    
    public ISourcePartitioner Partitioner { get; }
    public ISource Source { get; }
    public string Id { get; }
    public ISourceHandlerFactory Handler { get; }
}