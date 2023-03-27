namespace Allard.Eventing.Abstractions;

public class SourceHandlerFactoryFunc : ISourceHandlerFactory
{
    private readonly Func<string, ISourceHandler> _func;

    public SourceHandlerFactoryFunc(Func<string, ISourceHandler> func)
    {
        _func = func;
    }

    public ISourceHandler CreateHandler(string key)
    {
        return _func(key);
    }
}