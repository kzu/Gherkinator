using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gherkin;
using Gherkin.Ast;
using Gherkinator.Properties;

namespace Gherkinator
{
    public class ScenarioRunner : ScenarioRunner<StepContext>
    {
        public ScenarioRunner(string featureFile, string scenarioName) : base(featureFile, scenarioName)
        {
        }

        public ScenarioRunner(Feature feature, string scenarioName, string featureFile = null) : base(feature, scenarioName, featureFile)
        {
        }

        protected override StepContext GetActionContext(ScenarioDefinition scenario, Step step, ScenarioState state)
            => new StepContext(scenario, step, state);
    }

    public abstract class ScenarioRunner<TContext>
    {
        readonly string featureFile;

        public ScenarioRunner(string featureFile, string scenarioName)
        {
            this.featureFile = featureFile;
            Feature = new Parser().Parse(featureFile ?? throw new ArgumentNullException(nameof(featureFile))).Feature;
            Scenario = Scenario = Feature.Children
                .FirstOrDefault(x => x.Name.Equals(scenarioName, StringComparison.OrdinalIgnoreCase)) ??
                throw new ArgumentException(string.Format(Resources.ScenarioNotFoundInFile, scenarioName, featureFile), nameof(scenarioName));
        }

        public ScenarioRunner(Feature feature, string scenarioName, string featureFile = null)
        {
            this.featureFile = featureFile;
            Feature = feature ?? throw new ArgumentNullException(nameof(feature));
            Scenario = Scenario = Feature.Children
                .FirstOrDefault(x => x.Name.Equals(scenarioName, StringComparison.OrdinalIgnoreCase)) ??
                throw new ArgumentException(string.Format(Resources.ScenarioNotFound, scenarioName), nameof(scenarioName));
        }

        public Feature Feature { get; }

        public ScenarioDefinition Scenario { get; set; }

        public virtual void Run(ScenarioActions<TContext> actions)
        {
            var given = new List<Tuple<Step, StepAction<TContext>>>();
            var when = new List<Tuple<Step, StepAction<TContext>>>();
            var then = new List<Tuple<Step, StepAction<TContext>>>();

            var steps = Scenario.Steps;
            var background = Feature.Children.OfType<Background>().FirstOrDefault();
            if (background != null)
                steps = background.Steps.Concat(steps);

            List<Tuple<Step, StepAction<TContext>>> phase = null;
            IEnumerable<StepAction<TContext>> implementation = null;

            foreach (var step in steps)
            {
                var name = step.Text.Trim();
                switch (step.Keyword.Trim().ToLowerInvariant())
                {
                    case "given":
                        phase = given;
                        implementation = actions.Given;
                        break;
                    case "when":
                        phase = when;
                        implementation = actions.When;
                        break;
                    case "then":
                        phase = then;
                        implementation = actions.Then;
                        break;
                    case "and":
                        break;
                    default:
                        throw new NotSupportedException(string.Format(Resources.UnsupportedKeyword, step.Keyword.Trim()));
                }

                phase.Add(Tuple.Create(step,
                    implementation.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ??
                    OnMissing(step)));
            }

            var state = GetInitialState();

            foreach (var action in given)
                action.Item2.Action.Invoke(GetActionContext(Scenario, action.Item1, state));

            foreach (var action in when)
                action.Item2.Action.Invoke(GetActionContext(Scenario, action.Item1, state));

            foreach (var action in then)
                action.Item2.Action.Invoke(GetActionContext(Scenario, action.Item1, state));
        }

        protected virtual ScenarioState GetInitialState() => new ScenarioState();

        protected abstract TContext GetActionContext(ScenarioDefinition scenario, Step step, ScenarioState state);

        /// <summary>
        /// Invoked whenever a scenario step does not have a matching action.
        /// </summary>
        protected virtual StepAction<TContext> OnMissing(Step step)
        {
            if (featureFile != null)
                throw new ArgumentException(
                    string.Format(Resources.MissingActionInFile, step.Text, Scenario.Name, new FileInfo(featureFile).FullName, step.Location.Line, step.Location.Column), 
                    step.Keyword.Trim().ToLowerInvariant());

            throw new ArgumentException(string.Format(Resources.MissingAction, step.Text, Scenario.Name, Feature.Name), step.Keyword.Trim().ToLowerInvariant());
        }
    }
}
