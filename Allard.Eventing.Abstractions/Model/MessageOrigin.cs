namespace Allard.Eventing.Abstractions.Model;

public record MessageOrigin(string StreamId, string PartitionId, long SequenceNumber);
