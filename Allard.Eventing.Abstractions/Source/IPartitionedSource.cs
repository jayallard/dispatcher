using Allard.Eventing.Abstractions.Model;

namespace Allard.Eventing.Abstractions.Source;

public interface IPartitionedSource : ISource
{
    Task PausePartitions(IEnumerable<MessageOrigin> origins);
    Task ResumePartition(IEnumerable<MessageOrigin> origins);
}