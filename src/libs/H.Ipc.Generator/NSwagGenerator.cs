using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace H.Ipc.Generator;

[Generator]
public class NSwagGenerator : IIncrementalGenerator
{
    #region Constants

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
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
               "IpcClientAttribute.generated.cs",
               SourceText.From(@"
namespace H.IpcGenerators
{
    [global::System.AttributeUsage(global::System.AttributeTargets.Class)]
    public class IpcClientAttribute : global::System.Attribute
    {
    }
}", Encoding.UTF8)));
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
               "IpcServerAttribute.generated.cs",
               SourceText.From(@"
namespace H.IpcGenerators
{
    [global::System.AttributeUsage(global::System.AttributeTargets.Class)]
    public class IpcServerAttribute : global::System.Attribute
    {
    }
}", Encoding.UTF8)));

        var enumDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: static (context, _) => GetSemanticTargetForGeneration(context))
            .Where(static syntax => syntax is not null);

        var compilationAndEnums = context.CompilationProvider.Combine(enumDeclarations.Collect());

        context.RegisterSourceOutput(
            compilationAndEnums,
            static (context, source) => Execute(source.Left, source.Right!, context));
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
            foreach(var @class in classes)
            {
                var code = SourceGenerationHelper.GenerateClientImplementation(@class);

                context.AddSource(
                    $"{@class.Name}.generated.cs",
                    SourceText.From(code, Encoding.UTF8));
            }
        }
        catch (Exception exception)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "IPCG0001",
                        "Exception: ",
                        $"{exception}",
                        "Usage",
                        DiagnosticSeverity.Error,
                        true),
                    Location.None));
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
                var code = SourceGenerationHelper.GenerateServerImplementation(@class);

                context.AddSource(
                    $"{@class.Name}.generated.cs",
                    SourceText.From(code, Encoding.UTF8));
            }
        }
        catch (Exception exception)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "IPCG0001",
                        "Exception: ",
                        $"{exception}",
                        "Usage",
                        DiagnosticSeverity.Error,
                        true),
                    Location.None));
        }
    }

    private static List<ClassData> GetTypesToGenerate(
        Compilation compilation,
        IEnumerable<ClassDeclarationSyntax> enums,
        CancellationToken cancellationToken)
    {
        var enumsToGenerate = new List<ClassData>();
        var enumAttribute = compilation.GetTypeByMetadataName(IpcClientAttribute);
        if (enumAttribute == null)
        {
            return enumsToGenerate;
        }

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
            
            // Get the full type name of the enum e.g. Colour, 
            // or OuterClass<T>.Colour if it was nested in a generic type (for example)
            var fullClassName = classSymbol.ToString();
            var @namespace = fullClassName.Substring(0, fullClassName.LastIndexOf('.'));
            var className = fullClassName.Substring(fullClassName.LastIndexOf('.') + 1);
            
            // Get all the members in the enum
            //var enumMembers = classSymbol.GetMembers();
            //var members = new List<string>(enumMembers.Length);

            //// Get all the fields from the enum, and add their name to the list
            //foreach (ISymbol member in enumMembers)
            //{
            //    if (member is IFieldSymbol field && field.ConstantValue is not null)
            //    {
            //        members.Add(member.Name);
            //    }
            //}

            enumsToGenerate.Add(new ClassData(@namespace, className, methods));
        }

        return enumsToGenerate;
    }

    #endregion
}

public readonly record struct ClassData(
    string Namespace,
    string Name,
    IReadOnlyCollection<IMethodSymbol> Methods);