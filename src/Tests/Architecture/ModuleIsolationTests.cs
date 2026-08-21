using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Architecture;

public class ModuleIsolationTests
{
    private static readonly string[] ModuleNames = ["Users", "Authentication"];
    private static readonly string ModulesRoot = GetModulesRoot();

    public static IEnumerable<object[]> ModulePairs()
    {
        foreach (var module in ModuleNames)
        foreach (var other in ModuleNames)
        {
            if (module != other)
                yield return new object[] { module, other };
        }
    }

    [Theory]
    [MemberData(nameof(ModulePairs))]
    public void Module_ShouldNotHaveProjectReferenceToAnotherModule(string moduleName, string otherModuleName)
    {
        var csprojPath = Path.Combine(ModulesRoot, moduleName, $"{moduleName}.csproj");
        var xml = XDocument.Load(csprojPath);

        var referencedProjectNames = xml.Descendants("ProjectReference")
            .Select(e => e.Attribute("Include")?.Value)
            .Where(path => path is not null)
            .Select(path => Path.GetFileNameWithoutExtension(path!));

        Assert.DoesNotContain(otherModuleName, referencedProjectNames);
    }

    private static string GetModulesRoot([CallerFilePath] string thisFilePath = "") =>
        Path.GetFullPath(Path.Combine(Path.GetDirectoryName(thisFilePath)!, "..", "..", "Modules"));
}
