namespace Allard.Eventing.Dispatcher;

public static class Starter
{
    public static void EnsureCanStart(ref int isStarted)
    {
        // returns the original value. if the original value was 1, then the
        // service is already started.
        if (Interlocked.CompareExchange(ref isStarted, 1, 0) == 1)
        {
            throw new InvalidOperationException("The buffer is already started");
        }
    }
}