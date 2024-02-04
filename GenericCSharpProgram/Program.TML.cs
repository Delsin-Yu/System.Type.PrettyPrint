using System.Text;
using BenchmarkDotNet.Attributes;

namespace GenericCSharpProgram;

public partial class Program
{
    [Benchmark] public void SimpleType_TML() => SimpleType(ConstructTypeName_TML);
    [Benchmark] public void ModerateType_TML() => ModerateType(ConstructTypeName_TML);
    [Benchmark] public void CrazyType_TML() => CrazyType(ConstructTypeName_TML);
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
}