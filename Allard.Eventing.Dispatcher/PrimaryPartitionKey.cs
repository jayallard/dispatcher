namespace Allard.Eventing.Dispatcher;

public record PrimaryPartitionKey(
    string SourceId, 
    string StreamId, 
    string PartitionId);