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

        public static void KeepTempDir(this ScenarioState state) => state.Set(nameof(KeepTempDir), true);

        public static ScenarioBuilder UseFiles(this ScenarioBuilder builder, bool keepTempDir = false)
        {
            return builder
                .BeforeGiven(state => CleanDirectory(state.GetTempDir()))
                .AfterThen(state =>
                {
                    var tempDir = state.GetTempDir();
                    var shouldKeep = keepTempDir || (state.TryGet<bool>(nameof(KeepTempDir), out var keepTemp) && keepTemp);
                    if (!shouldKeep)
                        CleanDirectory(tempDir);
                })
                .Fallback(OnFallback);
        }

        static void CleanDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                foreach (var file in Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (IOException)
                    {
                        Console.WriteLine("Failed to delete ", file);
                    }
                }

                try
                {
                    Directory.Delete(directory, true);
                }
                catch (IOException)
                {
                    Console.WriteLine("Failed to delete ", directory);
                }
            }
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
