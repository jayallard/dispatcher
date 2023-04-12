// See https://aka.ms/new-console-template for more information

using Allard.Eventing.Benchmarks;
using BenchmarkDotNet.Running;
BenchmarkRunner.Run<ManualResetEventSlimOverhead>();