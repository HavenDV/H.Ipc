using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace H.Generators;

public static class CommonSteps
{
    public static IncrementalValuesProvider<GeneratorAttributeSyntaxContext>
        ForAttributeWithMetadataName(
            this SyntaxValueProvider source,
            string fullyQualifiedMetadataName)
    {
        return source
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: fullyQualifiedMetadataName,
                predicate: static (node, _) =>
                    node is ClassDeclarationSyntax { AttributeLists.Count: > 0 } or RecordDeclarationSyntax { AttributeLists.Count: > 0 },
                transform: static (context, _) => context);
    }

    public static IncrementalValuesProvider<(SemanticModel SemanticModel, AttributeData AttributeData, ClassDeclarationSyntax ClassSyntax, INamedTypeSymbol ClassSymbol)>
        SelectManyAllAttributesOfCurrentClassSyntax(
            this IncrementalValuesProvider<GeneratorAttributeSyntaxContext> source)
    {
        return source
            .SelectMany(static (context, _) => context.Attributes
                .Select(x => (
                    context.SemanticModel,
                    AttributeData: x,
                    ClassSyntax: (ClassDeclarationSyntax)context.TargetNode,
                    ClassSymbol: (INamedTypeSymbol)context.TargetSymbol)));
    }
}
