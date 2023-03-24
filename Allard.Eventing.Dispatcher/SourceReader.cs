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
        _key = new PrimaryPartitionKey("a", "b", "c");
        _buffer = _buffers.GetBuffer(_key);
    }

    private MessageBuffer _buffer;
    private PrimaryPartitionKey _key;

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
                    // await Task.Delay(FromMilliseconds(500), stoppingToken);
                    continue;
                }

                // var pk = new PrimaryPartitionKey(
                //     message.SourceId,
                //     message.Origin.StreamId,
                //     message.Origin.PartitionId);
                var context = new MessageContext(message);
                _buffer.Add(context);
            }
        }
    }
}