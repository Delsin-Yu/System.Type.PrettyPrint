namespace GenericCSharpProgram;

public partial class Program
{
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
        var args = string.Join(",", genericArguments.Select<Type, string>(ConstructTypeName_ZERXZ));
        return type.IsValueType ? $"({args})" : $"{name}<{args}>";
    }
}