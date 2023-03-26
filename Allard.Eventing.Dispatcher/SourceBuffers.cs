using System.Collections.Concurrent;
using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public class SourceBuffers
{
    private readonly ConcurrentDictionary<SourcePartitionKey, Lazy<MessageBufferTask>> _buffers = new();
    private readonly MessageSource _source;

    public SourceBuffers(MessageSource source)
    {
        _source = source;
    }

    public MessageBuffer GetBuffer(SourcePartitionKey key)
    {
        return _buffers.GetOrAdd(key, k =>
        {
            return new Lazy<MessageBufferTask>(() =>
            {
                var handler = _source.Handler.CreateHandler(key);
                var buffer = new MessageBuffer(handler);
                var cancellationSource = new CancellationTokenSource();
                var runner = buffer.Start(cancellationSource.Token);
                var task = new MessageBufferTask(buffer, runner, cancellationSource);
                return task;
            });
        }).Value.Buffer;
    }
}