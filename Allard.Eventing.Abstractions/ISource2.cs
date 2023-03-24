namespace Allard.Eventing.Abstractions;

public interface ISource2
{
    Task<MessageEnvelope?> Get(CancellationToken cancellationToken = default);
}

public interface IPartitionedSource : ISource2
{
    Task PausePartitions(IEnumerable<MessageOrigin> origins);
    Task ResumePartition(IEnumerable<MessageOrigin> origins);
}