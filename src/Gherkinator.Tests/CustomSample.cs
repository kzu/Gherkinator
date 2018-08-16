using System;
using System.Runtime.CompilerServices;
using Gherkin.Ast;
using Xunit;

namespace Gherkinator.Tests
{
    public class CustomSample
    {
        [Fact]
        public void foo_should_equal_bar()
            => Scenario("CustomSample.Feature")
                .UseCustom()
                .Then("foo equals bar", c => Assert.Equal(c.State.Get<string>("Foo"), c.State.Get<string>("Bar")))
                .Run();

        ScenarioBuilder Scenario(string featureFile, string scenarioName = null, [CallerMemberName] string testMethod = null)
            => new ScenarioBuilder(featureFile, scenarioName ?? testMethod.Replace('_', ' '));
    }
}

namespace Gherkinator
{
    using Gherkinator.Sdk;

    public static class Custom
    {
        public static ScenarioBuilder UseCustom(this ScenarioBuilder builder)
        {
            builder.Fallback(step =>
            {
                if (step.Keyword.Trim().Equals("given", StringComparison.OrdinalIgnoreCase) && 
                    step.Text.Contains("="))
                {
                    var parts = step.Text.Split('=');
                    var key = parts[0].Trim();
                    var value = (step.Argument as DocString)?.Content ?? parts[1].Trim();

                    value = value.Trim('\"');

                    return new StepAction(step.Text, c => c.State.Set(key, value));
                }

                return null;
            });

            return builder;
        }
    }
}
