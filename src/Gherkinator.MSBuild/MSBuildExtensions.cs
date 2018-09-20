using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
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
        public static ScenarioBuilder UseMSBuild(this ScenarioBuilder builder, bool keepTempDir = false)
            => builder
            .UseFiles(keepTempDir)
            .Fallback(OnFallback)
            .AfterThen(state => state.Get<BuildManager>()?.Dispose());

        public static MSBuildState MSBuild(this ScenarioState state) => new MSBuildState(state);

        public static (BuildResult Result, IEnumerable<BuildEventArgs> Events) Build(this StepContext context, string project, string target = null, params (string key, string value)[] properties)
            => Run(context, Path.Combine(context.State.GetTempDir(), project), target, properties.ToDictionary(x => x.key, x => x.value));

        public static (BuildResult Result, IEnumerable<BuildEventArgs> Events) Build(this StepContext context, string project, string target = null, Dictionary<string, string> globalProperties = null)
            => Run(context, Path.Combine(context.State.GetTempDir(), project), target, globalProperties);

        static StepAction OnFallback(Step step)
        {
            //switch (step.Text.Trim().ToLowerInvariant())
            //{
            //}

            return null;
        }

        static (BuildResult Result, IEnumerable<BuildEventArgs> Events) Run(StepContext context, string project, string target, Dictionary<string, string> globalProperties = null)
        {
            CallContext<string>.SetData("Build.Project", project);
            CallContext<string>.SetData("Build.Target", target);

            globalProperties = globalProperties ?? new Dictionary<string, string>();
            if (context.State.TryGet<Dictionary<string, string>>(out var properties))
            {
                foreach (var pair in properties)
                    globalProperties[pair.Key] = pair.Value;
            }

            globalProperties.Add("ForceReEvaluateWithARandomGuid", Guid.NewGuid().ToString());

            var collection = new ProjectCollection(globalProperties);
            var evaluated = collection.LoadProject(project);
            var manager = context.State.GetOrSet(() => new BuildManager(context.Scenario.Name));
            var instance = manager.GetProjectInstanceForBuild(evaluated);

            var request = new BuildRequestData(
                instance,
                target == null ? new string[0] : new[] { target });

            var eventsLogger = new BuildEventsLogger();
            var parameters = new BuildParameters
            {
                DisableInProcNode = false,
                EnableNodeReuse = false,
                ShutdownInProcNodeOnBuildFinish = true,
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

            var result = manager.Build(parameters, request);
            result.ProjectStateAfterBuild = instance.DeepCopy(true);

            // Expose as "latest build result" directly
            context.State.MSBuild().LastBuildResult = result;
            // As well as project/target tuple
            var projectPath = project.Substring(context.State.GetTempDir().Length + 1);
            context.State.Set((projectPath, target), result);
            context.State.MSBuild().LastBuildEvents = eventsLogger.Events;

            manager.ShutdownAllNodes();
            collection.Dispose();

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
