using Allard.Eventing.Abstractions.Source;

namespace Allard.Eventing.Dispatcher;

public class MessageSource
{
    public MessageSource(
        string id, 
        ISource source)
    {
        Id = id;
        Source = source;
    }
    
    public ISource Source { get; }
    public string Id { get; }
}