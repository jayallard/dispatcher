namespace Allard.Eventing.Abstractions.Source;

public interface ISourcePartitioner
{
    public string GetSourcePartitionKey(MessageEnvelope messageContext);
}