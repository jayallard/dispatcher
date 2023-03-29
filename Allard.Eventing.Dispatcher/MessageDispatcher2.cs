using System.Collections.Immutable;
using Allard.Eventing.Abstractions.Source;
using Microsoft.Extensions.DependencyInjection;

namespace Allard.Eventing.Dispatcher;

public class MessageDispatcher2
{
    private readonly DispatcherSetup _setup;
    private SourceReaderTask[]? _readers;

    public MessageDispatcher2(DispatcherSetup setup)
    {
        _setup = setup;
    }

    private int _isStarted;

    public async Task Start(CancellationToken stoppingToken)
    {
        Starter.EnsureCanStart(ref _isStarted);
        _readers = _setup.Sources
            .Select(s =>
            {
                var subscriberTypes = new SourceConfig(s.SubscriberTypes.ToImmutableArray());
                s.Services.AddSingleton(subscriberTypes);
                var services = s.Services.BuildServiceProvider();
                var cancellation = new CancellationTokenSource();
                var source = services.GetRequiredService<ISource>();
                var reader = services.GetRequiredService<SourceReader>();
                var runner = reader.Start(new MessageSource(s.Id, source), cancellation.Token);
                return new SourceReaderTask(services, reader, runner, cancellation);
            })
            .ToArray();

        await Task.WhenAll(_readers.Select(s => s.Runner));
    }
}