using System.Collections.Concurrent;
using Allard.Eventing.Abstractions.Model;
using Allard.Eventing.Abstractions.Source;

namespace Allard.Eventing.Dispatcher;

public class MessageBuffer
{
    private readonly ISourceHandler _handler;
    private readonly ConcurrentQueue<MessageContext> _messages = new();
    private readonly ManualResetEventSlim _blocker = new();

    private static int Capacity => 100;

    public bool IsFull => _messages.Count >= Capacity;

    public bool HasMessages => !_messages.IsEmpty;

    public MessageBuffer(ISourceHandler handler)
    {
        _handler = handler;
    }

    public void Add(MessageContext message)
    {
        _messages.Enqueue(message);
        _blocker.Set();
    }

    private int _isStarted;
    public async Task Start(CancellationToken stoppingToken)
    {
        Starter.Start(ref _isStarted);
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