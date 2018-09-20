using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace Gherkinator
{
    public class MSBuildState
    {
        readonly ScenarioState state;

        internal MSBuildState(ScenarioState state) => this.state = state;

        public BuildManager BuildManager
        {
            get => state.Get<BuildManager>();
            set => state.Set(value);
        }

        public BuildResult LastBuildResult
        {
            get => state.Get<BuildResult>();
            set => state.Set(value);
        }

        public IEnumerable<BuildEventArgs> LastBuildEvents
        {
            get => state.Get<IEnumerable<BuildEventArgs>>();
            set => state.Set(value);
        }

        public BuildResult BuildResult(string project, string builtTarget)
            => state.Get<BuildResult>((project, builtTarget));

        public Process OpenLog(string project, string builtTarget)
        {
            // Opening the log also preserves the files.
            state.KeepTempDir();

            var log = Path.Combine(
                state.GetTempDir(),
                Path.GetDirectoryName(project),
                Path.GetFileNameWithoutExtension(project) + $"-{builtTarget}.binlog");

            if (!File.Exists(log))
                throw new FileNotFoundException($"Could not find log file for project {project} and target {builtTarget} at {log}", log);

            var process = Process.Start(log);

            process.WaitForInputIdle();

            return process;
        }
    }
}
