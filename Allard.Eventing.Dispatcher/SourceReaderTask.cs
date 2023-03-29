namespace Allard.Eventing.Dispatcher;

public record SourceReaderTask(
    IServiceProvider services,
    SourceReader Reader,
    Task Runner,
    CancellationTokenSource CancellationTokenSource);