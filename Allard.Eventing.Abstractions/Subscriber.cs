using System.Collections.Immutable;

namespace Allard.Eventing.Abstractions;

public class Subscriber
{
    public Func<DispatchContext, Task> Handler { get; }
    public string Id { get; }
    // public ImmutableArray<Trigger> Triggers { get; }
    public IScopeLifetime ScopeLifetime { get; }
    public Func<MessageEnvelope, bool> Condition { get; }

    public Subscriber(
        string id,
        Func<DispatchContext, Task> handler,
        Func<MessageEnvelope, bool> condition,
        IScopeLifetime lifetime)
    {
        Id = id;
        Handler = handler;
        // Triggers = triggers.ToImmutableArray();
        ScopeLifetime = lifetime;
        Condition = condition;
    }
}