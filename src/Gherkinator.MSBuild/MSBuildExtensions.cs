using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Gherkin.Ast;
using Gherkinator.Sdk;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Xunit;

namespace Gherkinator
{
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static class MSBuildExtensions
    {
        public static ScenarioBuilder UseMSBuild(this ScenarioBuilder builder)
            => builder
            .UseFiles()
            .BeforeWhen(LoadProjects)
            .Fallback(OnFallback)
            .OnDispose(state => state.Get<BuildManager>()?.Dispose());

        static StepAction OnFallback(Step step)
        {
            switch (step.Text.Trim().ToLowerInvariant())
            {
                case "restoring packages":
                    return new StepAction(step, RestorePackages);
                case "restore succeeds":
                    return new StepAction(step, VerifyRestore);
                default:
                    break;
            }

            return null;
        }

        static void LoadProjects(ScenarioState state)
        {
            var dir = state.GetTempDir();
            var manager = new BuildManager(state.Scenario.Name);
            state.Set(manager);

            var projects = new List<ProjectInstance>();
            var collection = new ProjectCollection();

            foreach (var file in Directory
                .EnumerateFiles(state.GetTempDir(), "*.csproj")
                .Concat(Directory
                .EnumerateFiles(state.GetTempDir(), "*.vbproj")))
            {
                projects.Add(manager.GetProjectInstanceForBuild(new Project(file, null, null, collection)));
            }

            state.Set(collection);
            state.Set(projects);
        }

        static void RestorePackages(StepContext context)
        {
            var projects = context.State.Get<List<ProjectInstance>>();
            if (projects == null)
                return;

            foreach (var project in projects)
            {
                Run(context, project, "Restore");
            }
        }

        static void VerifyRestore(StepContext context)
        {
            var projects = context.State.Get<List<ProjectInstance>>();
            if (projects == null)
                return;

            foreach (var project in projects)
            {
                var result = context.State.Get<BuildResult>((project, "Restore"));
                Assert.NotNull(result);

#if DEBUG
                // Automatically launch the binlog on failures, for debug builds
                if (result.OverallResult != BuildResultCode.Success)
                    Process.Start(Path.ChangeExtension(project.FullPath, $"-Restore.binlog"));
#endif

                Assert.Equal(BuildResultCode.Success, result.OverallResult);
            }
        }

        static void Run(StepContext context, ProjectInstance project, string target)
        {
            var request = new BuildRequestData(project, new[] { target });
            var parameters = new BuildParameters
            {
                DisableInProcNode = false,
                EnableNodeReuse = false,
                ShutdownInProcNodeOnBuildFinish = false,
                LogInitialPropertiesAndItems = true,
                LogTaskInputs = true,
                Loggers = new ILogger[]
                {
                    new Microsoft.Build.Logging.StructuredLogger.StructuredLogger
                    {
                        Verbosity = LoggerVerbosity.Diagnostic,
                        Parameters = Path.ChangeExtension(project.FullPath, $"-{target}.binlog")
                    }
                }
            };

            var result = context.State.Get<BuildManager>().Build(parameters, request);

            context.State.Set((project, target), result);
        }
    }
}
