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

    /// <summary>
    /// A typed action that sets the result value as a state entry in the
    /// <see cref="StepContext.State"/>.
    /// </summary>
    public class StepAction<TResult> : StepAction
    {
        public StepAction(string name, Func<StepContext, TResult> action)
            : base(name, c => c.State.Set(action(c))) { }

        public StepAction(string name, Func<StepContext, (string key, TResult value)> action)
            : base(name, c =>
            {
                var (key, value) = action(c);
                c.State.Set(key, value);
            })
        {
        }

        public StepAction(string name, Func<StepContext, (object key, TResult value)> action)
            : base(name, c =>
            {
                var (key, value) = action(c);
                c.State.Set(key, value);
            })
        {
        }
    }
}