using System.Collections.Concurrent;
using System.Diagnostics;
using Allard.Eventing.Abstractions;
using Allard.Eventing.Abstractions.Model;

namespace Allard.Eventing.Dispatcher;

/* this is terrible - 0% optimized. at this point, just going for functionality.
 once all test are good and we're ready to revisit, then write it better.
 */

public class SequenceSourcePartitionTracker : ISourcePartitionTracker
{
    private readonly ConcurrentDictionary<MessageOrigin, bool> _ids = new();

    /// <summary>
    /// Indicate that a key is being processed.
    /// </summary>
    /// <param name="origin"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Start(MessageOrigin origin)
    {
        if (_ids.ContainsKey(origin))
        {
            throw new InvalidOperationException("already started");
        }

        // make sure that sequence numbers are added sequentially.
        // e.g.: can't add #3 if #4 has already been added.
        // the source needs to add them as received, in order.
        // such as kafka offsets
        var isOutOfOrder = _ids
            .Keys
            .Any(k =>
                k.StreamId == origin.StreamId
                && k.PartitionId == origin.PartitionId
                && k.SequenceNumber >= origin.SequenceNumber);
        if (isOutOfOrder) throw new InvalidOperationException("can't start items out of order");
        _ids[origin] = false;
    }

    /// <summary>
    /// Indicate that processing has completed.
    /// </summary>
    /// <param name="origin"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Complete(MessageOrigin origin)
    {
        if (_ids.TryUpdate(origin, true, false)) return;
        if (_ids.ContainsKey(origin))
        {
            throw new InvalidOperationException("already complete");
        }

        throw new InvalidOperationException("unknown id");
    }

    /// <summary>
    /// Indicate that the keys, and their predecessors,
    /// have been committed in the source. These keys, and their
    /// predecessors, will be removed from the tracker.
    /// </summary>
    /// <param name="origin"></param>
    public void Clear(IEnumerable<MessageOrigin> origin)
    {
        foreach (var o in origin)
        {
            var remove = _ids
                .Keys
                .Where(k =>
                    k.StreamId == o.StreamId
                    && k.PartitionId == o.PartitionId
                    && k.SequenceNumber <= o.SequenceNumber)
                .ToArray();
            foreach (var r in remove)
            {
                _ids.TryRemove(r, out _);
            }
        }
    }

    /// <summary>
    /// Returns MessageOrigin objects that are ready to be completed.
    /// This only returns the latest sequence number per stream and partition.
    /// Examples, for a single partition:
    ///     Messages 1,2,3 are complete: returns 3
    ///     Message 1 and 2 are not complete, but 3,4,5 are: returns nothing.
    ///     Message 1 is complete, 2 isn't, 3 is: returns 1
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MessageOrigin> GetComplete()
    {
        var groups = _ids
            .GroupBy(kv => new { kv.Key.StreamId, kv.Key.PartitionId })
            .ToArray();

        var complete = new List<MessageOrigin>();
        foreach (var group in groups)
        {
            var values = group
                .OrderBy(p => p.Key.SequenceNumber)
                .ToArray();

            for (var i = 0; i < values.Length; i++)
            {
                // if the current message is COMPLETE, and
                //   - it's the last message on the list
                //   - or the next message is NOT COMPLETE
                // then, this is the last message in the COMPLETE
                // sequence, and is returned.
                var isLast = i == values.Length - 1 || !values[i + 1].Value;
                if (values[i].Value && isLast)
                {
                    complete.Add(values[i].Key);
                    break;
                }

                // this one is true,
                // and the next one is true,
                // so keep going
                if (values[i].Value) continue;
                
                // if the current value is false, then
                // we're done. nothing to do.
                // this would only happen when i = 0.
                Debug.Assert(i == 0);
                break;
            }
        }

        return complete;
    }
}