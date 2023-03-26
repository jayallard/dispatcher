namespace Allard.Eventing.Abstractions;

public class SourceHandlerFactoryFunc : ISourceHandlerFactory
{
    private readonly Func<SourcePartitionKey, ISourceHandler> _func;

    public SourceHandlerFactoryFunc(Func<SourcePartitionKey, ISourceHandler> func)
    {
        _func = func;
    }

    public ISourceHandler CreateHandler(SourcePartitionKey key)
    {
        return _func(key);
    }
}