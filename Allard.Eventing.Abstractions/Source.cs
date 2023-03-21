namespace Allard.Eventing.Abstractions;

public record Source(string SourceId, ISource MessageSource);