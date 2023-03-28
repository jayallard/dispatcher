using System.Collections.Concurrent;
using Allard.Eventing.Abstractions;
using Allard.Eventing.Abstractions.Source;

namespace Allard.Eventing.Dispatcher;

public class MessageBuffer
{
    private readonly ISourceHandler _handler;
    private readonly ConcurrentQueue<MessageContext> _messages = new();
    private readonly ManualResetEventSlim _blocker = new();

    public int Capacity => 100;

    public bool IsFull => _messages.Count >= Capacity;

    public bool HasMessages => _messages.Count > 0;

    public MessageBuffer(ISourceHandler handler)
    {
        _handler = handler;
    }

    public void Add(MessageContext message)
    {
        _messages.Enqueue(message);
        _blocker.Set();
    }

    public int Count { get; private set; }
    private int _isStarted;
    public async Task Start(CancellationToken stoppingToken)
    {
        Starter.EnsureCanStart(ref _isStarted);
        await Task.Yield();
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_messages.TryDequeue(out var message))
                {
                    await _handler.Handle(message, stoppingToken);
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