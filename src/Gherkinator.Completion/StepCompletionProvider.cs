using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Tags;
using Microsoft.CodeAnalysis.Text;

namespace Gherkinator.Completion
{
    [ExportCompletionProvider(nameof(StepCompletionProvider), LanguageNames.CSharp)]
    public class StepCompletionProvider : CompletionProvider
    {
        private static readonly CompletionItemRules StandardCompletionRules = CompletionItemRules.Default.WithSelectionBehavior(CompletionItemSelectionBehavior.SoftSelection);

        public override bool ShouldTriggerCompletion(SourceText text, int caretPosition, CompletionTrigger trigger, OptionSet options)
        {
            // TODO: should trigger if we're inside a string
            return base.ShouldTriggerCompletion(text, caretPosition, trigger, options);
        }

        public override Task<CompletionDescription> GetDescriptionAsync(Document document, CompletionItem item, CancellationToken cancellationToken)
        {
            // TODO: get th actual .feature file location.
            return Task.FromResult(CompletionDescription.FromText("Step retrieved from Foo.feature(120,34)"));
        }

        public override Task<CompletionChange> GetChangeAsync(Document document, CompletionItem item, char? commitKey, CancellationToken cancellationToken)
        {
            // Determine change to insert
            return base.GetChangeAsync(document, item, commitKey, cancellationToken);
        }

        public override async Task ProvideCompletionsAsync(CompletionContext context)
        {
            if (!context.Document.SupportsSemanticModel)
                return;

            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
            var compilation = await context.Document.Project.GetCompilationAsync(context.CancellationToken);
            var diagnostics = compilation.GetDiagnostics(context.CancellationToken);

            var span = context.CompletionListSpan;
            var node = syntaxRoot
                .DescendantNodes(n => n.FullSpan.Contains(context.Position))
                //.OfType<InvocationExpressionSyntax>()
                .LastOrDefault();

            if (node != null)
            {
                var symbol = semanticModel.GetSymbolInfo(node, context.CancellationToken);
                if (symbol.Symbol != null)
                {

                }
            }

            //context.AddItem(CompletionItem.Create("@style/MainTheme",
            //    tags: ImmutableArray.Create(WellKnownTags.Constant),
            //    // TODO: props should contain the file location it was retrieved from
            //    // properties: ImmutableDictionary.CreateBuilder<string, string>().Add(,
            //    rules: StandardCompletionRules));
        }
    }
}
