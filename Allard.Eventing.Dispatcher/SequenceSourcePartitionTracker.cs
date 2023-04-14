using System.Collections.Concurrent;
using Allard.Eventing.Abstractions;
using Allard.Eventing.Abstractions.Model;

namespace Allard.Eventing.Dispatcher;

public class SequenceSourcePartitionTracker : ISourcePartitionTracker
{
    private readonly ConcurrentDictionary<MessageOrigin, bool> _ids = new();

    public void Start(MessageOrigin origin)
    {
        if (_ids.ContainsKey(origin))
        {
            throw new InvalidOperationException("already started");
        }
        
        var isOutOfOrder = _ids
            .Keys
            .Any(k =>
                k.StreamId == origin.StreamId
                && k.PartitionId == origin.PartitionId
                && k.SequenceNumber >= origin.SequenceNumber);
        if (isOutOfOrder) throw new InvalidOperationException("can't start items out of order");
        _ids[origin] = false;
        return;

    }

    public void Complete(MessageOrigin origin)
    {
        if (_ids.TryUpdate(origin, true, false)) return;
        if (_ids.ContainsKey(origin))
        {
            throw new InvalidOperationException("already complete");
        }

        throw new InvalidOperationException("unknown id");
    }

    public void Clear(IEnumerable<MessageOrigin> origin)
    {
        foreach (var o in origin)
        {
            _ids.TryRemove(o, out _);
        }
    }

    public IEnumerable<MessageOrigin> GetComplete()
    {
        var groups = _ids
            .GroupBy(kv => new { kv.Key.StreamId, kv.Key.PartitionId })
            .ToArray();

        var complete = new List<MessageOrigin>();
        foreach (var group in groups)
        {
            var values = group
                .OrderBy(p => p.Key.SequenceNumber);

            foreach (var v in values)
            {
                if (v.Value)
                {
                    complete.Add(v.Key);
                    continue;
                }

                break;
            }
        }
        
        return complete;
    }
}