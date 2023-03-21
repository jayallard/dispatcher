using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public class MessageTracker
{
    public PartitionKey PartitionKey { get; }
    private readonly List<MessageContext> _messages = new();

    public MessageTracker(PartitionKey partitionKey)
    {
        PartitionKey = partitionKey;
    }

    public IEnumerable<MessageContext> Messages => _messages.ToArray();
    
    public DateTimeOffset FirstEntry { get; private set; }
    public DateTimeOffset LastEntry { get; private set; }

    public void Add(MessageContext message)
    {
        if (_messages.Count > 0)
        {
            if (_messages.Last().Message.Origin.SequenceNumber >= message.Message.Origin.SequenceNumber)
            {
                throw new InvalidOperationException("Message received out of order");
            }
        }
        
        LastEntry = DateTimeOffset.Now;
        if (_messages.Count == 0)
        {
            FirstEntry = LastEntry;
        }
        
        _messages.Add(message);
    }
}