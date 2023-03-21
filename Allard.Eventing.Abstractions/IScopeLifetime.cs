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

    public ScopeMaxCountOrDuration( int maxCount, TimeSpan maxDuration)
    {
        MaxCount = maxCount;
        MaxDuration = maxDuration;
    }

    public ScopeStatus CheckStatus(DispatchContext context)
    {
        var result = context.MessageCount >= MaxCount
        || context.Started.Elapsed() >= MaxDuration;
        return new ScopeStatus(result);
    }
}


public record ScopeStatus(bool IsComplete);