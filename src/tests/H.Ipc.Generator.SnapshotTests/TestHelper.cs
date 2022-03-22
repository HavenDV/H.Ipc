using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace H.Generators.IntegrationTests;

public static class TestHelper
{
    public static async Task CheckSource(this VerifyBase verifier, string source)
    {
        var dotNetFolder = Path.GetDirectoryName(typeof(object).Assembly.Location) ?? string.Empty;
        var compilation = (Compilation)CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[]
            {
                CSharpSyntaxTree.ParseText(source),
                CSharpSyntaxTree.ParseText(@"
namespace MyCode
{
    public class Program
    {
        public static void Main(string[] args)
        {
        }
    }
}
"),
            },
            references: new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(dotNetFolder, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(dotNetFolder, "netstandard.dll")),
                MetadataReference.CreateFromFile(typeof(H.Pipes.PipeClient<>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Text.Json.JsonSerializer).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(H.IpcGenerators.IpcClientAttribute).Assembly.Location),
            });
        var generator = new HIpcGenerator();
        var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGenerators(compilation);
        
        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out compilation,
            out _);
        var diagnostics = compilation.GetDiagnostics();

        await verifier
            .Verify(diagnostics)
            .UseDirectory("Snapshots")
            .UseTextForParameters("Diagnostics");
        await verifier
            .Verify(driver)
            .UseDirectory("Snapshots");
    }
}