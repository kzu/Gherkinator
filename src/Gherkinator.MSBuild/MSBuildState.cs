using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Evaluation;
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
            => Process.Start(Path.Combine(state.GetTempDir(), Path.ChangeExtension(project, $"-{builtTarget}.binlog")));
    }
}
