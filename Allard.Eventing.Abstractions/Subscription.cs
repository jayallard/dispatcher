using System.Collections.Immutable;

namespace Allard.Eventing.Abstractions;

public class Subscription
{
    public Func<DispatchContext, Task> Handler { get; }
    public ImmutableHashSet<string> MessageTypes { get; }
    public string Id { get; }
    public ImmutableArray<Trigger> Triggers { get; }
    public IScopeLifetime ScopeLifetime { get; }

    public Subscription(
        string id,
        Func<DispatchContext, Task> handler,
        IScopeLifetime lifetime,
        IEnumerable<string> messageTypes,
        IEnumerable<Trigger> triggers)
    {
        Id = id;
        Handler = handler;
        MessageTypes = messageTypes.ToImmutableHashSet();
        Triggers = triggers.ToImmutableArray();
        ScopeLifetime = lifetime;
        if (!MessageTypes.Any())
        {
            throw new InvalidOperationException("Subscription must be fore at least one message type");
        }
    }
}