namespace Allard.Eventing.Abstractions;

public class TimeOrCountTrigger : ITriggerCondition
{
    public int MaxCount { get; }
    public TimeSpan MaxDuration { get; }

    public TimeOrCountTrigger(TimeSpan maxDuration, int maxCount)
    {
        MaxDuration = maxDuration;
        MaxCount = maxCount;
    }
    
    public Task<TriggerState> GetReadiness(DispatchContext context)
    {
        var isSatisfied =
            context.MessageCount >= MaxCount
            || (context.MessageCount > 0 && context.Started.Elapsed() >= MaxDuration);
        return Task.FromResult(new TriggerState(IsReady: isSatisfied));
    }
}