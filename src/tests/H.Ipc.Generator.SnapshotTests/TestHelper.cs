using H.Generators.Tests.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;

namespace H.Generators.IntegrationTests;

public static class TestHelper
{
    public static async Task CheckSourceAsync(
        this VerifyBase verifier,
        string source,
        CancellationToken cancellationToken = default)
    {
        var referenceAssemblies = LatestReferenceAssemblies.Net80
            .WithPackages([new PackageIdentity("H.Pipes.AccessControl", "2.0.59")]);
        var references = await referenceAssemblies.ResolveAsync(null, cancellationToken);
        references = references
            .Add(MetadataReference.CreateFromFile(typeof(H.IpcGenerators.IpcClientAttribute).Assembly.Location));

        var compilation = (Compilation)CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[]
            {
                CSharpSyntaxTree.ParseText(source, cancellationToken: cancellationToken),
            },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var driver = CSharpGeneratorDriver
            .Create(new HIpcGenerator())
            .RunGeneratorsAndUpdateCompilation(compilation, out compilation, out _, cancellationToken);
        var diagnostics = compilation.GetDiagnostics(cancellationToken);

        await Task.WhenAll(
            verifier
                .Verify(diagnostics)
                .UseDirectory("Snapshots")
                .UseTextForParameters("Diagnostics"),
            verifier
                .Verify(driver)
                .UseDirectory("Snapshots"));
    }
}