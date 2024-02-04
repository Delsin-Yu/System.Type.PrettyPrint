using System.Text;
using BenchmarkDotNet.Attributes;

namespace GenericCSharpProgram;

public partial class Program
{
    [Benchmark] public void SimpleType_ZERXZ() => SimpleType(ConstructTypeName_ZERXZ);
    [Benchmark] public void ModerateType_ZERXZ() => ModerateType(ConstructTypeName_ZERXZ);
    [Benchmark] public void CrazyType_ZERXZ() => CrazyType(ConstructTypeName_ZERXZ);


    public static string ConstructTypeName_ZERXZ(Type type)
    {
        if (type.TryBuildArray(out var result))
        {
            return result;
        }
        if (!type.IsGenericType) return type.GetSimpleTypeName();
        var typeGenericTypeDefinition = type.GetGenericTypeDefinition();
        var name = typeGenericTypeDefinition.Name!;
        var index = name.IndexOf('`');
        var genericArguments = type.GetGenericArguments();
        var args = string.Join(",", genericArguments.Select<Type, string>(ConstructTypeName_ZERXZ));
        var isValueType = typeGenericTypeDefinition.IsValueTupleType();
        var isNullable = type.IsNullableType();
        if (index > 0)
        {
            name = name[..index];
        }
        if (isNullable)
        {
            return $"{args}?";
        }

        return isValueType ? $"({args})" : $"{name}<{args}>";
    }
    [ThreadStatic] private static StringBuilder _ZerxzStringBuilder;
    public static string ConstructTypeName_ZERXZ_OPT(Type type)
    {
        if (!type.IsArray && !type.IsGenericType)
        {
            return type.GetSimpleTypeName();
        }
        _ZerxzStringBuilder ??= new StringBuilder();
        var sb = _ZerxzStringBuilder;
        sb.AppendType(type);
        var resultType = sb.ToString();
        _ZerxzStringBuilder.Clear();
        return resultType;


    }

}

public static class ZerxzUtils
{
    public const string NullableType = "System.Nullable";
    public const string ValueTupleType = "System.ValueTuple";
    public static readonly IReadOnlyDictionary<Type, string> CacheType = new Dictionary<Type, string>()
    {
        {
            typeof(int), "int"
        },
        {
            typeof(float), "float"
        },
        {
            typeof(double), "double"
        },
        {
            typeof(bool), "bool"
        },
        {
            typeof(string), "string"
        },
        {
            typeof(object), "object"
        },
        {
            typeof(void), "void"
        },
        {
            typeof(char), "char"
        },
        {
            typeof(byte), "byte"
        },
        {
            typeof(sbyte), "sbyte"
        },
        {
            typeof(short), "short"
        },
        {
            typeof(ushort), "ushort"
        },
        {
            typeof(uint), "uint"
        },
        {
            typeof(long), "long"
        },
        {
            typeof(ulong), "ulong"
        },
        {
            typeof(nint), "nint"
        },
        {
            typeof(nuint), "nuint"
        },
        {
            typeof(decimal), "decimal"
        },
    };
    public static readonly IReadOnlyDictionary<string, string> CacheTypeName = new Dictionary<string, string>()
    {
        {
            "System.SByte", "sbyte"
        },
        {
            "System.Byte", "byte"
        },
        {
            "System.Int16", "short"
        },
        {
            "System.UInt16", "ushort"
        },
        {
            "System.Int32", "int"
        },
        {
            "System.UInt32", "uint"
        },
        {
            "System.Int64", "long"
        },
        {
            "System.UInt64", "ulong"
        },
        {
            "System.IntPtr", "nint"
        },
        {
            "System.UIntPtr", "nuint"
        },
        {
            "System.Single", "float"
        },
        {
            "System.Double", "double"
        },
        {
            "System.Decimal", "decimal"
        },
        {
            "System.Boolean", "bool"
        },
        {
            "System.Char", "char"
        },
        {
            "System.String", "string"
        },
        {
            "System.Object", "object"
        },
    };
    public static bool IsNullableType(this Type type) => type.FullName!.StartsWith(NullableType);
    public static bool IsValueTupleType(this Type type) => type.FullName!.StartsWith(ValueTupleType);

    public static string GetSimpleTypeName(this string typeName)
    {
        return CacheTypeName.GetValueOrDefault(typeName, typeName);
    }
    public static string GetSimpleTypeName(this Type type)
    {
        return CacheType.GetValueOrDefault(type, type.Name);
    }
    public static bool BuildArray(this Type type, StringBuilder stringBuilder)
    {
        if (!type.IsArray)
        {
            return false;
        }
        stringBuilder.Append(GetTypeArrayResult(type));
        var typeArrayIndex = type.Name.IndexOf('[');
        for (int i = type.Name.Length - 1; i >= typeArrayIndex; i--)
        {
            var c = type.Name[i];
            switch (c)
            {
                case '[':
                    stringBuilder.Append(']');
                    continue;
                case ']':
                    stringBuilder.Append('[');
                    continue;
                default:
                    stringBuilder.Append(c);
                    continue;
            }
        }
        return true;
    }
    public static void AppendType(this StringBuilder sb, Type type)
    {
        if (type.BuildArray(sb))
        {
            return;
        }
        if (type.BuildGeneric(sb))
        {
            return;
        }
        sb.Append(type.GetSimpleTypeName());
    }

    public static bool BuildGeneric(this Type type, StringBuilder stringBuilder)
    {
        if (!type.IsGenericType)
        {
            return false;
        }
        var typeGenericTypeDefinition = type.GetGenericTypeDefinition();
        var name = typeGenericTypeDefinition.Name!.AsSpan();
        var index = name.IndexOf('`');
        var genericArguments = type.GetGenericArguments();
        // var args = string.Join(",", genericArguments.Select<Type, string>(ConstructTypeName_ZERXZ));
        var args = new StringBuilder();
        for (var i = 0; i < genericArguments.Length; i++)
        {
            args.AppendType(genericArguments[i]);
            if (i < genericArguments.Length - 1)
            {
                args.Append(',');
            }
        }
        var isNullable = type.IsNullableType();

        if (isNullable)
        {
            stringBuilder.AppendFormat("{0}?", args);
            return true;
        }
        var isValueType = typeGenericTypeDefinition.IsValueTupleType();
        if (isValueType)
        {
            stringBuilder.AppendFormat("({0})", args);
        }
        else
        {
            stringBuilder.Append(index > 0 ? name[..index] : name);
            stringBuilder.AppendFormat("<{0}>", args);
        }
        return true;
    }
    public static bool TryBuildArray(this Type type, out string result)
    {

        if (!type.IsArray)
        {
            result = "";
            return false;
        }
        var typeResult = GetTypeArrayResult(type);

        var typeArrayIndex = type.Name.IndexOf('[');
        if (typeArrayIndex > 0)
        {
            var arrayArgs = type.Name[typeArrayIndex..].Reverse().Select(c => c == '[' ? ']' : c == ']' ? '[' : c);
            typeResult += string.Concat(arrayArgs);
        }
        result = typeResult;
        return true;
    }
    public static string GetTypeArrayResult(this Type type)
    {
        if (type.IsNullableType()) return type.GetGenericArguments().First().GetSimpleTypeName() + "?";
        var typeNameIndex = type.FullName!.IndexOf('[');
        return typeNameIndex > 0 ? type.FullName[..typeNameIndex].GetSimpleTypeName() : type.Name.GetSimpleTypeName();
    }
}