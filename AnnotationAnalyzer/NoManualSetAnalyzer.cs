using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AnnotationAnalyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NoManualSetAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "RO001",
        title: "Field marked with [NoManualSet] should not be set manually",
        messageFormat: "Fields marked with [NoManualSet] should not be assigned manually",
        category: "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Register action for field assignments
        context.RegisterSyntaxNodeAction(AnalyzeAssignment, SyntaxKind.SimpleAssignmentExpression);
    }

    private static void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
    {
        var assignment = (AssignmentExpressionSyntax) context.Node;

        // Get the left-hand symbol (the field being assigned)
        var leftSymbol = ModelExtensions.GetSymbolInfo(context.SemanticModel, assignment.Left).Symbol as IFieldSymbol;
        if (leftSymbol == null) return;

        // Check if the field has the [NoManualSet] attribute
        if (!leftSymbol.GetAttributes().Any(a => a.AttributeClass?.Name == "NoManualSetAttribute")) return;

        // Report diagnostic
        var diagnostic = Diagnostic.Create(Rule, assignment.GetLocation(), leftSymbol.Name);
        context.ReportDiagnostic(diagnostic);
    }
}