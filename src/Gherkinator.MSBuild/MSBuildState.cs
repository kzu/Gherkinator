using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;

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

        public Process OpenLog(string project, string builtTarget)
            => Process.Start(Path.Combine(state.GetTempDir(), Path.ChangeExtension(project, $"-{builtTarget}.binlog")));
    }
}
