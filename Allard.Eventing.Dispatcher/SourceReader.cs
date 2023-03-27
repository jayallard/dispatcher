using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public class SourceReader
{
    private readonly MessageSource _source;
    private readonly Starter _starter = new();
    private readonly SourceBuffers _buffers;

    public SourceReader(MessageSource source)
    {
        _source = source;
        _buffers = new SourceBuffers(source);
    }

    private SpinWait _spinner;

    public async Task Start(CancellationToken stoppingToken)
    {
        _starter.Start();
        await Task.Yield();
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

                var key = _source.Partitioner.GetSourcePartitionKey(message);
                var buffer = _buffers.GetBuffer(key);
                buffer.Add(new MessageContext(message));
            }
        }
    }
}