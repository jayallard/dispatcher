using Allard.Eventing.Abstractions.Model;

namespace Allard.Eventing.Abstractions;

public interface ISourcePartitionTracker
{
    void Start(MessageOrigin origin);
    void Complete(MessageOrigin origin);

    void Clear(IEnumerable<MessageOrigin> origin);

    IEnumerable<MessageOrigin> GetComplete();
}