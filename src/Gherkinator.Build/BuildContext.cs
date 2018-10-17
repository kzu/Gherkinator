using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace Gherkinator
{
    public class BuildContext : ScenarioContext
    {
        public BuildManager BuildManager
        {
            get => Get<BuildManager>();
            set => Set(value);
        }

        public BuildResult LastBuildResult
        {
            get => Get<BuildResult>();
            set => Set(value);
        }

        public IEnumerable<BuildEventArgs> LastBuildEvents
        {
            get => TryGet<IEnumerable<BuildEventArgs>>(out var value) ? value : Enumerable.Empty<BuildEventArgs>();
            set => Set(value);
        }

        public BuildResult BuildResult(string project, string builtTarget)
            => Get<BuildResult>((project, builtTarget));

        public Process OpenLog(string project, string builtTarget)
        {
            // Opening the log also preserves the files.
            this.KeepTempDir();

            var log = Path.Combine(
                this.GetTempDir(),
                Path.GetDirectoryName(project),
                Path.GetFileNameWithoutExtension(project) + $"-{builtTarget}.binlog");

            if (!File.Exists(log))
                throw new FileNotFoundException($"Could not find log file for project {project} and target {builtTarget} at {log}", log);

            var process = Process.Start(log);

            process.WaitForInputIdle();

            return process;
        }

        public (BuildResult Result, IEnumerable<BuildEventArgs> Events) Build(string project, string target = null, params (string key, string value)[] properties)
            => Run(Path.Combine(this.GetTempDir(), project), target, properties.ToDictionary(x => x.key, x => x.value));

        public (BuildResult Result, IEnumerable<BuildEventArgs> Events) Build(string project, string target = null, Dictionary<string, string> globalProperties = null)
            => Run(Path.Combine(this.GetTempDir(), project), target, globalProperties);


        (BuildResult Result, IEnumerable<BuildEventArgs> Events) Run(string project, string target, Dictionary<string, string> globalProperties = null)
        {
            CallContext.SetData("Build.Project", project);
            CallContext.SetData("Build.Target", target);

            globalProperties = globalProperties ?? new Dictionary<string, string>();
            if (TryGet<Dictionary<string, string>>(out var properties))
            {
                foreach (var pair in properties)
                    globalProperties[pair.Key] = pair.Value;
            }

            globalProperties["ForceReEvaluationWithGuid"] = Guid.NewGuid().ToString();

            var collection = new ProjectCollection(globalProperties);
            var evaluated = collection.LoadProject(project);
            var manager = GetOrSet(() => new BuildManager(Get<string>("Scenario.Name")));
            var instance = manager.GetProjectInstanceForBuild(evaluated);

            var request = new BuildRequestData(
                instance,
                target == null ? new string[0] : new[] { target },
                null,
                BuildRequestDataFlags.ClearCachesAfterBuild |
                BuildRequestDataFlags.ProvideProjectStateAfterBuild |
                BuildRequestDataFlags.ReplaceExistingProjectInstance);

            var eventsLogger = new BuildEventsLogger();
            var parameters = new BuildParameters
            {
                DisableInProcNode = false,
                EnableNodeReuse = false,
                ShutdownInProcNodeOnBuildFinish = true,
                ResetCaches = true,
                MaxNodeCount = 1,
                UseSynchronousLogging = true,
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

            // Expose as "latest build result" directly
            LastBuildResult = result;
            // As well as project/target tuple
            var projectPath = project.Substring(this.GetTempDir().Length + 1);
            Set((projectPath, target), result);
            LastBuildEvents = eventsLogger.Events;

            if (Debugger.IsAttached || TryGet("Build.OpenLogs", out bool value) && value)
                OpenLog(project, target);

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
