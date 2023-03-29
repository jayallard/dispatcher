namespace Allard.Eventing.Abstractions.Model;

public class MessageContext
{
    public MessageEnvelope Message { get; }
    public string SourceId { get; }

    public MessageContext(MessageEnvelope message, string sourceId)
    {
        Message = message;
        SourceId = sourceId;
    }
}