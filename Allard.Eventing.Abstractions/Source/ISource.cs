namespace Allard.Eventing.Abstractions.Source;

public interface ISource
{
    Task<MessageEnvelope?> Get(CancellationToken cancellationToken = default);
}

public interface IPartitionedSource : ISource
{
    Task PausePartitions(IEnumerable<MessageOrigin> origins);
    Task ResumePartition(IEnumerable<MessageOrigin> origins);
}