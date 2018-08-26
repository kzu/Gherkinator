using System;
using System.ComponentModel;
using System.IO;
using Gherkin.Ast;

namespace Gherkinator
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static class UseFilesExtension
    {
        /// <summary>
        /// Gets the temporary directory where files will be written, if any.
        /// </summary>
        public static string GetTempDir(this ScenarioState state)
            => state.GetOrSet("TempDir", () => Path.Combine(Path.GetTempPath(),
                // By default we try to use the test method as set by the Syntax.Scenario call.
                state.GetOrSet("testMethod", () => Guid.NewGuid().ToString())));

        public static ScenarioBuilder UseFiles(this ScenarioBuilder builder)
        {
            return builder.AfterThen(state =>
            {
                var tempDir = state.GetTempDir();
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            })
            .Fallback(OnFallback);
        }

        static StepAction OnFallback(Step step)
        {
            var content = (step.Argument as DocString)?.Content;
            // Value assignment can also be performed inline
            if (content == null && step.Text.Contains("="))
                content = step.Text.Substring(step.Text.IndexOf('=') + 1).Trim('\"', ' ');

            if (content != null && step.Text.Contains("="))
            {
                var path = step.Text.Substring(0, step.Text.IndexOf("=")).Trim();
                if (path.Contains("."))
                {
                    if (step.Keyword.Equals("given", StringComparison.OrdinalIgnoreCase) || 
                        step.Keyword.Equals("when", StringComparison.Ordinal))
                    {
                        return new StepAction(step, context =>
                        {
                            var tempDir = context.State.GetTempDir();
                            Directory.CreateDirectory(tempDir);
                            File.WriteAllText(Path.Combine(tempDir, path), content);
                        });
                    } 
                    else if (step.Keyword.Equals("then", StringComparison.OrdinalIgnoreCase))
                    {
                        return new StepAction(step, context
                            => Xunit.Assert.Equal(content, File.ReadAllText(
                                Path.Combine(context.State.GetTempDir(), path))));
                    }
                }
            }

            return null;
        }
    }
}
