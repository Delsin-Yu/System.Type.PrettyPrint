using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace GenericCSharpProgram;

[
    MemoryDiagnoser,
    SimpleJob(RuntimeMoniker.Net60, invocationCount: 1000),
    SimpleJob(RuntimeMoniker.Net70, invocationCount: 1000),
    SimpleJob(RuntimeMoniker.Net80, invocationCount: 1000),
]
public class Program
{
    private static void Main(string[] args)
    {
        BenchmarkRunner.Run<Program>(DefaultConfig.Instance.WithOptions(ConfigOptions.DisableOptimizationsValidator));
    }
}