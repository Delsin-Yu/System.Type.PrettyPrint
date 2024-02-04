using System.Runtime.CompilerServices;

namespace GenericCSharpProgram.Test;

public class Tests
{
    [Test] public static void SimpleType_JSDY_Test() => SimpleTypeValidate(Algorithms.ConstructTypeName_JSDY);
    [Test] public static void ModerateType_JSDY_Test() => ModerateTypeValidate(Algorithms.ConstructTypeName_JSDY);
    [Test] public static void CrazyType_JSDY_Test() => CrazyTypeValidate(Algorithms.ConstructTypeName_JSDY);
    [Test] public static void SimpleType_JSDY_OPT_Test() => SimpleTypeValidate(Algorithms.ConstructTypeName_JSDY_OPT);
    [Test] public static void ModerateType_JSDY_OPT_Test() => ModerateTypeValidate(Algorithms.ConstructTypeName_JSDY_OPT);
    [Test] public static void CrazyType_JSDY_OPT_Test() => CrazyTypeValidate(Algorithms.ConstructTypeName_JSDY_OPT);

    [Test] public static void SimpleType_JSDY_OPT2_Test() => SimpleTypeValidate(Algorithms.ConstructTypeName_JSDY_OPT2);
    [Test] public static void ModerateType_JSDY_OPT2_Test() => ModerateTypeValidate(Algorithms.ConstructTypeName_JSDY_OPT2);
    [Test] public static void CrazyType_JSDY_OPT2_Test() => CrazyTypeValidate(Algorithms.ConstructTypeName_JSDY_OPT2);

    [Test] public static void SimpleType_JSDY_OPT3_Test() => SimpleTypeValidate(Algorithms.ConstructTypeName_JSDY_OPT3);
    [Test] public static void ModerateType_JSDY_OPT3_Test() => ModerateTypeValidate(Algorithms.ConstructTypeName_JSDY_OPT3);
    [Test] public static void CrazyType_JSDY_OPT3_Test() => CrazyTypeValidate(Algorithms.ConstructTypeName_JSDY_OPT3);

    [Test] public static void SimpleType_TML_Test() => SimpleTypeValidate(Algorithms.ConstructTypeName_TML);
    [Test] public static void ModerateType_TML_Test() => ModerateTypeValidate(Algorithms.ConstructTypeName_TML);
    [Test] public static void CrazyType_TML_Test() => CrazyTypeValidate(Algorithms.ConstructTypeName_TML);

    [Test] public static void SimpleType_ZERXZ_Test() => SimpleTypeValidate(Algorithms.ConstructTypeName_ZERXZ);
    [Test] public static void ModerateType_ZERXZ_Test() => ModerateTypeValidate(Algorithms.ConstructTypeName_ZERXZ);
    [Test] public static void CrazyType_ZERXZ_Test() => CrazyTypeValidate(Algorithms.ConstructTypeName_ZERXZ);
    [Test] public static void SimpleType_ZERXZ_OPT_Test() => SimpleTypeValidate(Algorithms.ConstructTypeName_ZERXZ_OPT);
    [Test] public static void ModerateType_ZERXZ_OPT_Test() => ModerateTypeValidate(Algorithms.ConstructTypeName_ZERXZ_OPT);
    [Test] public static void CrazyType_ZERXZ_OPT_Test() => CrazyTypeValidate(Algorithms.ConstructTypeName_ZERXZ_OPT);


    private static void SimpleTypeValidate(Func<Type, string> call)
    {
        Validate(call(typeof(int?)));
        Validate(call(typeof((int, int))));
        Validate(call(typeof(List<int>)));
        Validate(call(typeof(Dictionary<int, int>)));
        Validate(call(typeof(Action<int, float, double, bool>)));
    }

    private static void ModerateTypeValidate(Func<Type, string> call)
    {
        Validate(call(typeof(int?[])));
        Validate(call(typeof(List<List<(List<int>, List<int[]>)>>)));
        Validate(call(typeof(List<int[,,][,,,]>)));
        Validate(call(typeof(List<int?[,][,,][,,,]>)));
        Validate(call(typeof((int, int, int, int, (int, int, int), (int, int), (int, int, int, int, int, int)))));
    }

    private static void CrazyTypeValidate(Func<Type, string> call)
    {
        Validate(call(typeof(List<Dictionary<List<int>, List<int?[,]>>>)));
        Validate(call(typeof(List<Func<List<Dictionary<List<int>, List<int[,,]>>>, List<(List<int>, List<int[]>)>>>)));
        Validate(call(typeof(List<List<List<Dictionary<List<Dictionary<int, IList<Dictionary<int, bool>>>>, int>>>>)));
        Validate(call(typeof(List<(int, float, (List<Func<List<Dictionary<List<int?>, List<int?[,,]>>>, List<(List<int>, List<int[]>)?>>>, List<int[,,][,][,,,,]>))?>)));
    }

    private static void Validate(string match, [CallerArgumentExpression(nameof(match))] string sourceType = "")
    {
        Assert.That(sourceType[12..^2], Is.EqualTo(match));
    }
}