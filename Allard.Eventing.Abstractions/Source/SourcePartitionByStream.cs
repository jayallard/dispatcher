namespace Allard.Eventing.Abstractions.Source;

/// <summary>
/// Partition by the stream id.
/// (e.g. Kafka Topic)
/// </summary>
public class SourcePartitionByStream : ISourcePartitioner
{
    public string GetSourcePartitionKey(MessageEnvelope message)
    {
        return $"stream={message.Origin.StreamId};";
    }
}