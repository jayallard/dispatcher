using System.Collections.Immutable;

namespace Allard.Eventing.Dispatcher;

public record SourceConfig(ImmutableArray<Type> SubscriberTypes);
