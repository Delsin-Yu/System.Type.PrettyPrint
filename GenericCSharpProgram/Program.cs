using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace GenericCSharpProgram;

public class Program
{
    private static void Main(string[] args)
    {
        BenchmarkRunner.Run<Algorithms>(DefaultConfig.Instance.WithOptions(ConfigOptions.DisableOptimizationsValidator));
    }
}