using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Allard.Eventing.Dispatcher;

public class MessageBuffers
{
    private readonly ConcurrentDictionary<string, Lazy<MessageBufferTask>> _buffers = new();
    private readonly IServiceProvider _serviceProvider;

    public MessageBuffers(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public MessageBuffer GetBuffer(string key)
    {
        return _buffers.GetOrAdd(key, k =>
        {
            return new Lazy<MessageBufferTask>(() =>
            {
                var buffer = _serviceProvider.GetRequiredService<MessageBuffer>();
                var cancellationSource = new CancellationTokenSource();
                var runner = buffer.Start(cancellationSource.Token);
                return new MessageBufferTask(buffer, runner, cancellationSource);
            });
        }).Value.Buffer;
    }
}