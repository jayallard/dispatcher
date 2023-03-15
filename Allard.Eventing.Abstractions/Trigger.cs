namespace Allard.Eventing.Abstractions;

public class Trigger
{
    public ITriggerCondition Condition { get; }
    public ITriggerAction Action { get; }

    public Trigger(ITriggerCondition condition, ITriggerAction action)
    {
        Condition = condition;
        Action = action;
    }
}