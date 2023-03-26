using System.Collections.Concurrent;
using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public class DirectSource : ISource2
{
    private readonly ConcurrentQueue<MessageEnvelope> _messages = new();

    public void Send(MessageEnvelope message)
    {
        _messages.Enqueue(message);
    }
    
    public Task<MessageEnvelope?> Get(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_messages.TryDequeue(out var message)
            ? message
            : null);
    }
}