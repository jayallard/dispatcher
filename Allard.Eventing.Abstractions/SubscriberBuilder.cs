namespace Allard.Eventing.Abstractions;

public class SubscriberBuilder
{
    public static SubscriberBuilder CreateSubscription(string id)
    {
        return new SubscriberBuilder(id);
    }

    public Func<DispatchContext, Task> Handler { get; set; }
    public Func<MessageEnvelope, bool> Condition { get; set; }
    public string Id { get; set; }
    public IScopeLifetime ScopeLifetime { get; set; } = new ScopePerMessage();
    // private HashSet<string> MessageTypes { get; } = new();
    public SubscriberBuilder SetCondition(Func<MessageEnvelope, bool> condition)
    {
        Condition = condition;
        return this;
    }

    private SubscriberBuilder(string id)
    {
        Id = id;
    }

    public SubscriberBuilder SetHandler(Func<DispatchContext, Task> handler)
    {
        Handler = handler;
        return this;
    }

    public SubscriberBuilder SetId(string id)
    {
        Id = id;
        return this;
    }

    public SubscriberBuilder SetScopeLifetime(IScopeLifetime lifetime)
    {
        ScopeLifetime = lifetime;
        return this;
    }
    
    public Subscriber Build()
    {
        return new Subscriber(
            id: Id,
            handler: Handler,
            condition: Condition,
            lifetime: ScopeLifetime);
    }
}