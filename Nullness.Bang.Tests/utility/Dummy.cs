using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DummyAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "Dummy";

    private static readonly LocalizableString Title = "Dummy analyzer";
    private static readonly LocalizableString MessageFormat = "Dummy analyzer";
    private static readonly LocalizableString Description = "Dummy analyzer";
    private const string Category = "Testing";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: Description
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        // You can add the analysis logic here or leave it empty if you only want a dummy analyzer for testing purposes.
    }
}
