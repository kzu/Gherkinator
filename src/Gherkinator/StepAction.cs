using System;

namespace Gherkinator
{
    public class StepAction : StepAction<StepContext>
    {
        public StepAction(string name, Action<StepContext> action) : base(name, action)
        {
        }
    }

    public class StepAction<TContext>
    {
        public StepAction(string name, Action<TContext> action)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public string Name { get; }

        public Action<TContext> Action { get; }
    }
}
