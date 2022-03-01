using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace H.NSwag.Generator;

[Generator]
public class NSwagGenerator : ISourceGenerator
{
    #region Methods

    public void Execute(GeneratorExecutionContext context)
    {
        foreach (var text in context.AdditionalFiles
            .Where(static text => text.Path.EndsWith(
                ".nswag",
                StringComparison.InvariantCultureIgnoreCase)))
        {
            try
            {
                context.AddSource(
                    $"{Path.GetFileName(text.Path)}.cs",
                    SourceText.From("", Encoding.UTF8));
            }
            catch (Exception exception)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        new DiagnosticDescriptor(
                            "NSG0001",
                            "Exception: ",
                            $"{exception}",
                            "Usage",
                            DiagnosticSeverity.Error,
                            true),
                        Location.None));
            }
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }

    #endregion

    #region Utilities

    private static string? GetGlobalOption(GeneratorExecutionContext context, string name)
    {
        return context.AnalyzerConfigOptions.GlobalOptions.TryGetValue(
            $"build_property.{nameof(NSwagGenerator)}_{name}",
            out var result) &&
            !string.IsNullOrWhiteSpace(result)
            ? result
            : null;
    }

    #endregion
}
