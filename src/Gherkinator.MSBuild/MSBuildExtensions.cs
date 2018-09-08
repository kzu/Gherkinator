using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Gherkin.Ast;
using Gherkinator.Sdk;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

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

        public static (BuildResult result, IEnumerable<BuildEventArgs> events) Build(this StepContext context, string project, string target = null, Dictionary<string, string> globalProperties = null)
            => Run(context, Path.Combine(context.State.GetTempDir(), project), target, globalProperties);

        static StepAction OnFallback(Step step)
        {
            switch (step.Text.Trim().ToLowerInvariant())
            {
            }

            return null;
        }

        static (BuildResult result, IEnumerable<BuildEventArgs> events) Run(StepContext context, string project, string target, Dictionary<string, string> globalProperties = null)
        {
            var collection = context.State.GetOrSet(() => new ProjectCollection());

            var request = new BuildRequestData(
                project ?? throw new ArgumentNullException(nameof(project)),
                globalProperties ?? new Dictionary<string, string>(),
                null,
                target == null ? new string[0] : new[] { target },
                null
                );

            var eventsLogger = new BuildEventsLogger();
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
                        Parameters = Path.Combine(
                            Path.GetDirectoryName(project), 
                            Path.GetFileNameWithoutExtension(project) + $"-{target}.binlog")
                    }, 
                    eventsLogger
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
            context.State.MSBuild().LastBuildEvents = eventsLogger.Events;

            return (result, eventsLogger.Events);
        }

        class BuildEventsLogger : ILogger
        {
            IEventSource eventSource;

            public LoggerVerbosity Verbosity { get; set; }

            public string Parameters { get; set; }

            public List<BuildEventArgs> Events { get; } = new List<BuildEventArgs>();

            public void Initialize(IEventSource eventSource)
            {
                this.eventSource = eventSource;
                eventSource.AnyEventRaised += OnEvent;
            }

            public void Shutdown() => eventSource.AnyEventRaised -= OnEvent;

            void OnEvent(object sender, BuildEventArgs e) => Events.Add(e);
        }
    }
}
