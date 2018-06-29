using System;
using System.Collections.Generic;

namespace Gherkinator
{
    public class ScenarioActions<TContext>
    {
        public ScenarioActions(IEnumerable<StepAction<TContext>> given, IEnumerable<StepAction<TContext>> when, IEnumerable<StepAction<TContext>> then)
        {
            Given = given ?? throw new ArgumentNullException(nameof(given));
            When = when ?? throw new ArgumentNullException(nameof(when));
            Then = then ?? throw new ArgumentNullException(nameof(then));
        }

        public IEnumerable<StepAction<TContext>> Given { get; }

        public IEnumerable<StepAction<TContext>> When { get; }

        public IEnumerable<StepAction<TContext>> Then { get; }
    }
}
