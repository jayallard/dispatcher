namespace Allard.Eventing.Dispatcher;

public static class Starter
{
    /// <summary>
    /// If the value is 0, changes it to 1.
    /// If the value is 1, throws an exception.
    /// Used by classes with START methods, to assure that
    /// START is only called once.
    /// </summary>
    /// <param name="isStarted"></param>
    /// <exception cref="InvalidOperationException"></exception>
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