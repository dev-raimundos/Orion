using System.Reflection;

namespace Architecture;

public class ModuleIsolationTests
{
    private const string ModulesRootNamespace = "Api.Modules";
    private static readonly Assembly ApiAssembly = typeof(Api.Program).Assembly;

    public static IEnumerable<object[]> ModuleNames() =>
        ApiAssembly.GetTypes()
            .Select(t => t.Namespace)
            .Where(ns => ns is not null && ns.StartsWith(ModulesRootNamespace + "."))
            .Select(ns => ns![(ModulesRootNamespace.Length + 1)..].Split('.')[0])
            .Distinct()
            .Select(name => new object[] { name });

    [Theory]
    [MemberData(nameof(ModuleNames))]
    public void Module_ShouldNotReferenceTypesFromOtherModules(string moduleName)
    {
        var moduleNamespace = $"{ModulesRootNamespace}.{moduleName}";

        var moduleTypes = ApiAssembly.GetTypes()
            .Where(t => IsInNamespace(t, moduleNamespace))
            .ToList();

        var violations = moduleTypes
            .SelectMany(type => GetReferencedTypes(type).Select(referenced => (type, referenced)))
            .Where(pair => pair.referenced.Namespace is not null
                && pair.referenced.Namespace.StartsWith(ModulesRootNamespace + ".")
                && !IsInNamespace(pair.referenced, moduleNamespace))
            .Select(pair => $"{pair.type.FullName} -> {pair.referenced.FullName}")
            .Distinct()
            .ToList();

        Assert.True(violations.Count == 0,
            $"O módulo '{moduleName}' fere o isolamento ao referenciar tipos de outro módulo diretamente:\n{string.Join('\n', violations)}");
    }

    private static bool IsInNamespace(Type type, string ns) =>
        type.Namespace is not null && (type.Namespace == ns || type.Namespace.StartsWith(ns + "."));

    private static IEnumerable<Type> GetReferencedTypes(Type type)
    {
        var referenced = new List<Type>();

        if (type.BaseType is not null)
            referenced.Add(type.BaseType);

        referenced.AddRange(type.GetInterfaces());

        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic
            | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        foreach (var property in type.GetProperties(flags))
            referenced.Add(property.PropertyType);

        foreach (var field in type.GetFields(flags))
            referenced.Add(field.FieldType);

        foreach (var method in type.GetMethods(flags))
        {
            referenced.Add(method.ReturnType);
            referenced.AddRange(method.GetParameters().Select(p => p.ParameterType));
        }

        foreach (var constructor in type.GetConstructors(flags))
            referenced.AddRange(constructor.GetParameters().Select(p => p.ParameterType));

        return referenced.SelectMany(UnwrapGenericArguments).Distinct();
    }

    private static IEnumerable<Type> UnwrapGenericArguments(Type type)
    {
        yield return type;

        if (!type.IsGenericType)
            yield break;

        foreach (var argument in type.GetGenericArguments())
        {
            foreach (var unwrapped in UnwrapGenericArguments(argument))
                yield return unwrapped;
        }
    }
}
