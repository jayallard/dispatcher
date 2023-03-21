namespace Allard.Eventing.Dispatcher;

public record PartitionKey(string StreamId, string PartitionId);