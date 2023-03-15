namespace Allard.Eventing.Abstractions;

public record MessageOrigin(string Source, string Partition, long SequenceNumber);
