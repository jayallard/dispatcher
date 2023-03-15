namespace Allard.Eventing.Abstractions;

public class MessageContext
{
    public MessageEnvelope Message { get; }
    public DateTimeOffset Started { get; } = DateTimeOffset.Now;
    public DateTimeOffset? Finished { get; private set; }

    internal void Finish()
    {
        Finished = DateTimeOffset.Now;
    }

    public MessageContext(MessageEnvelope message)
    {
        Message = message;
    }
}