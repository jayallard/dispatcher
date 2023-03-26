using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public class SourceReader
{
    private readonly ISource2 _source;
    private readonly Buffers _buffers;
    private readonly Starter _starter = new();

    public SourceReader(
        ISource2 source,
        Buffers buffers)
    {
        _source = source;
        _buffers = buffers;
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
                var message = await _source.Get(stoppingToken);
                if (message == null)
                {
                    _spinner.SpinOnce();
                    continue;
                }

                var pk = new PrimaryPartitionKey(
                    message.SourceId,
                    message.Origin.StreamId,
                    message.Origin.PartitionId);
                _buffers.GetBuffer(pk).Add(new MessageContext(message));
            }
        }
    }
}