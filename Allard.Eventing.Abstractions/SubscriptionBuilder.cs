namespace Allard.Eventing.Abstractions;

public class SubscriptionBuilder
{
    public static SubscriptionBuilder CreateSubscription(string id)
    {
        return new SubscriptionBuilder(id);
    }

    public Func<DispatchContext, Task> Handler { get; set; }

    public List<Trigger> Triggers { get; } = new();
    public string Id { get; set; }

    public IScopeLifetime ScopeLifetime { get; set; } = new ScopePerMessage();
    private HashSet<string> MessageTypes { get; } = new();

    private SubscriptionBuilder(string id)
    {
        Id = id;
    }

    public SubscriptionBuilder SetHandler(Func<DispatchContext, Task> handler)
    {
        Handler = handler;
        return this;
    }

    public SubscriptionBuilder AddMessageType(string messageType)
    {
        MessageTypes.Add(messageType);
        return this;
    }

    public SubscriptionBuilder SetId(string id)
    {
        Id = id;
        return this;
    }

    public SubscriptionBuilder AddTrigger(ITriggerCondition condition, ITriggerAction action)
    {
        Triggers.Add(new Trigger(condition, action));
        return this;
    }

    public SubscriptionBuilder SetScopeLifetime(IScopeLifetime lifetime)
    {
        ScopeLifetime = lifetime;
        return this;
    }
    
    public Subscription Build()
    {
        return new Subscription(
            id: Id,
            handler: Handler,
            messageTypes: MessageTypes,
            lifetime: ScopeLifetime,
            triggers: Triggers);
    }
}