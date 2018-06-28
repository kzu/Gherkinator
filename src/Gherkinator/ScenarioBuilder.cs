using System;
using System.Collections.Generic;
using System.Linq;
using Gherkin;
using Gherkin.Ast;
using Gherkinator.Properties;

namespace Gherkinator
{
    public class ScenarioBuilder
    {
        IList<StepAction> phase;

        public ScenarioBuilder(string featureFile, string scenarioName)
        {
            FeatureFile = featureFile ?? throw new ArgumentNullException(nameof(featureFile));
            ScenarioName = scenarioName ?? throw new ArgumentNullException(nameof(ScenarioName));

            Feature = new Parser().Parse(featureFile).Feature;
            Scenario = Scenario = Feature.Children
                .FirstOrDefault(x => x.Name.Equals(scenarioName, StringComparison.OrdinalIgnoreCase)) ??
                throw new ArgumentException(string.Format(Resources.ScenarioNotFoundInFile, scenarioName, featureFile), nameof(scenarioName));
        }

        public Feature Feature { get; }

        public string FeatureFile { get; }

        public ScenarioDefinition Scenario { get; set; }

        public string ScenarioName { get; }

        protected IList<StepAction> GivenActions { get; } = new List<StepAction>();

        protected IList<StepAction> WhenActions { get; } = new List<StepAction>();

        protected IList<StepAction> ThenActions { get; } = new List<StepAction>();

        public virtual ScenarioBuilder Given(string name, Action<StepContext> assertion)
        {
            GivenActions.Add(new StepAction(name, assertion));
            phase = GivenActions;
            return this;
        }

        public virtual ScenarioBuilder When(string name, Action<StepContext> assertion)
        {
            WhenActions.Add(new StepAction(name, assertion));
            phase = WhenActions;
            return this;
        }

        public virtual ScenarioBuilder Then(string name, Action<StepContext> assertion)
        {
            ThenActions.Add(new StepAction(name, assertion));
            phase = ThenActions;
            return this;
        }

        public virtual ScenarioBuilder And(string name, Action<StepContext> assertion)
        {
            if (phase == null)
                throw new InvalidOperationException(Resources.AndWithoutPhase);

            phase.Add(new StepAction(name, assertion));
            return this;
        }

        public virtual ScenarioActions Build() => new ScenarioActions(GivenActions, WhenActions, ThenActions);

        public virtual void Run() => new ScenarioRunner(Feature, ScenarioName, FeatureFile).Run(Build());
    }
}
