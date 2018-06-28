using System;
using System.Collections.Generic;
using System.Text;

namespace Gherkinator
{
    public class StepAction
    {
        public StepAction(string name, Action<StepContext> action)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public string Name { get; }

        public Action<StepContext> Action { get; }
    }
}
