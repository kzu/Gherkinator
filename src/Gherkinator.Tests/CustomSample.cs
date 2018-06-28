using System;
using System.Linq;
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
                .Then("foo equals bar", c => Assert.Equal(c.State.Get<string>("Foo"), c.State.Get<string>("Bar")))
                .Run();

        CustomBuilder Scenario(string featureFile, string scenarioName = null, [CallerMemberName] string testMethod = null)
            => new CustomBuilder(featureFile, scenarioName ?? testMethod.Replace('_', ' '));

        class CustomBuilder : ScenarioBuilder
        {
            public CustomBuilder(string featureFile, string scenarioName) : base(featureFile, scenarioName)
            {
            }

            public override ScenarioActions Build()
            {
                var steps = Scenario.Steps;
                var background = Feature.Children.OfType<Background>().FirstOrDefault();
                if (background != null)
                    steps = background.Steps.Concat(steps);

                foreach (var step in steps.Where(s => s.Keyword.Trim().Equals("given", StringComparison.OrdinalIgnoreCase)))
                {
                    if (GivenActions.FirstOrDefault(x => x.Name.Equals(step.Keyword.Trim(), StringComparison.OrdinalIgnoreCase)) == null && 
                        step.Text.Contains("="))
                    {
                        var parts = step.Text.Split('=');
                        var key = parts[0].Trim();
                        var value = (step.Argument as DocString)?.Content ?? parts[1].Trim();

                        value = value.TrimStart('\"').TrimEnd('\"');

                        GivenActions.Add(new StepAction(step.Text, c => c.State.Set(key, value)));
                    }
                }

                return base.Build();
            }
        }
    }
}
