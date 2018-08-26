using System;
using System.ComponentModel;
using Gherkin.Ast;

namespace Gherkinator.Sdk
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SdkExtensions
    {
        public static ScenarioBuilder BeforeGiven(this ScenarioBuilder builder, Action<ScenarioState> callback) => builder.BeforeGiven(callback);
        public static ScenarioBuilder AfterGiven (this ScenarioBuilder builder, Action<ScenarioState> callback) => builder.AfterGiven(callback);
        public static ScenarioBuilder BeforeWhen (this ScenarioBuilder builder, Action<ScenarioState> callback) => builder.BeforeWhen(callback);
        public static ScenarioBuilder AfterWhen  (this ScenarioBuilder builder, Action<ScenarioState> callback) => builder.AfterWhen(callback);
        public static ScenarioBuilder BeforeThen (this ScenarioBuilder builder, Action<ScenarioState> callback) => builder.BeforeThen(callback);
        public static ScenarioBuilder AfterThen  (this ScenarioBuilder builder, Action<ScenarioState> callback) => builder.AfterThen(callback);
        public static ScenarioBuilder Fallback(this ScenarioBuilder builder, Func<Step, StepAction> fallback) => builder.Fallback(fallback);
        public static ScenarioBuilder Fallback(this ScenarioBuilder builder, params Func<Step, StepAction>[] fallbacks)
        {
            foreach (var fallback in fallbacks ?? throw new ArgumentNullException(nameof(fallbacks)))
            {
                builder.Fallback(fallback);
            }
            return builder;
        }
    }
}
