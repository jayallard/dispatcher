namespace Allard.Eventing.Abstractions;

public interface ISource
{
    Task Start(Func<MessageEnvelope, Task> writer, CancellationToken stoppingToken);
}