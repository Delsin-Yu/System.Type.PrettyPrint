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

    [Benchmark] public void SimpleType_TML() => SimpleType(ConstructTypeName_TML);
    [Benchmark] public void ModerateType_TML() => ModerateType(ConstructTypeName_TML);
    [Benchmark] public void CrazyType_TML() => CrazyType(ConstructTypeName_TML);

    [Benchmark] public void SimpleType_JSDY() => SimpleType(ConstructTypeName_JSDY);
    [Benchmark] public void ModerateType_JSDY() => ModerateType(ConstructTypeName_JSDY);
    [Benchmark] public void CrazyType_JSDY() => CrazyType(ConstructTypeName_JSDY);

    [Benchmark] public void SimpleType_JSDY_OPT() => SimpleType(ConstructTypeName_JSDY_OPT);
    [Benchmark] public void ModerateType_JSDY_OPT() => ModerateType(ConstructTypeName_JSDY_OPT);
    [Benchmark] public void CrazyType_JSDY_OPT() => CrazyType(ConstructTypeName_JSDY_OPT);

    [Benchmark] public void SimpleType_JSDY_OPT2() => SimpleType(ConstructTypeName_JSDY_OPT2);
    [Benchmark] public void ModerateType_JSDY_OPT2() => ModerateType(ConstructTypeName_JSDY_OPT2);
    [Benchmark] public void CrazyType_JSDY_OPT2() => CrazyType(ConstructTypeName_JSDY_OPT2);

    [Benchmark] public void SimpleType_JSDY_OPT3() => SimpleType(ConstructTypeName_JSDY_OPT3);
    [Benchmark] public void ModerateType_JSDY_OPT3() => ModerateType(ConstructTypeName_JSDY_OPT3);
    [Benchmark] public void CrazyType_JSDY_OPT3() => CrazyType(ConstructTypeName_JSDY_OPT3);

    [Benchmark] public void SimpleType_ZERXZ() => SimpleType(ConstructTypeName_ZERXZ);
    [Benchmark] public void ModerateType_ZERXZ() => ModerateType(ConstructTypeName_ZERXZ);
    [Benchmark] public void CrazyType_ZERXZ() => CrazyType(ConstructTypeName_ZERXZ);

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