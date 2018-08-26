using System.Collections.Generic;
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

        public ProjectCollection ProjectCollection => state.Get<ProjectCollection>();

        public ProjectInstance GetProject(string path) => state.Get<ProjectInstance>(path);

        public IEnumerable<ProjectInstance> Projects => state.Get<List<ProjectInstance>>();
    }
}
