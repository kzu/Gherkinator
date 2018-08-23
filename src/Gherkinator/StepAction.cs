using System;
using Gherkin.Ast;

namespace Gherkinator
{
    public class StepAction
    {
        public StepAction(string name, Action<StepContext> action)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public StepAction(Step step, Action<StepContext> action)
        {
            Step = step ?? throw new ArgumentNullException(nameof(step));
            Name = step.Text.Trim();
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public string Name { get; }

        public Action<StepContext> Action { get; }

        internal Step Step { get; set; }
    }
}