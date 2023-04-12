using Allard.Eventing.Abstractions;
using Allard.Eventing.Abstractions.Model;
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
    private int _isStarted;
    private SpinWait _spinner;
    private MessageSource _source;

    public SourceReader(
        MessageBuffers buffers, 
        IMessagePartitioner partitioner)
    {
        _buffers = buffers;
        _partitioner = partitioner;
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
                buffer.AddMessage(message);
            }
        }
    }
}