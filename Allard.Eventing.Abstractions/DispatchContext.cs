namespace Allard.Eventing.Abstractions;

public class DispatchContext
{
    private readonly List<MessageContext> _messages = new();
    public Guid ContextId { get; } = Guid.NewGuid();
    public DateTimeOffset Started { get; } = DateTimeOffset.Now;
    public IEnumerable<MessageContext> Messages => _messages.ToArray();
    public int MessageCount => _messages.Count;
    public MessageContext? Current {get; private set; }

    public void SetCurrent(MessageContext message)
    {
        Current = message;
        if (!message.Message.IsDispatchMessage())
        {
            _messages.Add(message);
        }
    }
}