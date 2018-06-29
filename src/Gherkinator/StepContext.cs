using System;
using Gherkin.Ast;

namespace Gherkinator
{
    public class StepContext
    {
        public StepContext(ScenarioDefinition scenario, Step step, ScenarioState state)
        {
            Scenario = scenario ?? throw new ArgumentNullException(nameof(scenario));
            Step = step ?? throw new ArgumentNullException(nameof(step));
            State = state ?? throw new ArgumentNullException(nameof(state));
        }

        public ScenarioDefinition Scenario { get; }

        public Step Step { get; set; }

        public ScenarioState State { get; set; }
    }
}