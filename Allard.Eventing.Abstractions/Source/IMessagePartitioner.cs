using Allard.Eventing.Abstractions.Model;

namespace Allard.Eventing.Abstractions.Source;

public interface IMessagePartitioner
{
    public string GetPartitionKey(MessageEnvelope messageContext);
}