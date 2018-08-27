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
            .Fallback(OnFallback)
            .AfterThen(state => state.Get<BuildManager>()?.Dispose());

        public static MSBuildState MSBuild(this ScenarioState state) => new MSBuildState(state);

        public static BuildResult Build(this StepContext context, string project, string target = null, Dictionary<string, string> globalProperties = null)
            => Run(context, Path.Combine(context.State.GetTempDir(), project), target, globalProperties);

        static StepAction OnFallback(Step step)
        {
            switch (step.Text.Trim().ToLowerInvariant())
            {
            }

            return null;
        }

        static BuildResult Run(StepContext context, string project, string target, Dictionary<string, string> globalProperties = null)
        {
            var collection = context.State.GetOrSet(() => new ProjectCollection());

            var request = new BuildRequestData(
                project ?? throw new ArgumentNullException(nameof(project)),
                globalProperties ?? new Dictionary<string, string>(),
                null,
                target == null ? new string[0] : new[] { target },
                null
                );

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
                        Parameters = Path.ChangeExtension(project, $"-{target}.binlog")
                    }
                }
            };

            if (globalProperties != null)
                parameters.GlobalProperties = globalProperties;

            var manager = context.State.GetOrSet(() => new BuildManager(context.Scenario.Name));
            var result = manager.Build(parameters, request);
            
            // Expose as "latest build result" directly
            context.State.MSBuild().LastBuildResult = result;
            // As well as project/target tuple
            var projectPath = project.Substring(context.State.GetTempDir().Length + 1);
            context.State.Set((projectPath, target), result);

            return result;
        }
    }
}
