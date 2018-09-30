using System;

namespace Gherkinator
{
    /// <summary>
    /// Represents a configured action that will run for the specified 
    /// step.
    /// </summary>
    public class StepAction<TContext> where TContext : ScenarioContext
    {
        public StepAction(string name, Action<TContext> action)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public string Name { get; }

        public Action<TContext> Action { get; }
    }

    /// <summary>
    /// A typed action that sets the result value as a state entry in the
    /// <see cref="TContext"/>.
    /// </summary>
    public class StepAction<TContext, TResult> : StepAction<TContext> where TContext : ScenarioContext
    {
        public StepAction(string name, Func<TContext, TResult> action)
            : base(name, c => c.Set(action(c))) { }

        public StepAction(string name, Func<TContext, (string key, TResult value)> action)
            : base(name, c =>
            {
                var (key, value) = action(c);
                c.Set(key, value);
            })
        {
        }

        public StepAction(string name, Func<TContext, (object key, TResult value)> action)
            : base(name, c =>
            {
                var (key, value) = action(c);
                c.Set(key, value);
            })
        {
        }
    }
}
