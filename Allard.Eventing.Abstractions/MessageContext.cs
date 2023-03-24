namespace Allard.Eventing.Abstractions;

public class MessageContext
{
    public MessageEnvelope Message { get; }

    public MessageContext(MessageEnvelope message)
    {
        Message = message;
    }
}