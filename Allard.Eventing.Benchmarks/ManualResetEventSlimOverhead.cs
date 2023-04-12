using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;

namespace Allard.Eventing.Benchmarks;

public class ManualResetEventSlimOverhead
{
    private const int Count = 1_000_000;
    
    [Benchmark]
    public void WithoutEvent()
    {
        var queue = new ConcurrentQueue<int>();
        for (var i = 0; i < Count; i++)
        {
            queue.Enqueue(i);
        }
    }

    [Benchmark]
    public void WithEvent()
    {
        var evt = new ManualResetEventSlim(true);
        var queue = new ConcurrentQueue<int>();
        for (var i = 0; i < Count; i++)
        {
            evt.Wait();
            queue.Enqueue(i);
        }
    }
}