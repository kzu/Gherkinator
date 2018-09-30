using System.Runtime.CompilerServices;
using Microsoft.Build.Execution;

namespace Gherkinator
{
    public class BuildScenario : Scenario<BuildContext>
    {
        /// <summary>
        /// Creates the scenario, optionally passing the scenario name and feature file, 
        /// which are otherwise inferred from the current test method and file.
        /// </summary>
        public BuildScenario(
            string scenarioName = null, string featureFile = null,
            bool keepTempDir = false, bool openLogs = false,
            [CallerMemberName] string testMethod = null, [CallerFilePath] string testFile = null, [CallerLineNumber] int? testLine = null)
            : base(scenarioName, featureFile, testMethod, testFile, testLine)
        {
            this.UseFiles<BuildScenario, BuildContext>(keepTempDir)
                .Sdk.BeforeGiven(c => c.Set("Build.OpenLogs", openLogs))
                .Sdk.AfterThen(c => c.Get<BuildManager>()?.Dispose());
        }

        /// <summary>
        /// Creates the scenario with the specified name and feature file.
        /// </summary>
        public BuildScenario(string scenarioName, string featureFile,
            bool keepTempDir = false, bool openLogs = false)
            : base(scenarioName, featureFile)
        {
            this.UseFiles<BuildScenario, BuildContext>(keepTempDir)
                .Sdk.BeforeGiven(c => c.Set("Build.OpenLogs", openLogs))
                .Sdk.AfterThen(c => c.Get<BuildManager>()?.Dispose());
        }

        protected override BuildContext CreateContext() => new BuildContext();
    }
}
