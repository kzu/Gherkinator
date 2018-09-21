using System;
using System.Linq;
using Gherkin.Ast;

namespace Gherkinator
{
    public class ScenarioRunner
    {
        public event EventHandler<ScenarioState> BeforeGiven;
        public event EventHandler<ScenarioState> AfterGiven;
        public event EventHandler<ScenarioState> BeforeWhen;
        public event EventHandler<ScenarioState> AfterWhen;
        public event EventHandler<ScenarioState> BeforeThen;
        public event EventHandler<ScenarioState> AfterThen;

        public ScenarioRunner(Scenario scenario)
            => Scenario = scenario ?? throw new ArgumentNullException(nameof(scenario));

        public Scenario Scenario { get; set; }

        public ScenarioState Run(ScenarioActions actions)
        {
            var state = new ScenarioState(new Scenario(
                Scenario.Tags.ToArray(), Scenario.Location, Scenario.Keyword, Scenario.Name, Scenario.Description,
                actions
                    .Given.Select(x => x.Step)
                    .Concat(actions.When.Select(x => x.Step))
                    .Concat(actions.Then.Select(x => x.Step))
                    .ToArray()));

            CallContext.SetData(state);

            BeforeGiven?.Invoke(this, state);
            foreach (var callback in actions.BeforeGiven)
                callback.Invoke(state);

            foreach (var action in actions.Given)
                action.Action.Invoke(new StepContext(Scenario, action.Step, state));

            AfterGiven?.Invoke(this, state);
            foreach (var callback in actions.AfterGiven)
                callback.Invoke(state);

            BeforeWhen?.Invoke(this, state);
            foreach (var callback in actions.BeforeWhen)
                callback.Invoke(state);

            foreach (var action in actions.When)
                action.Action.Invoke(new StepContext(Scenario, action.Step, state));

            AfterWhen?.Invoke(this, state);
            foreach (var callback in actions.AfterWhen)
                callback.Invoke(state);

            BeforeThen?.Invoke(this, state);
            foreach (var callback in actions.BeforeThen)
                callback.Invoke(state);

            foreach (var action in actions.Then)
                action.Action.Invoke(new StepContext(Scenario, action.Step, state));

            AfterThen?.Invoke(this, state);
            foreach (var callback in actions.AfterThen)
                callback.Invoke(state);

            return state;
        }
    }
}
