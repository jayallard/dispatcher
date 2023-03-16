namespace Allard.Eventing.Abstractions;

public interface IScopeLifetime
{
    ScopeStatus CheckStatus(DispatchContext context);
}

public class ScopeAlways : IScopeLifetime
{
    public ScopeStatus CheckStatus(DispatchContext context)
    {
        return new ScopeStatus(false);
    }
}

public class ScopePerMessage : IScopeLifetime
{
    public ScopeStatus CheckStatus(DispatchContext context)
    {
        return new ScopeStatus(true);
    }
}

public class ScopeMaxCountOrDuration : IScopeLifetime
{
    public TimeSpan MaxDuration { get; }
    public int MaxCount { get; }

    public ScopeMaxCountOrDuration(TimeSpan maxDuration, int maxCount)
    {
        MaxDuration = maxDuration;
        MaxCount = maxCount;
    }

    public ScopeStatus CheckStatus(DispatchContext context)
    {
        var result = context.Started.Elapsed() >= MaxDuration
                     || context.MessageCount >= MaxCount;
        return new ScopeStatus(result);
    }
}


public record ScopeStatus(bool IsComplete);