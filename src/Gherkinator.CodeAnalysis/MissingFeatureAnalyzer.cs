using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Gherkinator.CodeAnalysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MissingFeatureAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
            => ImmutableArray.Create(new DiagnosticDescriptor(
                "GK001", 
                "Feature file not found",
                "A feature file named {0} was not found in the current directory.", 
                "Build", DiagnosticSeverity.Error, true));

        public override void Initialize(AnalysisContext context)
            => context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.ObjectCreationExpression);

        void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var semantic = context.Compilation.GetSemanticModel(context.Node.SyntaxTree);
            var symbol = semantic.GetSymbolInfo(context.Node);
            var creation = (ObjectCreationExpressionSyntax)context.Node;
            var args = creation.ArgumentList.Arguments.Select(arg => semantic.GetSymbolInfo(arg)).ToList();

            //if (symbol.Symbol?.Kind == SymbolKind.Method)
            //{
            //    var method = (IMethodSymbol)symbol.Symbol;
            //    if (method.GetAttributes().Any(x => x.AttributeClass == generator) &&
            //        // We don't generate anything if generator is applied to a non-generic method.
            //        !method.TypeArguments.IsDefaultOrEmpty)
            //    // Skip generic method definitions since they are typically usability overloads 
            //    // like Mock.Of<T>(...)
            //    // TODO: doesn't seem like this would be needed?
            //    //!method.TypeArguments.Any(x => x.Kind == SymbolKind.TypeParameter))
            //    {
            //        var args = method.TypeArguments.OfType<INamedTypeSymbol>().Where(t => t.ContainingType != null).ToArray();
            //        if (args.Length != 0)
            //        {
            //            var diagnostic = Diagnostic.Create(
            //                descriptor,
            //                context.Node.GetLocation(),
            //                string.Join(", ", args.Select(t => t.ContainingType.Name + "." + t.Name)));

            //            context.ReportDiagnostic(diagnostic);
            //        }
            //    }
            //}
        }
    }
}
