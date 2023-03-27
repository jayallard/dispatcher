namespace Allard.Eventing.Abstractions;

public class SourcePartitionByStreamPartition : ISourcePartitioner
{
    public string GetSourcePartitionKey(MessageEnvelope message)
    {
        return $"stream={message.Origin.StreamId};partition={message.Origin.PartitionId}";
    }
}