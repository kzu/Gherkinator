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
        public event EventHandler<ScenarioState> BeforeGiven;
        public event EventHandler<ScenarioState> AfterGiven;
        public event EventHandler<ScenarioState> BeforeWhen;
        public event EventHandler<ScenarioState> AfterWhen;
        public event EventHandler<ScenarioState> BeforeThen;
        public event EventHandler<ScenarioState> AfterThen;

        public ScenarioRunner(ScenarioDefinition scenario)
            => Scenario = scenario ?? throw new ArgumentNullException(nameof(scenario));

        public ScenarioDefinition Scenario { get; set; }

        public void Run(ScenarioActions actions)
        {
            var state = new ScenarioState();

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
        }
    }
}
