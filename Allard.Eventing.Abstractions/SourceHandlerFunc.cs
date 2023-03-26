namespace Allard.Eventing.Abstractions;

public class SourceHandlerFunc : ISourceHandler
{
    private readonly Func<MessageContext, Task> _func;

    public SourceHandlerFunc(Func<MessageContext, Task> func)
    {
        _func = func;
    }

    public Task Handle(MessageContext message, CancellationToken cancellationToken)
    {
         return _func(message);
    }
}