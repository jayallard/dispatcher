namespace Allard.Eventing.Dispatcher;

public record SourceReaderTask(
    SourceReader Reader,
    Task Runner,
    CancellationTokenSource CancellationTokenSource);