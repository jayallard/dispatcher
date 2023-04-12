using System.Collections.Immutable;

namespace Allard.Eventing.Dispatcher;

public record SourceConfig(string SourceId, ImmutableArray<Type> SubscriberTypes);
