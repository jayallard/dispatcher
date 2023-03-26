// See https://aka.ms/new-console-template for more information

using Allard.Eventing.Benchmarks;
using BenchmarkDotNet.Running;

Random _random = new();
var _testData = Enumerable
    .Range(0, 25)
    .Select(_ => Convert.ToChar(_random.Next(0, 26) + 65))
    .ToArray();


BenchmarkRunner.Run<ObjectCachingBenchmarks>();