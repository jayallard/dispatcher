using System.Collections.Concurrent;
using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public class Buffers
{
    private readonly ConcurrentDictionary<PrimaryPartitionKey, Lazy<MessageBufferTask>> _buffers = new();
    public readonly Func<MessageContext, Task> _handler;

    public Buffers(Func<MessageContext, Task> handler)
    {
        _handler = handler;
    }

    public MessageBuffer GetBuffer(PrimaryPartitionKey key)
    {
        return _buffers.GetOrAdd(key, k =>
        {
            return new Lazy<MessageBufferTask>(() =>
            {
                var buffer = new MessageBuffer(_handler);
                var cancellationSource = new CancellationTokenSource();
                var runner = buffer.Start(cancellationSource.Token);
                var task = new MessageBufferTask(buffer, runner, cancellationSource);
                return task;
            });
        }).Value.Buffer;
    }
}