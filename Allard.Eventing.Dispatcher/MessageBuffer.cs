using System.Collections.Concurrent;
using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public class MessageBuffer
{
    private readonly Func<MessageContext, Task> _handler;
    private readonly ConcurrentQueue<MessageContext> _messages = new();
    private readonly ManualResetEventSlim _blocker = new();

    public int Capacity => 100;

    public bool Full => _messages.Count >= Capacity;

    public bool HasMessages => _messages.Count > 0;

    public MessageBuffer(Func<MessageContext, Task> handler)
    {
        _handler = handler;
    }

    public void Add(MessageContext message)
    {
        _messages.Enqueue(message);
        _blocker.Set();
    }

    public int Count { get; private set; }
    private readonly Starter _starter = new();
    public async Task Start(CancellationToken stoppingToken)
    {
        _starter.Start();
        await Task.Yield();
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_messages.TryDequeue(out var message))
                {
                    await _handler(message);
                    continue;
                }

                _blocker.Wait(stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}