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
public class Program
{
    private static void Main(string[] args)
    {
        
        BenchmarkRunner.Run<Program>();
    }

    [Benchmark] public void SimpleType_TML() => SimpleType(ConstructTypeName_TML);
    [Benchmark] public void ModerateType_TML() => ModerateType(ConstructTypeName_TML);
    [Benchmark] public void CrazyType_TML() => ModerateType(ConstructTypeName_TML);

    [Benchmark] public void SimpleType_JSDY() => SimpleType(ConstructTypeName_JSDY);
    [Benchmark] public void ModerateType_JSDY() => ModerateType(ConstructTypeName_JSDY);
    [Benchmark] public void CrazyType_JSDY() => ModerateType(ConstructTypeName_JSDY);

    [Benchmark] public void SimpleType_JSDY_OPT() => SimpleType(ConstructTypeName_JSDY_OPT);
    [Benchmark] public void ModerateType_JSDY_OPT() => ModerateType(ConstructTypeName_JSDY_OPT);
    [Benchmark] public void CrazyType_JSDY_OPT() => ModerateType(ConstructTypeName_JSDY_OPT);

    [Benchmark] public void SimpleType_JSDY_OPT2() => SimpleType(ConstructTypeName_JSDY_OPT2);
    [Benchmark] public void ModerateType_JSDY_OPT2() => ModerateType(ConstructTypeName_JSDY_OPT2);
    [Benchmark] public void CrazyType_JSDY_OPT2() => ModerateType(ConstructTypeName_JSDY_OPT2);

    [Benchmark] public void SimpleType_JSDY_OPT3() => SimpleType(ConstructTypeName_JSDY_OPT3);
    [Benchmark] public void ModerateType_JSDY_OPT3() => ModerateType(ConstructTypeName_JSDY_OPT3);
    [Benchmark] public void CrazyType_JSDY_OPT3() => ModerateType(ConstructTypeName_JSDY_OPT3);

    [Benchmark] public void SimpleType_ZERXZ() => SimpleType(ConstructTypeName_ZERXZ);
    [Benchmark] public void ModerateType_ZERXZ() => ModerateType(ConstructTypeName_ZERXZ);
    [Benchmark] public void CrazyType_ZERXZ() => ModerateType(ConstructTypeName_ZERXZ);

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

    private static string ConstructTypeName_TML(Type type)
    {
        var stringBuilder = new StringBuilder();
        GetTypeNameByType(stringBuilder, type);
        return stringBuilder.ToString();

        static void GetTypeNameByType(StringBuilder sb, Type type)
        {
            if (type.IsArray)
            {
                var innerType = type;
                do
                {
                    innerType = innerType.GetElementType()!;
                } while (innerType.IsArray);

                GetTypeNameByType(sb, innerType);

                innerType = type;
                do
                {
                    var arrayTypeFullName = innerType.FullName!;
                    var arrayBracketStartIndex = arrayTypeFullName.LastIndexOf('[');
                    var arrayBracketEndIndex = arrayTypeFullName.LastIndexOf(']');
                    var arrayDefinition = arrayTypeFullName.AsSpan()[arrayBracketStartIndex..(arrayBracketEndIndex + 1)];
                    sb.Append(arrayDefinition);
                    innerType = innerType.GetElementType()!;
                } while (innerType.IsArray);

                return;
            }

            if (type.IsGenericType)
            {
                ReadOnlySpan<char> prependToken;
                char? prependSymbol;
                char appendSymbol;
                if (type.FullName!.StartsWith("System.Nullable"))
                {
                    prependToken = ReadOnlySpan<char>.Empty;
                    prependSymbol = null;
                    appendSymbol = '?';
                }
                else if (type.FullName.StartsWith("System.ValueTuple"))
                {
                    prependToken = ReadOnlySpan<char>.Empty;
                    prependSymbol = '(';
                    appendSymbol = ')';
                }
                else
                {
                    prependToken = TrimGenericMarker(type.Name);
                    prependSymbol = '<';
                    appendSymbol = '>';
                }

                sb.Append(prependToken);
                if (prependSymbol.HasValue) sb.Append(prependSymbol.Value);
                var genericArgs = type.GenericTypeArguments;
                for (int i = 0; i < genericArgs.Length; i += 1)
                {
                    GetTypeNameByType(sb, genericArgs[i]);
                    if (i < genericArgs.Length - 1)
                    {
                        sb.Append(',').Append(' ');
                    }
                }

                sb.Append(appendSymbol);
                return;
            }

            sb.Append(type.FullName switch
            {
                "System.SByte" => "sbyte",
                "System.Byte" => "byte",
                "System.Int16" => "short",
                "System.UInt16" => "ushort",
                "System.Int32" => "int",
                "System.UInt32" => "uint",
                "System.Int64" => "long",
                "System.UInt64" => "ulong",
                "System.IntPtr" => "nint",
                "System.UIntPtr" => "nuint",
                "System.Single" => "float",
                "System.Double" => "double",
                "System.Decimal" => "decimal",
                "System.Boolean" => "bool",
                "System.Char" => "char",
                "System.String" => "string",
                "System.Object" => "object",
                _ => type.Name
            });
            return;
        }

        static ReadOnlySpan<char> TrimGenericMarker(ReadOnlySpan<char> name)
        {
            var lastAccentIndex = name.LastIndexOf('`');
            return name[..lastAccentIndex];
        }
    }

    private static string ConstructTypeName_JSDY(Type type)
    {
        if (type.IsArray || type.IsGenericType)
        {
            var sb = new StringBuilder();
            BuildType(sb, type);
            return sb.ToString();
        }

        return GetSimpleTypeName(type);

        static void BuildType(StringBuilder sb, Type type)
        {
            if (type.IsArray)
            {
                BuildArray(sb, type);
            }
            else if (type.IsGenericType)
            {
                BuildGeneric(sb, type);
            }
            else
            {
                sb.Append(GetSimpleTypeName(type));
            }
        }

        static void BuildArray(StringBuilder sb, Type type)
        {
            var elementType = type;
            while (elementType.IsArray)
            {
                elementType = elementType.GetElementType()!;
            }

            BuildType(sb, elementType);
            BuildArrayRecursive(sb, type);

            static void BuildArrayRecursive(StringBuilder sb, Type type)
            {
                BuildArrayBrackets(sb, type.GetArrayRank());
                var elementType = type.GetElementType()!;
                if (elementType.IsArray)
                {
                    BuildArrayRecursive(sb, elementType);
                }
            }

            static void BuildArrayBrackets(StringBuilder sb, int rank)
            {
                sb.Append('[');
                for (int i = 0; i < rank - 1; i++)
                {
                    sb.Append(',');
                }

                sb.Append(']');
            }
        }

        static void BuildGeneric(StringBuilder sb, Type type)
        {
            var genericDefinitionFullName = type.GetGenericTypeDefinition().FullName ?? string.Empty;
            var genericArgs = type.GenericTypeArguments;

            //Nullable
            if (genericDefinitionFullName == "System.Nullable`1")
            {
                BuildType(sb, genericArgs[0]);
                sb.Append('?');
                return;
            }

            //ValueTuple
            if (genericDefinitionFullName.StartsWith("System.ValueTuple`"))
            {
                sb.Append('(');
                BuildParamTypes(sb, genericArgs);
                sb.Append(')');
                return;
            }

            //normal generic
            sb.Append(type.Name[..type.Name.LastIndexOf('`')]);
            sb.Append('<');
            BuildParamTypes(sb, genericArgs);
            sb.Append('>');

            static void BuildParamTypes(StringBuilder sb, Type[] genericArgs)
            {
                for (int i = 0; i < genericArgs.Length; i += 1)
                {
                    BuildType(sb, genericArgs[i]);
                    if (i < genericArgs.Length - 1)
                    {
                        sb.Append(',').Append(' ');
                    }
                }
            }
        }

        static string GetSimpleTypeName(Type type)
        {
            return type.FullName switch
            {
                "System.SByte" => "sbyte",
                "System.Byte" => "byte",
                "System.Int16" => "short",
                "System.UInt16" => "ushort",
                "System.Int32" => "int",
                "System.UInt32" => "uint",
                "System.Int64" => "long",
                "System.UInt64" => "ulong",
                "System.IntPtr" => "nint",
                "System.UIntPtr" => "nuint",
                "System.Single" => "float",
                "System.Double" => "double",
                "System.Decimal" => "decimal",
                "System.Boolean" => "bool",
                "System.Char" => "char",
                "System.String" => "string",
                "System.Object" => "object",
                _ => type.Name
            };
        }
    }

    [ThreadStatic] private static StringBuilder _stringBuilder;

    private static string ConstructTypeName_JSDY_OPT(Type type)
    {
        if (!type.IsArray && !type.IsGenericType)
        {
            return GetSimpleTypeName(type);
        }

        _stringBuilder ??= new StringBuilder();
        var sb = _stringBuilder;
        AppendType(sb, type);
        var result = sb.ToString();
        _stringBuilder.Clear();
        return result;

        static void AppendType(StringBuilder sb, Type type)
        {
            if (type.IsArray)
            {
                AppendArray(sb, type);
            }
            else if (type.IsGenericType)
            {
                AppendGeneric(sb, type);
            }
            else
            {
                sb.Append(GetSimpleTypeName(type));
            }
        }

        static void AppendArray(StringBuilder sb, Type type)
        {
            //append inner most non-array element
            var elementType = type.GetElementType()!;
            while (elementType.IsArray)
            {
                elementType = elementType.GetElementType()!;
            }

            AppendType(sb, elementType);
            //append brackets
            AppendArrayRecursive(sb, type);

            static void AppendArrayRecursive(StringBuilder sb, Type type)
            {
                //append bracket with rank
                var rank = type.GetArrayRank();
                sb.Append('[');
                for (int i = 0; i < rank - 1; i++)
                {
                    sb.Append(',');
                }

                sb.Append(']');
                //recursive call
                var elementType = type.GetElementType()!;
                if (elementType.IsArray)
                {
                    AppendArrayRecursive(sb, elementType);
                }
            }
        }

        static void AppendGeneric(StringBuilder sb, Type type)
        {
            var genericDefinitionFullName = type.GetGenericTypeDefinition().FullName ?? string.Empty;
            var genericArgs = type.GenericTypeArguments;

            //Nullable
            if (genericDefinitionFullName == "System.Nullable`1")
            {
                AppendType(sb, genericArgs[0]);
                sb.Append('?');
                return;
            }

            //ValueTuple
            if (genericDefinitionFullName.StartsWith("System.ValueTuple`"))
            {
                sb.Append('(');
                AppendParamTypes(sb, genericArgs);
                sb.Append(')');
                return;
            }

            //normal generic
            var typeName = type.Name.AsSpan();
            sb.Append(typeName[..typeName.LastIndexOf('`')]);
            sb.Append('<');
            AppendParamTypes(sb, genericArgs);
            sb.Append('>');

            static void AppendParamTypes(StringBuilder sb, Type[] genericArgs)
            {
                for (int i = 0; i < genericArgs.Length; i += 1)
                {
                    AppendType(sb, genericArgs[i]);
                    if (i < genericArgs.Length - 1)
                    {
                        sb.Append(',').Append(' ');
                    }
                }
            }
        }

        static string GetSimpleTypeName(Type type)
        {
            return type.FullName switch
            {
                "System.SByte" => "sbyte",
                "System.Byte" => "byte",
                "System.Int16" => "short",
                "System.UInt16" => "ushort",
                "System.Int32" => "int",
                "System.UInt32" => "uint",
                "System.Int64" => "long",
                "System.UInt64" => "ulong",
                "System.IntPtr" => "nint",
                "System.UIntPtr" => "nuint",
                "System.Single" => "float",
                "System.Double" => "double",
                "System.Decimal" => "decimal",
                "System.Boolean" => "bool",
                "System.Char" => "char",
                "System.String" => "string",
                "System.Object" => "object",
                _ => type.Name
            };
        }
    }

    [ThreadStatic] private static StringBuilder _stringBuilder2;

    private static string ConstructTypeName_JSDY_OPT2(Type type)
    {
        if (!type.IsArray && !type.IsGenericType)
        {
            return GetSimpleTypeName(type);
        }

        _stringBuilder2 ??= new StringBuilder(256);
        var sb = _stringBuilder2;
        AppendType(sb, type);
        var result = sb.ToString();
        _stringBuilder2.Clear();
        return result;

        static void AppendType(StringBuilder sb, Type type)
        {
            if (type.IsArray)
            {
                AppendArray(sb, type);
            }
            else if (type.IsGenericType)
            {
                AppendGeneric(sb, type);
            }
            else
            {
                sb.Append(GetSimpleTypeName(type));
            }
        }

        static void AppendArray(StringBuilder sb, Type type)
        {
            //append inner most non-array element
            var elementType = type.GetElementType()!;
            while (elementType.IsArray)
            {
                elementType = elementType.GetElementType()!;
            }

            AppendType(sb, elementType);
            //append brackets
            AppendArrayRecursive(sb, type);

            static void AppendArrayRecursive(StringBuilder sb, Type type)
            {
                while (true)
                {
                    //append bracket with rank
                    var rank = type.GetArrayRank();
                    sb.Append('[');
                    for (int i = 0; i < rank - 1; i++)
                    {
                        sb.Append(',');
                    }

                    sb.Append(']');
                    //recursive call
                    var elementType = type.GetElementType()!;
                    if (elementType.IsArray)
                    {
                        type = elementType;
                        continue;
                    }

                    break;
                }
            }
        }

        static void AppendGeneric(StringBuilder sb, Type type)
        {
            var fullName = type.GetGenericTypeDefinition().FullName;
            ReadOnlySpan<char> genericDefinitionFullName = fullName != null ? fullName.AsSpan() : ReadOnlySpan<char>.Empty;
            var genericArgs = type.GenericTypeArguments;

            //Nullable
            if (genericDefinitionFullName == "System.Nullable`1")
            {
                AppendType(sb, genericArgs[0]);
                sb.Append('?');
                return;
            }

            //ValueTuple
            if (genericDefinitionFullName.StartsWith("System.ValueTuple`"))
            {
                sb.Append('(');
                AppendParamTypes(sb, genericArgs);
                sb.Append(')');
                return;
            }

            //normal generic
            var typeName = type.Name.AsSpan();
            sb.Append(typeName[..typeName.LastIndexOf('`')]);
            sb.Append('<');
            AppendParamTypes(sb, genericArgs);
            sb.Append('>');

            static void AppendParamTypes(StringBuilder sb, ReadOnlySpan<Type> genericArgs)
            {
                var n = genericArgs.Length - 1;
                for (int i = 0; i < n; i += 1)
                {
                    AppendType(sb, genericArgs[i]);
                    sb.Append(',').Append(' ');
                }

                AppendType(sb, genericArgs[^1]);
            }
        }

        static string GetSimpleTypeName(Type type)
        {
            return type.FullName switch
            {
                "System.SByte" => "sbyte",
                "System.Byte" => "byte",
                "System.Int16" => "short",
                "System.UInt16" => "ushort",
                "System.Int32" => "int",
                "System.UInt32" => "uint",
                "System.Int64" => "long",
                "System.UInt64" => "ulong",
                "System.IntPtr" => "nint",
                "System.UIntPtr" => "nuint",
                "System.Single" => "float",
                "System.Double" => "double",
                "System.Decimal" => "decimal",
                "System.Boolean" => "bool",
                "System.Char" => "char",
                "System.String" => "string",
                "System.Object" => "object",
                _ => type.Name
            };
        }
    }


    [ThreadStatic] private static StringBuilder _stringBuilder3;

    private static string ConstructTypeName_JSDY_OPT3(Type type)
    {
        if (!type.IsArray && !type.IsGenericType)
        {
            return GetSimpleTypeName(type);
        }

        _stringBuilder3 ??= new StringBuilder(256);
        var sb = _stringBuilder3;
        AppendType(sb, type);
        var result = sb.ToString();
        _stringBuilder3.Clear();
        return result;

        static void AppendType(StringBuilder sb, Type type)
        {
            if (type.IsArray)
            {
                AppendArray(sb, type);
            }
            else if (type.IsGenericType)
            {
                AppendGeneric(sb, type);
            }
            else
            {
                sb.Append(GetSimpleTypeName(type));
            }
        }

        static void AppendArray(StringBuilder sb, Type type)
        {
            //append inner most non-array element
            var elementType = type.GetElementType()!;
            while (elementType.IsArray)
            {
                elementType = elementType.GetElementType()!;
            }

            AppendType(sb, elementType);
            //append brackets
            AppendArrayRecursive(sb, type);

            static void AppendArrayRecursive(StringBuilder sb, Type type)
            {
                while (true)
                {
                    //append bracket with rank
                    var rank = type.GetArrayRank();
                    sb.Append('[');
                    sb.Append(',', rank - 1);
                    sb.Append(']');
                    //recursive call
                    var elementType = type.GetElementType()!;
                    if (elementType.IsArray)
                    {
                        type = elementType;
                        continue;
                    }

                    break;
                }
            }
        }

        static void AppendGeneric(StringBuilder sb, Type type)
        {
            var fullName = type.GetGenericTypeDefinition().FullName;
            ReadOnlySpan<char> genericDefinitionFullName = fullName != null ? fullName.AsSpan() : ReadOnlySpan<char>.Empty;
            var genericArgs = type.GenericTypeArguments;

            //Nullable
            if (genericDefinitionFullName == "System.Nullable`1")
            {
                AppendType(sb, genericArgs[0]);
                sb.Append('?');
                return;
            }

            //ValueTuple
            if (genericDefinitionFullName.StartsWith("System.ValueTuple`"))
            {
                sb.Append('(');
                AppendParamTypes(sb, genericArgs);
                sb.Append(')');
                return;
            }

            //normal generic
            var typeName = type.Name.AsSpan();
            sb.Append(typeName[..typeName.LastIndexOf('`')]);
            sb.Append('<');
            AppendParamTypes(sb, genericArgs);
            sb.Append('>');

            static void AppendParamTypes(StringBuilder sb, ReadOnlySpan<Type> genericArgs)
            {
                var n = genericArgs.Length - 1;
                for (int i = 0; i < n; i += 1)
                {
                    AppendType(sb, genericArgs[i]);
                    sb.Append(',').Append(' ');
                }

                AppendType(sb, genericArgs[^1]);
            }
        }

        static string GetSimpleTypeName(Type type)
        {
            return type.FullName switch
            {
                "System.SByte" => "sbyte",
                "System.Byte" => "byte",
                "System.Int16" => "short",
                "System.UInt16" => "ushort",
                "System.Int32" => "int",
                "System.UInt32" => "uint",
                "System.Int64" => "long",
                "System.UInt64" => "ulong",
                "System.IntPtr" => "nint",
                "System.UIntPtr" => "nuint",
                "System.Single" => "float",
                "System.Double" => "double",
                "System.Decimal" => "decimal",
                "System.Boolean" => "bool",
                "System.Char" => "char",
                "System.String" => "string",
                "System.Object" => "object",
                _ => type.Name
            };
        }
    }


    public static string ConstructTypeName_ZERXZ(Type type)
    {
        if (!type.IsGenericType) return type.Name;
        var name = type.GetGenericTypeDefinition().Name;
        var index = name.IndexOf('`');
        if (index > 0)
        {
            name = name[..index];
        }

        var genericArguments = type.GetGenericArguments();
        var args = string.Join(",", genericArguments.Select(ConstructTypeName_ZERXZ));
        return type.IsValueType ? $"({args})" : $"{name}<{args}>";
    }
}