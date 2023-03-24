using Allard.Eventing.Abstractions;

namespace Allard.Eventing.Dispatcher;

public class MessageDispatcher2
{
    private readonly ISource2[] _sources;
    private readonly Subscriber[] _subscribers;
    private readonly Buffers _buffers;
    private SourceTask[]? _sourceTasks;

    public MessageDispatcher2(
        IEnumerable<ISource2> sources,
        IEnumerable<Subscriber> subscribers)
    {
        _sources = sources.ToArray();
        _subscribers = subscribers.ToArray();
        _buffers = new Buffers(Process);
    }

    private Task Process(MessageContext mc)
    {
        // var subscribers = _subscribers
        //     .Where(s => s.Condition(mc.Message))
        //     .ToArray();
        foreach (var sub in _subscribers)
        {
            var dc = new DispatchContext();
            dc.SetCurrent(mc);
            sub.Handler(dc);
        }

        return Task.CompletedTask;
    }

    private readonly Starter _starter = new();

    public async Task Start(CancellationToken stoppingToken)
    {
        _starter.Start();
        await Task.Yield();

        _sourceTasks = _sources
            .Select(s =>
            {
                var reader = new SourceReader(s, _buffers);
                var cancellation = new CancellationTokenSource();
                var runner = reader.Start(cancellation.Token);
                return new SourceTask(runner, cancellation);
            })
            .ToArray();

        await Task.WhenAll(_sourceTasks.Select(s => s.Runner));
    }
}