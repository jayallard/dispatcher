namespace Allard.Eventing.Dispatcher;

public class MessageDispatcher
{
    private readonly MessageSource[] _sources;
    private SourceReaderTask[]? _readers;

    public MessageDispatcher(
        IEnumerable<MessageSource> sources)
    {
        _sources = sources.ToArray();
    }

    private int _isStarted;

    public async Task Start(CancellationToken stoppingToken)
    {
        Starter.EnsureCanStart(ref _isStarted);
        await Task.Yield();

        _readers = _sources
            .Select(s =>
            {
                var reader = new SourceReader(s);
                var cancellation = new CancellationTokenSource();
                var runner = reader.Start(cancellation.Token);
                return new SourceReaderTask(reader, runner, cancellation);
            })
            .ToArray();

        await Task.WhenAll(_readers.Select(s => s.Runner));
    }
}