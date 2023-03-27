namespace Allard.Eventing.Abstractions;

public interface ISourcePartitioner
{
    public string GetSourcePartitionKey(MessageEnvelope messageContext);
}