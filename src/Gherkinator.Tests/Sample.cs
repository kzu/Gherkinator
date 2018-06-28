using System;
using System.IO;
using System.Runtime.CompilerServices;
using Gherkin.Ast;
using Xunit;
using Xunit.Abstractions;

namespace Gherkinator.Tests
{
    public class Sample
    {
        private ITestOutputHelper output;

        public Sample(ITestOutputHelper output) => this.output = output;

        [Fact]
        public void when_missing_feature_file_then_throws()
            => Assert.Throws<FileNotFoundException>(() => Scenario("Foo.Feature")
                .Run());

        [Fact]
        public void when_missing_scenario_then_throws()
            => Assert.Throws<ArgumentException>("scenarioName", () => Scenario("Sample.Feature", "Non existing scenario")
                .Run());

        [Fact]
        public void when_and_without_given_then_throws()
            => Assert.Throws<InvalidOperationException>(() => Scenario("Sample.Feature", null, nameof(foo_should_equal_bar))
                .And("doing something", _ => { })
                .Run());

        [Fact]
        public void when_missing_given_then_throws()
            => Assert.Throws<ArgumentException>("given", () => Scenario("Sample.Feature", null, nameof(foo_should_equal_bar))
                .Run());

        [Fact]
        public void when_missing_when_then_throws()
            => Assert.Throws<ArgumentException>("when", () => Scenario("Sample.Feature", null, nameof(foo_should_equal_bar))
                .Given("foo", _ => { })
                .Given("bar", _ => { })
                .Run());

        [Fact]
        public void when_missing_and_then_throws()
            => Assert.Throws<ArgumentException>("and", () => Scenario("Sample.Feature", null, nameof(foo_should_equal_bar))
                .Given("foo", _ => { })
                .Given("bar", _ => { })
                .When("running test", _ => { })
                .Run());

        [Fact]
        public void when_missing_then_then_throws()
            => Assert.Throws<ArgumentException>("then", () => Scenario("Sample.Feature", null, nameof(foo_should_equal_bar))
                .Given("foo", _ => { })
                .Given("bar", _ => { })
                .When("running test", _ => { })
                .And("doing something", _ => { })
                .Run());

        [Fact]
        public void foo_should_equal_bar()
            => Scenario("Sample.Feature")
                .Given("foo", c => { output.WriteLine(c.Step.Keyword + c.Step.Text); c.State.Set("foo", ((DocString)c.Step.Argument).Content); })
                .Given("bar", c => { output.WriteLine(c.Step.Keyword + c.Step.Text); c.State.Set("bar", ((DocString)c.Step.Argument).Content); })
                .When("running test", c => output.WriteLine(c.Step.Keyword + c.Step.Text))
                .And("doing something", c => output.WriteLine(c.Step.Keyword + c.Step.Text))
                .Then("foo equals bar", c => { output.WriteLine(c.Step.Keyword + c.Step.Text); Assert.Equal(c.State.Get<string>("foo"), c.State.Get<string>("bar")); })
                .And("succeeds", c => output.WriteLine(c.Step.Keyword + c.Step.Text))
                .Run();

        ScenarioBuilder Scenario(string featureFile, string scenarioName = null, [CallerMemberName] string testMethod = null)
            => new ScenarioBuilder(featureFile, scenarioName ?? testMethod.Replace('_', ' '));
    }
}
