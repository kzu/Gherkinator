using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Gherkin;
using Gherkin.Ast;
using Gherkinator.Properties;

namespace Gherkinator
{
    public class ScenarioBuilder
    {
        readonly string featureFile;
        readonly string scenarioName;
        readonly string testMethod;
        readonly string testFile;
        readonly int? testLine;

        readonly List<Func<Step, StepAction>> fallbacks = new List<Func<Step, StepAction>>();
        readonly List<StepAction> given = new List<StepAction>();
        readonly List<StepAction> when = new List<StepAction>();
        readonly List<StepAction> then = new List<StepAction>();
        List<StepAction> currentPhase;

        readonly List<Action<ScenarioState>> beforeGiven = new List<Action<ScenarioState>>();
        readonly List<Action<ScenarioState>> afterGiven = new List<Action<ScenarioState>>();
        readonly List<Action<ScenarioState>> beforeWhen = new List<Action<ScenarioState>>();
        readonly List<Action<ScenarioState>> afterWhen = new List<Action<ScenarioState>>();
        readonly List<Action<ScenarioState>> beforeThen = new List<Action<ScenarioState>>();
        readonly List<Action<ScenarioState>> afterThen = new List<Action<ScenarioState>>();

        public ScenarioBuilder(string featureFile, string scenarioName, [CallerMemberName] string testMethod = null, [CallerFilePath] string testFile = null, [CallerLineNumber] int? testLine = null)
        {
            this.featureFile = featureFile ?? throw new ArgumentNullException(nameof(featureFile));
            this.scenarioName = scenarioName ?? throw new ArgumentNullException(nameof(ScenarioBuilder.scenarioName));

            Feature = new Parser().Parse(featureFile).Feature;
            Scenario = Scenario = Feature.Children
                .OfType<Scenario>()
                .FirstOrDefault(x => x.Name.Equals(scenarioName, StringComparison.OrdinalIgnoreCase)) ??
                throw new ArgumentException(string.Format(Resources.ScenarioNotFoundInFile, scenarioName, featureFile), nameof(scenarioName));

            this.testMethod = testMethod;
            this.testFile = testFile;
            this.testLine = testLine;
        }

        public Feature Feature { get; }

        public Scenario Scenario { get; }

        public ScenarioBuilder Given(string name, Action<StepContext> action)
        {
            given.Add(new StepAction(
                name ?? throw new ArgumentNullException(nameof(name)),
                action ?? throw new ArgumentNullException(nameof(action))));
            currentPhase = given;
            return this;
        }

        public ScenarioBuilder When(string name, Action<StepContext> action)
        {
            when.Add(new StepAction(
                name ?? throw new ArgumentNullException(nameof(name)),
                action ?? throw new ArgumentNullException(nameof(action))));
            currentPhase = when;
            return this;
        }

        public ScenarioBuilder Then(string name, Action<StepContext> action)
        {
            then.Add(new StepAction(
                name ?? throw new ArgumentNullException(nameof(name)),
                action ?? throw new ArgumentNullException(nameof(action))));
            currentPhase = then;
            return this;
        }

        public ScenarioBuilder And(string name, Action<StepContext> action)
        {
            if (currentPhase == null)
                throw new InvalidOperationException(Resources.AndWithoutPhase);

            currentPhase.Add(new StepAction(
                name ?? throw new ArgumentNullException(nameof(name)),
                action ?? throw new ArgumentNullException(nameof(action))));
            return this;
        }

        public ScenarioActions Build()
        {
            var finalGiven = new List<StepAction>();
            var finalWhen = new List<StepAction>();
            var finalThen = new List<StepAction>();

            var steps = Scenario.Steps;
            var background = Feature.Children.OfType<Background>().FirstOrDefault();
            if (background != null)
                steps = background.Steps.Concat(steps);

            List<StepAction> phase = null;
            List<StepAction> implementation = null;
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
                        fallbacks.Select(x => x.Invoke(new Step(step.Location, keyword, step.Text, step.Argument))).FirstOrDefault(x => x != null);

                if (action == null)
                {
                    if (featureFile != null)
                    {
                        if (testFile != null)
                            throw new ArgumentException(
                                string.Format(Resources.MissingActionInFileAndTest, 
                                    step.Text, 
                                    Scenario.Name, 
                                    new FileInfo(featureFile).FullName, 
                                    step.Location.Line, 
                                    step.Location.Column, 
                                    testFile, 
                                    testLine),
                                step.Keyword.Trim().ToLowerInvariant());
                        else
                            throw new ArgumentException(
                                string.Format(Resources.MissingActionInFile, step.Text, Scenario.Name, new FileInfo(featureFile).FullName, step.Location.Line, step.Location.Column),
                                step.Keyword.Trim().ToLowerInvariant());
                    }

                    throw new ArgumentException(string.Format(Resources.MissingAction, step.Text, Scenario.Name, Feature.Name), step.Keyword.Trim().ToLowerInvariant());
                }

                if (action.Step == null)
                    action.Step = step;

                phase.Add(action);
            }

            return new ScenarioActions(finalGiven, finalWhen, finalThen, 
                beforeGiven, afterGiven, beforeWhen, afterWhen, beforeThen, afterThen);
        }

        public ScenarioState Run() => new ScenarioRunner(Scenario).Run(Build());

        // Publicly available via SdkExtensions to avoid polluting the fluent API for regular use.
        internal ScenarioBuilder BeforeGiven(Action<ScenarioState> callback)
        {
            beforeGiven.Add(callback ?? throw new ArgumentNullException(nameof(callback)));
            return this;
        }

        internal ScenarioBuilder AfterGiven(Action<ScenarioState> callback)
        {
            afterGiven.Add(callback ?? throw new ArgumentNullException(nameof(callback)));
            return this;
        }

        internal ScenarioBuilder BeforeWhen(Action<ScenarioState> callback)
        {
            beforeWhen.Add(callback ?? throw new ArgumentNullException(nameof(callback)));
            return this;
        }

        internal ScenarioBuilder AfterWhen(Action<ScenarioState> callback)
        {
            afterWhen.Add(callback ?? throw new ArgumentNullException(nameof(callback)));
            return this;
        }

        internal ScenarioBuilder BeforeThen(Action<ScenarioState> callback)
        {
            beforeThen.Add(callback ?? throw new ArgumentNullException(nameof(callback)));
            return this;
        }

        internal ScenarioBuilder AfterThen(Action<ScenarioState> callback)
        {
            afterThen.Add(callback ?? throw new ArgumentNullException(nameof(callback)));
            return this;
        }

        internal ScenarioBuilder Fallback(Func<Step, StepAction> fallback)
        {
            fallbacks.Add(fallback ?? throw new ArgumentNullException(nameof(fallback)));
            return this;
        }
    }
}
