namespace Allard.Eventing.Abstractions;

public record SourcePartitionKey(
    string SourceId, 
    string StreamId, 
    string PartitionId);