using System.Collections.Concurrent;
using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public class DirectSource : IPartitionedSource
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<MessageEnvelope>> _messages = new();
    private readonly ConcurrentDictionary<string, int> _paused = new();

    public void Send(MessageEnvelope message)
    {
        _messages
            .GetOrAdd(message.Origin.PartitionId, p => new ConcurrentQueue<MessageEnvelope>())
            .Enqueue(message);
    }

    private int _last = 0;

    public async Task<MessageEnvelope?> Get(CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        var queues = _messages
            .Where(v => !_paused.ContainsKey(v.Key))
            .Select(kv => kv.Value)
            .ToArray();
        
        var count = 0;
        if (_last > queues.Length - 1)
        {
            _last = 0;
        }
        
        while (count < queues.Length)
        {
            count++;
            if (queues[_last].TryDequeue(out var m))
            {
                _last++;
                return m;
            }

            _last++;
            if (_last == queues.Length)
            {
                _last = 0;
            }
        }

        return null;
    }

    public async Task PausePartitions(IEnumerable<MessageOrigin> origins)
    {
        await Task.Yield();
        foreach (var o in origins)
        {
            _paused.TryAdd(o.PartitionId, 0);
        }
    }

    public async Task ResumePartition(IEnumerable<MessageOrigin> origins)
    {
        await Task.Yield();
        foreach (var o in origins)
        {
            _paused.TryRemove(o.PartitionId, out _);
        }
    }
}