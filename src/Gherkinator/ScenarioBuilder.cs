using System;
using System.Collections.Generic;
using System.Linq;
using Gherkin;
using Gherkin.Ast;
using Gherkinator.Properties;

namespace Gherkinator
{
    public class ScenarioBuilder : ScenarioBuilder<StepContext>
    {
        public ScenarioBuilder(string featureFile, string scenarioName) : base(featureFile, scenarioName)
        {
        }

        public new ScenarioBuilder Given(string name, Action<StepContext> action) => (ScenarioBuilder)base.Given(name, action);

        public new ScenarioBuilder When(string name, Action<StepContext> action) => (ScenarioBuilder)base.When(name, action);

        public new ScenarioBuilder Then(string name, Action<StepContext> action) => (ScenarioBuilder)base.Then(name, action);

        public new ScenarioBuilder And(string name, Action<StepContext> action) => (ScenarioBuilder)base.And(name, action);

        public virtual void Run() => new ScenarioRunner(Feature, ScenarioName, FeatureFile).Run(Build());
    }

    public abstract class ScenarioBuilder<TContext>
    {
        IList<StepAction<TContext>> phase;

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

        protected IList<StepAction<TContext>> GivenActions { get; } = new List<StepAction<TContext>>();

        protected IList<StepAction<TContext>> WhenActions { get; } = new List<StepAction<TContext>>();

        protected IList<StepAction<TContext>> ThenActions { get; } = new List<StepAction<TContext>>();

        public virtual ScenarioBuilder<TContext> Given(string name, Action<TContext> action)
        {
            GivenActions.Add(CreateAction(name, action));
            phase = GivenActions;
            return this;
        }

        public virtual ScenarioBuilder<TContext> When(string name, Action<TContext> action)
        {
            WhenActions.Add(CreateAction(name, action));
            phase = WhenActions;
            return this;
        }

        public virtual ScenarioBuilder<TContext> Then(string name, Action<TContext> action)
        {
            ThenActions.Add(CreateAction(name, action));
            phase = ThenActions;
            return this;
        }

        public virtual ScenarioBuilder<TContext> And(string name, Action<TContext> action)
        {
            if (phase == null)
                throw new InvalidOperationException(Resources.AndWithoutPhase);

            phase.Add(CreateAction(name, action));
            return this;
        }

        public virtual ScenarioActions<TContext> Build() => new ScenarioActions<TContext>(GivenActions, WhenActions, ThenActions);

        protected virtual StepAction<TContext> CreateAction(string name, Action<TContext> action) => new StepAction<TContext>(name, action);
    }
}
