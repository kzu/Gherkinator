using System;
using System.Collections.Generic;

namespace Gherkinator
{
    public class ScenarioActions
    {
        public ScenarioActions(
            IEnumerable<StepAction> given, 
            IEnumerable<StepAction> when, 
            IEnumerable<StepAction> then,
            IEnumerable<Action<ScenarioState>> beforeGiven = null,
            IEnumerable<Action<ScenarioState>> afterGiven = null,
            IEnumerable<Action<ScenarioState>> beforeWhen = null,
            IEnumerable<Action<ScenarioState>> afterWhen = null,
            IEnumerable<Action<ScenarioState>> beforeThen = null,
            IEnumerable<Action<ScenarioState>> afterThen = null,
            IEnumerable<Action<ScenarioState>> onDispose = null)
        {
            Given = given ?? throw new ArgumentNullException(nameof(given));
            When = when ?? throw new ArgumentNullException(nameof(when));
            Then = then ?? throw new ArgumentNullException(nameof(then));

            BeforeGiven = beforeGiven ?? Array.Empty<Action<ScenarioState>>();
            AfterGiven = afterGiven ?? Array.Empty<Action<ScenarioState>>();
            BeforeWhen = beforeWhen ?? Array.Empty<Action<ScenarioState>>();
            AfterWhen = afterWhen ?? Array.Empty<Action<ScenarioState>>();
            BeforeThen = beforeThen ?? Array.Empty<Action<ScenarioState>>();
            AfterThen = afterThen ?? Array.Empty<Action<ScenarioState>>();

            OnDispose = onDispose ?? Array.Empty<Action<ScenarioState>>();
        }

        public IEnumerable<StepAction> Given { get; }
        public IEnumerable<StepAction> When { get; }
        public IEnumerable<StepAction> Then { get; }

        internal IEnumerable<Action<ScenarioState>> BeforeGiven { get; }
        internal IEnumerable<Action<ScenarioState>> AfterGiven { get; }
        internal IEnumerable<Action<ScenarioState>> BeforeWhen { get; }
        internal IEnumerable<Action<ScenarioState>> AfterWhen { get; }
        internal IEnumerable<Action<ScenarioState>> BeforeThen { get; }
        internal IEnumerable<Action<ScenarioState>> AfterThen { get; }

        internal IEnumerable<Action<ScenarioState>> OnDispose { get; }
    }
}
