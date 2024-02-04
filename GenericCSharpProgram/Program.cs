using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

#nullable disable

namespace GenericCSharpProgram;

[
    MemoryDiagnoser,
    SimpleJob(RuntimeMoniker.Mono60),
    SimpleJob(RuntimeMoniker.Mono70),
    SimpleJob(RuntimeMoniker.Mono80),
    SimpleJob(RuntimeMoniker.Net60),
    SimpleJob(RuntimeMoniker.Net70),
    SimpleJob(RuntimeMoniker.Net80),
    SimpleJob(RuntimeMoniker.NativeAot60),
    SimpleJob(RuntimeMoniker.NativeAot70),
    SimpleJob(RuntimeMoniker.NativeAot80),
]
public partial class Program
{
    private static void Main(string[] args)
    {
        
        BenchmarkRunner.Run<Program>();
    }



 

  


    private static void SimpleType(Func<Type, string> call)
    {
        call(typeof(int?));
        call(typeof((int, int)));
        call(typeof(List<int>));
        call(typeof(Dictionary<int, int>));
        call(typeof(Action<int, float, double, bool>));
    }

    private static void ModerateType(Func<Type, string> call)
    {
        call(typeof(int?[]));
        call(typeof(List<List<(List<int>, List<int[]>)>>));
        call(typeof(List<int[,,][,,,]>));
        call(typeof(List<int?[,][,,][,,,]>));
        call((typeof((int, int, int, int, (int, int, int), (int, int), (int, int, int, int, int, int)))));
    }

    private static void CrazyType(Func<Type, string> call)
    {
        call(typeof(List<Dictionary<List<int>, List<int?[,]>>>));
        call(typeof(List<Func<List<Dictionary<List<int>, List<int[,,]>>>, List<(List<int>, List<int[]>)>>>));
        call(typeof(List<List<List<Dictionary<List<Dictionary<int, IList<Dictionary<int, bool>>>>, int>>>>));
        call(typeof(List<(int, float, (List<Func<List<Dictionary<List<int?>, List<int?[,,]>>>, List<(List<int>, List<int[]>)?>>>, List<int[,,][,][,,,,]>))?>));
    }
}