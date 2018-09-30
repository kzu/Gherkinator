using System;
using System.IO;
using Gherkin.Ast;
using Xunit;
using Xunit.Abstractions;

namespace Gherkinator.NewApi.Tests
{
    public class ScenarioTests
    {
        private ITestOutputHelper output;

        public ScenarioTests(ITestOutputHelper output) => this.output = output;

        [Fact]
        public void file_and_scenario_inference()
            => new Scenario().Run();

        [Fact]
        public void throws_if_scenario_not_found()
            => Assert.Throws<ArgumentException>(() => new Scenario());

        [Fact]
        public void throws_null_scenarioName()
            => Assert.Throws<ArgumentNullException>(() => new Scenario(null, "foo.feature"));

        [Fact]
        public void throws_null_featureFile()
            => Assert.Throws<ArgumentNullException>(() => new Scenario("scenario", null));

        [Fact]
        public void adding_before_after()
        {
            var result = new Scenario()
                .Sdk.Fallback(step => new StepAction<ScenarioContext>(step.Text, state => state.Set(step.Text, step.Text)))
                .Sdk.BeforeGiven(state => Set(state, "BeforeGiven"))
                .Sdk.AfterGiven(state => Set(state, "AfterGiven"))
                .Sdk.BeforeWhen(state => Set(state, "BeforeWhen"))
                .Sdk.AfterWhen(state => Set(state, "AfterWhen"))
                .Sdk.BeforeThen(state => Set(state, "BeforeThen"))
                .Sdk.AfterThen(state => Set(state, "AfterThen"))
                .Run();

            Assert.NotNull(result.Get<string>("BeforeGiven"));
            Assert.NotNull(result.Get<string>("AfterGiven"));
            Assert.NotNull(result.Get<string>("BeforeWhen"));
            Assert.NotNull(result.Get<string>("AfterWhen"));
            Assert.NotNull(result.Get<string>("BeforeThen"));
            Assert.NotNull(result.Get<string>("AfterThen"));
        }

        [Fact]
        public void use_custom_extension()
            => new Scenario()
                .UseCustom()
                .Then("foo equals bar", state => Assert.Equal("bar", state.Get<string>("foo")))
                .Run();

        [Fact]
        public void missing_feature_file_throws()
            => Assert.Throws<FileNotFoundException>(() => new Scenario(featureFile: "Foo")
                .Run());

        [Fact]
        public void missing_scenario_throws()
            => Assert.Throws<ArgumentException>("scenarioName", () => new Scenario(scenarioName: "Non existing scenario")
                .Run());

        [Fact]
        public void and_without_given_throws()
            => Assert.Throws<InvalidOperationException>(() => new Scenario(testMethod: nameof(foo_should_equal_bar))
                .And("doing something", _ => { })
                .Run());

        [Fact]
        public void missing_given_throws()
            => Assert.Throws<ArgumentException>("given", () => new Scenario(testMethod: nameof(foo_should_equal_bar))
                .Run());

        [Fact]
        public void missing_when_throws()
            => Assert.Throws<ArgumentException>("when", () => new Scenario(testMethod: nameof(foo_should_equal_bar))
                .Given("foo", _ => { })
                .Given("bar", _ => { })
                .Run());

        [Fact]
        public void missing_and_throws()
            => Assert.Throws<ArgumentException>("and", () => new Scenario(testMethod: nameof(foo_should_equal_bar))
                .Given("foo", _ => { })
                .Given("bar", _ => { })
                .When("running test", _ => { })
                .Run());

        [Fact]
        public void missing_then_throws()
            => Assert.Throws<ArgumentException>("then", () => new Scenario(testMethod: nameof(foo_should_equal_bar))
                .Given("foo", _ => { })
                .Given("bar", _ => { })
                .When("running test", _ => { })
                .And("doing something", _ => { })
                .Run());

        [Fact]
        public void foo_should_equal_bar()
            => new Scenario()
                .Given("bar", c => c.Set("bar", "bar"))
                .When("running test", c => c.Set("foo", "bar"))
                .And("doing something", c => { })
                .Then("foo equals bar", c => Assert.Equal(c.Get<string>("foo"), c.Get<string>("bar")))
                .And("succeeds", c => Assert.True(true))
                .Run();

        [Fact]
        public void steps_can_set_state()
            => new Scenario()
                .Given("foo", _ => { })
                .And("a saved value 10", c => ("Given", 10))
                .When("a value 20 is also saved", c => ("When", 20))
                .Then("can add two values from state", c => ("Then", c.Get<int>("Given") + c.Get<int>("When")))
                .And("verify the result", c => Assert.Equal(30, c.Get<int>("Then")))
                .Run();

        static void Set(ScenarioContext state, string name)
            => state.Set(name, name);

    }

    public static class Custom
    {
        public static TScenario UseCustom<TScenario>(this TScenario scenario) where TScenario : Scenario<ScenarioContext>
        {
            scenario.Sdk.Fallback(step =>
            {
                if (step.Keyword.Trim().Equals("given", StringComparison.OrdinalIgnoreCase) &&
                    step.Text.Contains("="))
                {
                    var parts = step.Text.Split('=');
                    var key = parts[0].Trim();
                    var value = (step.Argument as DocString)?.Content ?? parts[1].Trim();

                    value = value.Trim('\"');

                    return new StepAction<ScenarioContext>(step.Text, state => state.Set(key, value));
                }

                return null;
            });

            return scenario;
        }
    }
}
