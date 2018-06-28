using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gherkin;
using Gherkin.Ast;
using Gherkinator.Properties;

namespace Gherkinator
{
    public class ScenarioRunner
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

        public virtual void Run(ScenarioActions actions)
        {
            var given = new List<Tuple<Step, StepAction>>();
            var when = new List<Tuple<Step, StepAction>>();
            var then = new List<Tuple<Step, StepAction>>();

            var steps = Scenario.Steps;
            var background = Feature.Children.OfType<Background>().FirstOrDefault();
            if (background != null)
                steps = background.Steps.Concat(steps);

            foreach (var step in steps)
            {
                var name = step.Text.Trim();
                switch (step.Keyword.Trim().ToLowerInvariant())
                {
                    case "given":
                        given.Add(Tuple.Create(step,
                            actions.Given.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ?? 
                            OnMissing(step)));
                        break;
                    case "when":
                        when.Add(Tuple.Create(step, 
                            actions.When.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ??
                            OnMissing(step)));
                        break;
                    case "then":
                        then.Add(Tuple.Create(step, 
                            actions.Then.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ??
                            OnMissing(step)));
                        break;
                    default:
                        break;
                }
            }

            var state = new ScenarioState();

            foreach (var action in given)
                action.Item2.Action.Invoke(new StepContext(Scenario, action.Item1, state));

            foreach (var action in when)
                action.Item2.Action.Invoke(new StepContext(Scenario, action.Item1, state));

            foreach (var action in then)
                action.Item2.Action.Invoke(new StepContext(Scenario, action.Item1, state));
        }

        /// <summary>
        /// Invoked whenever a scenario step does not have a matching action.
        /// </summary>
        protected virtual StepAction OnMissing(Step step)
        {
            if (featureFile != null)
                throw new ArgumentException(
                    string.Format(Resources.MissingActionInFile, step.Text, Scenario.Name, new FileInfo(featureFile).FullName, step.Location.Line, step.Location.Column), 
                    step.Keyword.Trim().ToLowerInvariant());

            throw new ArgumentException(string.Format(Resources.MissingAction, step.Text, Scenario.Name, Feature.Name), step.Keyword.Trim().ToLowerInvariant());
        }
    }
}
