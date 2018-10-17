using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gherkinator.CodeAnalysis;
using Gherkinator.Completion;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Gherkinator.Tests
{
    public class CompletionTests
    {
        [Theory]
        [InlineData(@"using Gherkinator;
public class Foo 
{
    public void Do() 
    {
        new Scenario(""scenario"", ""feature"")
            .Given(""`"", _ => { });
    }
}")]
        public async Task create_project(string code)
        {
            var hostServices = MefHostServices.Create(MefHostServices.DefaultAssemblies.Concat(
                new[]
                {
                    typeof(CompletionService).Assembly,
                    typeof(StepCompletionProvider).Assembly,
                }));

            var workspace = new AdhocWorkspace(hostServices);
            var document = workspace
               .AddProject("TestProject", LanguageNames.CSharp)
               .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
               .WithMetadataReferences(new MetadataReference[]
               {
                   MetadataReference.CreateFromFile(Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\.nuget\packages\netstandard.library\2.0.3\build\netstandard2.0\ref\netstandard.dll")),
                   MetadataReference.CreateFromFile("Gherkinator.dll"),
                   MetadataReference.CreateFromFile("Gherkin.dll"),
               })
               .AddAdditionalDocument("feature.feature", @"Feature: feature
    Scenario: scenario
        Given foo")
               .Project
               .AddDocument("TestDocument.cs", code);

            var service = CompletionService.GetService(document);
            Assert.NotNull(service);

            var caret = code.Replace(Environment.NewLine, "\\r").IndexOf('`');
            Assert.NotEqual(-1, caret);

            var completions = await service.GetCompletionsAsync(document, caret);

            Assert.NotNull(completions);
            Assert.NotEmpty(completions.Items);
        }

        public static int FindCaret(string code)
        {
            var span = SourceText.From(code);
            for (var i = 0; i < span.Length; i++)
            {
                if (span[i] == '`')
                    return i;
            }

            return -1;
        }
    }
}
