using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace H.Ipc.Generator.IntegrationTests;

public static class TestHelper
{
    public static Task CheckSource(this VerifyBase verifier, string source)
    {
        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[]
            {
                CSharpSyntaxTree.ParseText(source),
            },
            references: new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            });
        var generator = new HIpcGenerator();
        var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGenerators(compilation);

        return verifier
            .Verify(driver)
            .UseDirectory("Snapshots");
    }
}