using Allard.Eventing.Abstractions;
using Allard.Eventing.Abstractions.Source;

namespace Allard.Eventing.Dispatcher;

/// <summary>
/// Retrieves messages from a source, and publishes
/// them to a buffer.
/// </summary>
public class SourceReader
{
    private readonly MessageBuffers _buffers;
    private readonly IMessagePartitioner _partitioner;
    private readonly ISourcePartitionTracker _tracker;

    private int _isStarted;
    private SpinWait _spinner;
    private MessageSource _source;

    public SourceReader(
        MessageBuffers buffers,
        IMessagePartitioner partitioner,
        ISourcePartitionTracker tracker)
    {
        _buffers = buffers;
        _partitioner = partitioner;
        _tracker = tracker;
    }

    public async Task Start(
        MessageSource source,
        CancellationToken stoppingToken)
    {
        Starter.Start(ref _isStarted);
        await Task.Yield();
        _source = source;
        while (!stoppingToken.IsCancellationRequested)
        {
            var message = await _source.Source.Get(stoppingToken);
            if (message == null)
            {
                _spinner.SpinOnce();
                continue;
            }

            var key = _partitioner.GetPartitionKey(message);
            var buffer = _buffers.GetBuffer(key);
            _tracker.Start(message.Origin);
            buffer.AddMessage(message);
        }
        
        Console.WriteLine("=== reader done");
    }
}