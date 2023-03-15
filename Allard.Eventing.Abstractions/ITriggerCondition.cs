namespace Allard.Eventing.Abstractions;

public interface ITriggerCondition
{
    Task<TriggerState> GetReadiness(DispatchContext context);
}