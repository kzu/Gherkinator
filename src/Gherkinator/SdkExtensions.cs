using System;
using System.ComponentModel;
using Gherkin.Ast;

namespace Gherkinator.Sdk
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SdkExtensions
    {
        public static void BeforeGiven(this ScenarioBuilder builder, Action<ScenarioState> callback) => builder.BeforeGiven(callback);
        public static void AfterGiven (this ScenarioBuilder builder, Action<ScenarioState> callback) => builder.AfterGiven(callback);
        public static void BeforeWhen (this ScenarioBuilder builder, Action<ScenarioState> callback) => builder.BeforeWhen(callback);
        public static void AfterWhen  (this ScenarioBuilder builder, Action<ScenarioState> callback) => builder.AfterWhen(callback);
        public static void BeforeThen (this ScenarioBuilder builder, Action<ScenarioState> callback) => builder.BeforeThen(callback);
        public static void AfterThen  (this ScenarioBuilder builder, Action<ScenarioState> callback) => builder.AfterThen(callback);
        public static void Fallback(this ScenarioBuilder builder, Func<Step, StepAction> fallback) => builder.Fallback(fallback);
    }
}
