using BenchmarkDotNet.Running;
using NeoBenchmark;

var summary = BenchmarkRunner.Run<Benchmarks>();

Console.ReadLine();