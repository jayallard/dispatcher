namespace Allard.Eventing.Dispatcher;

public record MessageBufferTask(
    MessageBuffer Buffer,
    Task Runner,
    CancellationTokenSource CancellationTokenSource);