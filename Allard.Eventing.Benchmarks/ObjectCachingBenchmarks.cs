using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;

namespace Allard.Eventing.Benchmarks;

[MemoryDiagnoser]
public class ObjectCachingBenchmarks
{
    private static Random _random = new();

    private string[] _testData = Enumerable
        .Range(0, 1_000_000)
        .Select(_ => Convert.ToChar(_random.Next(0, 26) + 65).ToString())
        .ToArray();

    [Benchmark]
    public void Instantiate()
    {
        var items = new List<FakeKey>();
        foreach (var k in _testData)
        {
            items.Add(new FakeKey(k, k, k));
        }
    }

    [Benchmark]
    public void Cache()
    {
        var items = new List<FakeKey>();
        var cache = new ConcurrentDictionary<string, Lazy<FakeKey>>();
        
        foreach (var k in _testData)
        {
            var item = cache.GetOrAdd(k, x => new Lazy<FakeKey>(() =>  new FakeKey(x, x, x))).Value;
            items.Add(item);
        }
    }
}

public record FakeKey(string A, string B, string C);