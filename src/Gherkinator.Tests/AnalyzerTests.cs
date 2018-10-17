using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gherkinator.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Gherkinator.Tests
{
    public class AnalyzerTests
    {
        [Fact]
        public async Task create_project()
        {
            var hostServices = MefHostServices.Create(MefHostServices.DefaultAssemblies.Concat(
                new[]
                {
                    typeof(CompletionService).Assembly,
                    typeof(MissingStepAnalyzer).Assembly,
                }));

            var workspace = new AdhocWorkspace(hostServices);
            var document = workspace
               .AddProject("TestProject", LanguageNames.CSharp)
               .AddDocument("TestDocument.cs", @"public class Foo 
{
    public void Do() { }
}");

            var service = CompletionService.GetService(document);
            Assert.NotNull(service);

            var compilation = await document.Project
                .GetCompilationAsync();

            var diagnostics = await compilation.WithAnalyzers(
                ImmutableArray.Create<DiagnosticAnalyzer>(new MissingStepAnalyzer()), 
                new AnalyzerOptions(ImmutableArray.Create<AdditionalText>(new AdditionalTextContent("Foo.feature", ""))))
                .GetAnalyzerDiagnosticsAsync();
        }
    }
}
