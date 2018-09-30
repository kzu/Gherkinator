using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Gherkin;
using Gherkinator.Properties;
using Ast = Gherkin.Ast;

namespace Gherkinator
{
    /// <summary>
    /// A simple scenario that uses <see cref="ScenarioContext"/>.
    /// </summary>
    public class Scenario : Scenario<ScenarioContext>
    {
        /// <summary>
        /// Creates the scenario, optionally passing the scenario name and feature file, 
        /// which are otherwise inferred from the current test method and file.
        /// </summary>
        public Scenario(string scenarioName = null, string featureFile = null,
            [CallerMemberName] string testMethod = null, [CallerFilePath] string testFile = null, [CallerLineNumber] int? testLine = null)
            : base(scenarioName, featureFile, testMethod, testFile, testLine)
        {
        }

        /// <summary>
        /// Creates the scenario with the specified name and feature file.
        /// </summary>
        public Scenario(string scenarioName, string featureFile)
            : base(scenarioName, featureFile)
        {
        }

        /// <summary>
        /// Creates the initial empty <see cref="ScenarioContext"/>.
        /// </summary>
        protected override ScenarioContext CreateContext() => new ScenarioContext();
    }

    /// <summary>
    /// Base class containing the logic for configuring steps for a scenario run.
    /// </summary>
    public abstract class Scenario<TContext> where TContext : ScenarioContext
    {
        private readonly string featureFile;
        private readonly string scenarioName;

        private readonly Ast.Feature feature;
        private readonly Ast.Scenario scenario;

        private readonly List<Func<Ast.Step, StepAction<TContext>>> fallbacks = new List<Func<Ast.Step, StepAction<TContext>>>();

        private readonly List<StepAction<TContext>> given = new List<StepAction<TContext>>();
        private readonly List<StepAction<TContext>> when = new List<StepAction<TContext>>();
        private readonly List<StepAction<TContext>> then = new List<StepAction<TContext>>();

        private readonly List<Action<TContext>> beforeGiven = new List<Action<TContext>>();
        private readonly List<Action<TContext>> afterGiven = new List<Action<TContext>>();
        private readonly List<Action<TContext>> beforeWhen = new List<Action<TContext>>();
        private readonly List<Action<TContext>> afterWhen = new List<Action<TContext>>();
        private readonly List<Action<TContext>> beforeThen = new List<Action<TContext>>();
        private readonly List<Action<TContext>> afterThen = new List<Action<TContext>>();

        private List<StepAction<TContext>> currentPhase;

        /// <summary>
        /// Creates the scenario, optionally passing the scenario name and feature file, 
        /// which are otherwise inferred from the current test method and file.
        /// </summary>
        public Scenario(string scenarioName = null, string featureFile = null, 
            [CallerMemberName] string testMethod = null, [CallerFilePath] string testFile = null, [CallerLineNumber] int? testLine = null)
            : this(scenarioName ?? testMethod.Replace('_', ' '), featureFile ?? Path.GetFileNameWithoutExtension(testFile) + ".feature")
        {
        }

        /// <summary>
        /// Creates the scenario with the specified name and feature file.
        /// </summary>
        public Scenario(string scenarioName, string featureFile)
        {
            this.featureFile = featureFile ?? throw new ArgumentNullException(nameof(featureFile));
            this.scenarioName = scenarioName ?? throw new ArgumentNullException(nameof(scenarioName));

            feature = new Parser().Parse(featureFile).Feature;
            scenario = feature.Children
                .OfType<Ast.Scenario>()
                .FirstOrDefault(x => x.Name.Equals(scenarioName, StringComparison.OrdinalIgnoreCase)) ??
                throw new ArgumentException(string.Format(Resources.ScenarioNotFoundInFile, scenarioName, featureFile), nameof(scenarioName));
        }

        #region Given

        public virtual Scenario<TContext> Given(string name, Action<TContext> action)
        {
            given.Add(new StepAction<TContext>(
                name ?? throw new ArgumentNullException(nameof(name)),
                action ?? throw new ArgumentNullException(nameof(action))));
            currentPhase = given;
            return this;
        }

        public virtual Scenario<TContext> Given<TResult>(string name, Func<TContext, TResult> function)
        {
            given.Add(new StepAction<TContext, TResult>(
                name ?? throw new ArgumentNullException(nameof(name)),
                function ?? throw new ArgumentNullException(nameof(function))));
            currentPhase = given;
            return this;
        }

        public virtual Scenario<TContext> Given<TResult>(string name, Func<TContext, (string key, TResult value)> function)
        {
            given.Add(new StepAction<TContext, TResult>(
                name ?? throw new ArgumentNullException(nameof(name)),
                function ?? throw new ArgumentNullException(nameof(function))));
            currentPhase = given;
            return this;
        }

        public virtual Scenario<TContext> Given<TResult>(string name, Func<TContext, (object key, TResult value)> function)
        {
            given.Add(new StepAction<TContext, TResult>(
                name ?? throw new ArgumentNullException(nameof(name)),
                function ?? throw new ArgumentNullException(nameof(function))));
            currentPhase = given;
            return this;
        }

        #endregion

        #region When

        public virtual Scenario<TContext> When(string name, Action<TContext> action)
        {
            when.Add(new StepAction<TContext>(
                name ?? throw new ArgumentNullException(nameof(name)),
                action ?? throw new ArgumentNullException(nameof(action))));
            currentPhase = when;
            return this;
        }

        public virtual Scenario<TContext> When<TResult>(string name, Func<TContext, TResult> function)
        {
            when.Add(new StepAction<TContext, TResult>(
                name ?? throw new ArgumentNullException(nameof(name)),
                function ?? throw new ArgumentNullException(nameof(function))));
            currentPhase = when;
            return this;
        }

        public virtual Scenario<TContext> When<TResult>(string name, Func<TContext, (string key, TResult value)> function)
        {
            when.Add(new StepAction<TContext, TResult>(
                name ?? throw new ArgumentNullException(nameof(name)),
                function ?? throw new ArgumentNullException(nameof(function))));
            currentPhase = when;
            return this;
        }

        public virtual Scenario<TContext> When<TResult>(string name, Func<TContext, (object key, TResult value)> function)
        {
            when.Add(new StepAction<TContext, TResult>(
                name ?? throw new ArgumentNullException(nameof(name)),
                function ?? throw new ArgumentNullException(nameof(function))));
            currentPhase = when;
            return this;
        }

        #endregion

        #region Then

        public virtual Scenario<TContext> Then(string name, Action<TContext> action)
        {
            then.Add(new StepAction<TContext>(
                name ?? throw new ArgumentNullException(nameof(name)),
                action ?? throw new ArgumentNullException(nameof(action))));
            currentPhase = then;
            return this;
        }

        public virtual Scenario<TContext> Then<TResult>(string name, Func<TContext, TResult> function)
        {
            then.Add(new StepAction<TContext, TResult>(
                name ?? throw new ArgumentNullException(nameof(name)),
                function ?? throw new ArgumentNullException(nameof(function))));
            currentPhase = then;
            return this;
        }

        public virtual Scenario<TContext> Then<TResult>(string name, Func<TContext, (string key, TResult value)> function)
        {
            then.Add(new StepAction<TContext, TResult>(
                name ?? throw new ArgumentNullException(nameof(name)),
                function ?? throw new ArgumentNullException(nameof(function))));
            currentPhase = then;
            return this;
        }

        public virtual Scenario<TContext> Then<TResult>(string name, Func<TContext, (object key, TResult value)> function)
        {
            then.Add(new StepAction<TContext, TResult>(
                name ?? throw new ArgumentNullException(nameof(name)),
                function ?? throw new ArgumentNullException(nameof(function))));
            currentPhase = then;
            return this;
        }

        #endregion

        #region And

        public virtual Scenario<TContext> And(string name, Action<TContext> action)
        {
            if (currentPhase == null)
                throw new InvalidOperationException(Resources.AndWithoutPhase);

            currentPhase.Add(new StepAction<TContext>(
                name ?? throw new ArgumentNullException(nameof(name)),
                action ?? throw new ArgumentNullException(nameof(action))));
            return this;
        }

        public virtual Scenario<TContext> And<TResult>(string name, Func<TContext, TResult> function)
        {
            if (currentPhase == null)
                throw new InvalidOperationException(Resources.AndWithoutPhase);

            currentPhase.Add(new StepAction<TContext, TResult>(
                name ?? throw new ArgumentNullException(nameof(name)),
                function ?? throw new ArgumentNullException(nameof(function))));
            return this;
        }

        public virtual Scenario<TContext> And<TResult>(string name, Func<TContext, (string key, TResult value)> function)
        {
            if (currentPhase == null)
                throw new InvalidOperationException(Resources.AndWithoutPhase);

            currentPhase.Add(new StepAction<TContext, TResult>(
                name ?? throw new ArgumentNullException(nameof(name)),
                function ?? throw new ArgumentNullException(nameof(function))));
            return this;
        }

        public virtual Scenario<TContext> And<TResult>(string name, Func<TContext, (object key, TResult value)> function)
        {
            if (currentPhase == null)
                throw new InvalidOperationException(Resources.AndWithoutPhase);

            currentPhase.Add(new StepAction<TContext, TResult>(
                name ?? throw new ArgumentNullException(nameof(name)),
                function ?? throw new ArgumentNullException(nameof(function))));
            return this;
        }

        #endregion

        ScenarioSdk sdk;

        /// <summary>
        /// Access advanced members that are typically used by Gherkinator extensions.
        /// </summary>
        public ScenarioSdk Sdk => sdk ?? (sdk = new ScenarioSdk(this));

        /// <summary>
        /// Runs the configured steps and returns the resulting state.
        /// </summary>
        /// <returns></returns>
        public virtual TContext Run()
        {
            var (finalGiven, finalWhen, finalThen) = BuildSteps();

            var context = CreateContext();

            context.Set("Scenario.Name", scenarioName);
            context.Set("Feature.File", featureFile);

            CallContext.SetData(context);

            foreach (var callback in beforeGiven)
                callback.Invoke(context);

            foreach (var action in finalGiven)
                action.Action.Invoke(context);

            foreach (var callback in afterGiven)
                callback.Invoke(context);

            foreach (var callback in beforeWhen)
                callback.Invoke(context);

            foreach (var action in finalWhen)
                action.Action.Invoke(context);

            foreach (var callback in afterWhen)
                callback.Invoke(context);

            foreach (var callback in beforeThen)
                callback.Invoke(context);

            foreach (var action in finalThen)
                action.Action.Invoke(context);

            foreach (var callback in afterThen)
                callback.Invoke(context);

            return context;
        }

        /// <summary>
        /// Creates the initial state for the run.
        /// </summary>
        /// <returns></returns>
        protected abstract TContext CreateContext();

        private (List<StepAction<TContext>> finalGiven, List<StepAction<TContext>> finalWhen, List<StepAction<TContext>> finalThen) BuildSteps()
        {
            var finalGiven = new List<StepAction<TContext>>();
            var finalWhen = new List<StepAction<TContext>>();
            var finalThen = new List<StepAction<TContext>>();
            var steps = scenario.Steps;
            var background = feature.Children.OfType<Ast.Background>().FirstOrDefault();
            if (background != null)
                steps = background.Steps.Concat(steps);

            List<StepAction<TContext>> phase = null;
            List<StepAction<TContext>> implementation = null;
            string keyword = null;

            foreach (var step in steps)
            {
                var name = step.Text.Trim();
                switch (step.Keyword.Trim().ToLowerInvariant())
                {
                    case "given":
                        keyword = "Given";
                        phase = finalGiven;
                        implementation = given;
                        break;
                    case "when":
                        keyword = "When";
                        phase = finalWhen;
                        implementation = when;
                        break;
                    case "then":
                        keyword = "Then";
                        phase = finalThen;
                        implementation = then;
                        break;
                    case "and":
                        break;
                    default:
                        throw new NotSupportedException(string.Format(Resources.UnsupportedKeyword, step.Keyword.Trim()));
                }

                var action = implementation.FirstOrDefault(x
                    => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ??
                        // NOTE: we change the keyword to match Given/When/Then so the fallbacks are 
                        // easier to implement for each phase if needed.
                        fallbacks.Select(x => x.Invoke(new Ast.Step(step.Location, keyword, step.Text, step.Argument))).FirstOrDefault(x => x != null);

                if (action == null)
                {
                    throw new ArgumentException(
                        string.Format(Resources.MissingActionInFile, step.Text, scenario.Name, new FileInfo(featureFile).FullName, step.Location.Line, step.Location.Column),
                        step.Keyword.Trim().ToLowerInvariant());
                }

                phase.Add(action);
            }

            return (finalGiven, finalWhen, finalThen);
        }

        /// <summary>
        /// Advanced members typically used by extensions.
        /// </summary>
        public class ScenarioSdk
        {
            Scenario<TContext> scenario;

            public ScenarioSdk(Scenario<TContext> scenario) => this.scenario = scenario;

            /// <summary>
            /// Configures a callback to be run before the Given steps.
            /// </summary>
            public virtual Scenario<TContext> BeforeGiven(Action<TContext> callback)
            {
                scenario.beforeGiven.Add(callback ?? throw new ArgumentNullException(nameof(callback)));
                return scenario;
            }

            /// <summary>
            /// Configures a callback to be run after the Given steps.
            /// </summary>
            public virtual Scenario<TContext> AfterGiven(Action<TContext> callback)
            {
                scenario.afterGiven.Add(callback ?? throw new ArgumentNullException(nameof(callback)));
                return scenario;
            }

            /// <summary>
            /// Configures a callback to be run before the When steps.
            /// </summary>
            public virtual Scenario<TContext> BeforeWhen(Action<TContext> callback)
            {
                scenario.beforeWhen.Add(callback ?? throw new ArgumentNullException(nameof(callback)));
                return scenario;
            }

            /// <summary>
            /// Configures a callback to be run after the When steps.
            /// </summary>
            public virtual Scenario<TContext> AfterWhen(Action<TContext> callback)
            {
                scenario.afterWhen.Add(callback ?? throw new ArgumentNullException(nameof(callback)));
                return scenario;
            }

            /// <summary>
            /// Configures a callback to be run before the Then steps.
            /// </summary>
            public virtual Scenario<TContext> BeforeThen(Action<TContext> callback)
            {
                scenario.beforeThen.Add(callback ?? throw new ArgumentNullException(nameof(callback)));
                return scenario;
            }

            /// <summary>
            /// Configures a callback to be run after the Then steps.
            /// </summary>
            public virtual Scenario<TContext> AfterThen(Action<TContext> callback)
            {
                scenario.afterThen.Add(callback ?? throw new ArgumentNullException(nameof(callback)));
                return scenario;
            }

            /// <summary>
            /// Configures a fallback to be used when no other steps are matched for the given scenario.
            /// </summary>
            public virtual Scenario<TContext> Fallback(Func<Ast.Step, StepAction<TContext>> fallback)
            {
                scenario.fallbacks.Add(fallback ?? throw new ArgumentNullException(nameof(fallback)));
                return scenario;
            }
        }
    }
}