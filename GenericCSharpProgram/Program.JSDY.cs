using System.Text;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;

namespace GenericCSharpProgram;

public partial class Algorithms
{
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
    

    [Benchmark] public void SimpleType_JSDY_OPT4() => SimpleType(ConstructTypeName_JSDY_OPT4);
    [Benchmark] public void ModerateType_JSDY_OPT4() => ModerateType(ConstructTypeName_JSDY_OPT4);
    [Benchmark] public void CrazyType_JSDY_OPT4() => CrazyType(ConstructTypeName_JSDY_OPT4);
    

    public static string ConstructTypeName_JSDY(Type type)
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
    public static string ConstructTypeName_JSDY_OPT(Type type)
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
    public static string ConstructTypeName_JSDY_OPT2(Type type)
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
    public static string ConstructTypeName_JSDY_OPT3(Type type)
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

    [ThreadStatic] private static StringBuilder _stringBuilder4;
    private static readonly HashSet<Type> _tupleTypes = new HashSet<Type>() {
        // ValueTuple with only one element should be treated as normal generic type.
        //typeof(ValueTuple<>),
        typeof(ValueTuple<,>),
        typeof(ValueTuple<,,>),
        typeof(ValueTuple<,,,>),
        typeof(ValueTuple<,,,,>),
        typeof(ValueTuple<,,,,,>),
        typeof(ValueTuple<,,,,,,>),
        typeof(ValueTuple<,,,,,,,>),
    };
    private static readonly Dictionary<Type, string> _builtinTypeNameDict = new()
    {
        {typeof(sbyte),"sbyte"},
        {typeof(byte),"byte"},
        {typeof(short),"short"},
        {typeof(ushort),"ushort"},
        {typeof(int),"int"},
        {typeof(uint),"uint"},
        {typeof(long),"long"},
        {typeof(ulong),"ulong"},
        {typeof(nint),"nint"},
        {typeof(nuint),"nuint"},
        {typeof(float),"float"},
        {typeof(double),"double"},
        {typeof(decimal),"decimal"},
        {typeof(bool),"bool"},
        {typeof(char),"char"},
        {typeof(string),"string"},
        {typeof(object),"object"},
    };

    public static string ConstructTypeName_JSDY_OPT4(Type type)
    {
        if (!type.IsArray && !type.IsGenericType)
        {
            return GetSimpleTypeName(type);
        }

        _stringBuilder4 ??= new StringBuilder(256);
        var sb = _stringBuilder4;
        AppendType(sb, type);
        var result = sb.ToString();
        _stringBuilder4.Clear();
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

            var genericArgs = type.GenericTypeArguments;
            var genericDefinition = type.GetGenericTypeDefinition();
            //Nullable
            if (genericDefinition == typeof(Nullable<>))
            {
                AppendType(sb, genericArgs[0]);
                sb.Append('?');
                return;
            }

            //ValueTuple
            if (_tupleTypes.Contains(genericDefinition))
            {
                sb.Append('(');
                while (true) {
                    // We assume that ValueTuple has 1~8 elements.
                    // And the 8th element (TRest) is always another ValueTuple.

                    // This is a hard coded tuple element length check.
                    if (genericArgs.Length != 8) {
                        AppendParamTypes(sb, genericArgs);
                        break;
                    } else {
                        AppendParamTypesWithLastComma(sb, genericArgs.AsSpan(0, 7));

                        // TRest should be a ValueTuple!
                        var nextTuple = genericArgs[7];

                        genericArgs = nextTuple.GenericTypeArguments;
                    }
                }
                sb.Append(')');
                return;
            }
            static void AppendParamTypesWithLastComma(StringBuilder sb, ReadOnlySpan<Type> genericArgs) {
                var n = genericArgs.Length;
                for (int i = 0; i < n; i += 1) {
                    AppendType(sb, genericArgs[i]);
                    sb.Append(',').Append(' ');
                }
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
            return _builtinTypeNameDict.TryGetValue(type, out var name) ? name : type.Name;
        }
    }
}