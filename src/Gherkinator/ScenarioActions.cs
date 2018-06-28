using System;
using System.Collections.Generic;

namespace Gherkinator
{
    public class ScenarioActions
    {
        public ScenarioActions(IEnumerable<StepAction> given, IEnumerable<StepAction> when, IEnumerable<StepAction> then)
        {
            Given = given ?? throw new ArgumentNullException(nameof(given));
            When = when ?? throw new ArgumentNullException(nameof(when));
            Then = then ?? throw new ArgumentNullException(nameof(then));
        }

        public IEnumerable<StepAction> Given { get; }
        public IEnumerable<StepAction> When { get; }
        public IEnumerable<StepAction> Then { get; }
    }
}
