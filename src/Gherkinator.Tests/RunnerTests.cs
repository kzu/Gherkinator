using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gherkinator.Sdk;
using static Gherkinator.Syntax;
using static Gherkinator.Tests.Features.RunnerFeature;
using static Gherkinator.Tests.Features.RunnerFeature.Scenarios.RunnerScenario;
using Xunit;

namespace Gherkinator.Tests
{
    public class RunnerTests
    {
        [Fact]
        public void when_running_then_succeeds()
            => Scenario(nameof(Scenarios.RunnerScenario))
                .Given(nameof(Background.Given.GivenBackground), _ => { })
                .Given(nameof(Given.AGiven), _ => { })
                .And(nameof(Given.AndGiven), _ => { })
                .When(nameof(When.AWhen), _ => { })
                .And(nameof(When.AndWhen), _ => { })
                .Then(nameof(Then.AThen), _ => { })
                .And(nameof(Then.AndThen), _ => { })
                .Run();

        [Fact]
        public void when_adding_fallback_then_succeeds()
        {
            var state = Scenario(nameof(Scenarios.RunnerScenario))
                .Fallback(step => new StepAction(step, context => context.State.Set(step.Text, step.Text)))
                .Run();

            Assert.All(state.Scenario.Steps, step => Assert.Equal(step.Text, state.Get<string>(step.Text)));
        }

        [Fact]
        public void when_adding_before_after_then_succeeds()
        {
            var result = Scenario(nameof(Scenarios.RunnerScenario))
                .Fallback(step => new StepAction(step, context => context.State.Set(step.Text, step.Text)))
                .BeforeGiven(state => state.Set(SdkExtensions.BeforeGiven))
                .AfterGiven(state => state.Set(SdkExtensions.AfterGiven))
                .BeforeWhen(state => state.Set(SdkExtensions.BeforeWhen))
                .AfterWhen(state => state.Set(SdkExtensions.AfterWhen))
                .BeforeThen(state => state.Set(SdkExtensions.BeforeThen))
                .AfterThen(state => state.Set(SdkExtensions.AfterThen))
                .Run();

            Assert.NotNull(result.Get(SdkExtensions.BeforeGiven));
            Assert.NotNull(result.Get(SdkExtensions.AfterGiven));
            Assert.NotNull(result.Get(SdkExtensions.BeforeWhen));
            Assert.NotNull(result.Get(SdkExtensions.AfterWhen));
            Assert.NotNull(result.Get(SdkExtensions.BeforeThen));
            Assert.NotNull(result.Get(SdkExtensions.AfterThen));
        }

        void Set(ScenarioState state, Func<ScenarioBuilder, Action<ScenarioState>, ScenarioBuilder> beforeGiven)
        {
            throw new NotImplementedException();
        }

        static void Set(ScenarioState state, string name)
            => state.Set(name, name);
    }

    public static class Extensions
    {
        public static void Contains(this ScenarioState state, Func<ScenarioBuilder, Action<ScenarioState>, ScenarioBuilder> method)
            => Assert.NotNull(state.Get<string>(method.Method.Name));

        public static string Get(this ScenarioState state, Func<ScenarioBuilder, Action<ScenarioState>, ScenarioBuilder> method)
            => state.Get<string>(method.Method.Name);

        public static void Set(this ScenarioState state, Func<ScenarioBuilder, Action<ScenarioState>, ScenarioBuilder> method)
            => state.Set(method.Method.Name, method.Method.Name);
    }
}