using System.Collections.Immutable;
using H.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace H.Generators;

[Generator]
public class HIpcGenerator : IIncrementalGenerator
{
    #region Constants

    public const string Name = nameof(HIpcGenerator);
    public const string Id = "IPCG";

    private const string IpcClientAttribute = "H.IpcGenerators.IpcClientAttribute";
    private const string IpcServerAttribute = "H.IpcGenerators.IpcServerAttribute";

    #endregion

    #region Methods

    private static (ClassDeclarationSyntax Class, string FullName)? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();
                if (fullName == IpcClientAttribute ||
                    fullName == IpcServerAttribute)
                {
                    return (classDeclarationSyntax, fullName);
                }
            }
        }

        return null;
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var enumDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: static (context, _) => GetSemanticTargetForGeneration(context))
            .Where(static syntax => syntax is not null);

        var compilationAndEnums = context.CompilationProvider.Combine(enumDeclarations.Collect());

        context.RegisterSourceOutput(
            compilationAndEnums,
            static (context, source) => Execute(source.Left, source.Right, context));
    }

    private static void Execute(
        Compilation compilation,
        ImmutableArray<(ClassDeclarationSyntax Class, string FullName)?> classSyntaxes,
        SourceProductionContext context)
    {
        if (classSyntaxes.IsDefaultOrEmpty)
        {
            return;
        }

        try
        {
            var distinctClassSyntaxes = classSyntaxes
                .Where(static tuple => tuple!.Value.FullName == IpcClientAttribute)
                .Select(static tuple => tuple!.Value.Class)
                .Distinct()
                .ToArray();

            var classes = GetTypesToGenerate(compilation, distinctClassSyntaxes, context.CancellationToken);
            foreach (var @class in classes)
            {
                context.AddTextSource(
                    hintName: $"{@class.Name}.generated.cs",
                    text: SourceGenerationHelper.GenerateClientImplementation(@class));
            }
        }
        catch (Exception exception)
        {
            context.ReportException(
                id: "001",
                exception: exception,
                prefix: Id);
        }

        try
        {
            var distinctClassSyntaxes = classSyntaxes
                .Where(static tuple => tuple!.Value.FullName == IpcServerAttribute)
                .Select(static tuple => tuple!.Value.Class)
                .Distinct()
                .ToArray();

            var classes = GetTypesToGenerate(compilation, distinctClassSyntaxes, context.CancellationToken);
            foreach (var @class in classes)
            {
                context.AddTextSource(
                    hintName: $"{@class.Name}.generated.cs",
                    text: SourceGenerationHelper.GenerateServerImplementation(@class));
            }
        }
        catch (Exception exception)
        {
            context.ReportException(
                id: "001",
                exception: exception,
                prefix: Id);
        }

        try
        {
            var distinctClassSyntaxes = classSyntaxes
                .Where(static tuple =>
                    tuple!.Value.FullName == IpcServerAttribute ||
                    tuple.Value.FullName == IpcClientAttribute)
                .Select(static tuple => tuple!.Value.Class)
                .Distinct()
                .ToArray();

            var classes = GetTypesToGenerate(compilation, distinctClassSyntaxes, context.CancellationToken);
            foreach (var @class in classes
                .GroupBy(static @class => @class.InterfaceName)
                .Distinct()
                .Select(static group => group.First()))
            {
                context.AddTextSource(
                    hintName: $"{@class.InterfaceName}_Requests.generated.cs",
                    text: SourceGenerationHelper.GenerateRequests(@class));
            }
        }
        catch (Exception exception)
        {
            context.ReportException(
                id: "001",
                exception: exception,
                prefix: Id);
        }
    }

    private static List<ClassData> GetTypesToGenerate(
        Compilation compilation,
        IEnumerable<ClassDeclarationSyntax> enums,
        CancellationToken cancellationToken)
    {
        var enumsToGenerate = new List<ClassData>();
        foreach (var classDeclarationSyntax in enums)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(
                classDeclarationSyntax, cancellationToken) is not INamedTypeSymbol classSymbol)
            {
                continue;
            }

            var @interface = classSymbol.Interfaces.First();
            var methods = @interface
                .GetMembers()
                .OfType<IMethodSymbol>()
                .ToArray();
            
            var fullClassName = classSymbol.ToString();
            var @namespace = fullClassName.Substring(0, fullClassName.LastIndexOf('.'));
            var className = fullClassName.Substring(fullClassName.LastIndexOf('.') + 1);
            var interfaceName = @interface.Name;

            enumsToGenerate.Add(new ClassData(@namespace, className, interfaceName, methods));
        }

        return enumsToGenerate;
    }

    #endregion
}

public readonly record struct ClassData(
    string Namespace,
    string Name,
    string InterfaceName,
    IReadOnlyCollection<IMethodSymbol> Methods);