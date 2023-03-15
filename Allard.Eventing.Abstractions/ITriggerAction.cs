namespace Allard.Eventing.Abstractions;

public interface ITriggerAction
{
    Task<MessageEnvelope?> Trigger(DispatchContext context);
}