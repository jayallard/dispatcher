using Allard.Eventing.Abstractions.Model;

namespace Allard.Eventing.Abstractions.Source;

public class PartitionByStreamIdPartitionId : IMessagePartitioner
{
    public string GetPartitionKey(MessageEnvelope message)
    {
        return $"stream={message.Origin.StreamId};partition={message.Origin.PartitionId}";
    }
}