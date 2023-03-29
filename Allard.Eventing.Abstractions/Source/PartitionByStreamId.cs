using Allard.Eventing.Abstractions.Model;

namespace Allard.Eventing.Abstractions.Source;

/// <summary>
/// Partition by the stream id.
/// (e.g. Kafka Topic)
/// </summary>
public class PartitionByStreamId : IMessagePartitioner
{
    public string GetPartitionKey(MessageEnvelope message)
    {
        return $"stream={message.Origin.StreamId};";
    }
}