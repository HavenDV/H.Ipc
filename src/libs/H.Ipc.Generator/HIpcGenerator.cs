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

    #endregion

    #region Methods

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.SyntaxProvider
            .ForAttributeWithMetadataName("H.IpcGenerators.IpcClientAttribute")
            .SelectManyAllAttributesOfCurrentClassSyntax()
            .SelectAndReportExceptions(PrepareData, context, Id)
            .SelectAndReportExceptions((classData) => GetClientSourceCode(classData), context, Id)
            .AddSource(context);
        context.SyntaxProvider
            .ForAttributeWithMetadataName("H.IpcGenerators.IpcServerAttribute")
            .SelectManyAllAttributesOfCurrentClassSyntax()
            .SelectAndReportExceptions(PrepareData, context, Id)
            .SelectAndReportExceptions(GetServerSourceCode, context, Id)
            .AddSource(context);
        context.SyntaxProvider
            .ForAttributeWithMetadataName("H.IpcGenerators.IpcClientAttribute")
            .SelectManyAllAttributesOfCurrentClassSyntax()
            .SelectAndReportExceptions(PrepareData, context, Id)
            .SelectAndReportExceptions(GetClientRequestsSourceCode, context, Id)
            .AddSource(context);
        context.SyntaxProvider
            .ForAttributeWithMetadataName("H.IpcGenerators.IpcServerAttribute")
            .SelectManyAllAttributesOfCurrentClassSyntax()
            .SelectAndReportExceptions(PrepareData, context, Id)
            .SelectAndReportExceptions(GetServerRequestsSourceCode, context, Id)
            .AddSource(context);
    }

    private static ClassData PrepareData(
        (SemanticModel SemanticModel, AttributeData AttributeData, ClassDeclarationSyntax ClassSyntax, INamedTypeSymbol ClassSymbol) tuple)
    {
        var (_, _, _, classSymbol) = tuple;
        
        var @interface = classSymbol.Interfaces.First();
        var methods = @interface
            .GetMembers()
            .OfType<IMethodSymbol>()
            .ToArray();
        
        var fullClassName = classSymbol.ToString();
        var @namespace = fullClassName.Substring(0, fullClassName.LastIndexOf('.'));
        var className = fullClassName.Substring(fullClassName.LastIndexOf('.') + 1);
        var interfaceName = @interface.Name;

        return new ClassData(
            Namespace: @namespace,
            Name: className,
            InterfaceName: interfaceName,
            Methods: methods);
    }

    private static FileWithName GetClientSourceCode(ClassData @class)
    {
        return new FileWithName(
            Name: $"{@class.Name}.IpcClient.generated.cs",
            Text: SourceGenerationHelper.GenerateClientImplementation(@class));
    }

    private static FileWithName GetServerSourceCode(ClassData @class)
    {
        return new FileWithName(
            Name: $"{@class.Name}.IpcServer.generated.cs",
            Text: SourceGenerationHelper.GenerateServerImplementation(@class));
    }
    
    private static FileWithName GetClientRequestsSourceCode(ClassData @class)
    {
        return new FileWithName(
            Name: $"{@class.InterfaceName}.ClientRequests.generated.cs",
            Text: SourceGenerationHelper.GenerateRequests(@class, server: false));
    }
    
    private static FileWithName GetServerRequestsSourceCode(ClassData @class)
    {
        return new FileWithName(
            Name: $"{@class.InterfaceName}.ServerRequests.generated.cs",
            Text: SourceGenerationHelper.GenerateRequests(@class, server: true));
    }

    #endregion
}