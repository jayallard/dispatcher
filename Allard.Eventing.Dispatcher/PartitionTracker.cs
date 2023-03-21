using System.Collections.Concurrent;
using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public class PartitionTracker
{
    private readonly ConcurrentDictionary<PartitionKey, MessageTracker> _trackers = new();

    public MessageTracker GetTracker(PartitionKey key)
    {
        return _trackers[key];
    }

    public void Track(MessageContext message)
    {
        var key = new PartitionKey(
            message.Message.Origin.StreamId,
            message.Message.Origin.PartitionId);
        _trackers.GetOrAdd(key, k => new MessageTracker(k)).Add(message);
    }
}