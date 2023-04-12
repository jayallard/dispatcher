namespace Allard.Eventing.Abstractions.Model;

public class MessageContext
{
    public MessageEnvelope Message { get; }
    public string SourceId { get; }
    public IServiceProvider Services { get; }

    public MessageContext(MessageEnvelope message, string sourceId, IServiceProvider services)
    {
        Message = message;
        SourceId = sourceId;
        Services = services;
    }
}