namespace Allard.Eventing.Abstractions;

public record MessageOrigin(string StreamId, string PartitionId, long SequenceNumber);
