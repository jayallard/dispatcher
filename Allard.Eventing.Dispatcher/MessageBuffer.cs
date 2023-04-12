using System.Collections.Concurrent;
using Allard.Eventing.Abstractions.Model;
using Allard.Eventing.Abstractions.Source;

namespace Allard.Eventing.Dispatcher;

public class MessageBuffer
{
    private readonly ISourceHandler _handler;
    private readonly ConcurrentQueue<MessageEnvelope> _messages = new();
    private readonly ConcurrentQueue<HandlerCommand> _commands = new();
    private readonly ManualResetEventSlim _blocker = new();

    private static int Capacity => 100; 

    public bool IsFull => _messages.Count >= Capacity;

    public bool HasMessages => !_messages.IsEmpty;

    public MessageBuffer(ISourceHandler handler)
    {
        _handler = handler;
    }

    public void AddMessage(MessageEnvelope message)
    {
        _messages.Enqueue(message);
        _blocker.Set();
    }

    public void AddCommand(HandlerCommand command)
    {
        _commands.Enqueue(command);
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
                if (_commands.TryDequeue(out var command))
                {
                    await _handler.HandleCommand(command);
                }
                
                if (_messages.TryDequeue(out var message))
                {
                    await _handler.HandleMessage(message, stoppingToken);
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