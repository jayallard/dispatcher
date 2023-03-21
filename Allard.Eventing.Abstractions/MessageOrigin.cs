namespace Allard.Eventing.Abstractions;

// TODO: if any messages of the same source+partition are sent to different subscribers,
// then a slow subscriber might block the other subscribers


public record MessageOrigin(string StreamId, string PartitionId, long SequenceNumber);
