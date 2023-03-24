namespace Allard.Eventing.Dispatcher;

public record SourceTask(
    Task Runner,
    CancellationTokenSource CancellationTokenSource);