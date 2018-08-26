using System.IO;
using System.Linq;
using Microsoft.Build.Execution;
using Xunit;
using Xunit.Abstractions;
using static Gherkinator.Syntax;

namespace Gherkinator.Tests
{
    public class MSBuildSample
    {
        readonly ITestOutputHelper output;

        public MSBuildSample(ITestOutputHelper output) => this.output = output;

        [Fact]
        public void sdk_project_should_restore()
            => Scenario()
                .UseMSBuild()
                .Run();

        [Fact]
        public void can_access_MSBuild_state()
            => Scenario()
                .UseMSBuild()
                .Then("build result is successful", c 
                    => Assert.Equal(BuildResultCode.Success, c.State.MSBuild().LastBuildResult.OverallResult))
                .And("can access built project instance by path", c 
                    => Assert.NotNull(c.State.MSBuild().GetProject("Foo.csproj")))
                .And("can enumerate all built projects", c
                    => Assert.Collection(c.State.MSBuild().Projects, p => p.FullPath.EndsWith("Foo.csproj")))
                .Run();

        [Fact]
        public void should_invoke_target_by_name()
        => Scenario()
            .UseMSBuild()
            .When("invoking restore programmatically", c => c.State.Set(c.Build("Foo.csproj", "Restore")))
            .Then("build result is successful", c
                => Assert.Equal(BuildResultCode.Success, c.State.Get<BuildResult>().OverallResult))
            .Run();
    }
}
