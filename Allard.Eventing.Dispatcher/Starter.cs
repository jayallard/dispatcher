namespace Allard.Eventing.Dispatcher;

public class Starter
{
    private int _isStarted;

    public void Start()
    {
        if (Interlocked.CompareExchange(ref _isStarted, 1, 0) == 1)
        {
            throw new InvalidOperationException("The buffer is already started");
        }
    }
}