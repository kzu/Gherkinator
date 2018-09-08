using System.Collections.Generic;
using System.Diagnostics;
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
                .When("restoring packages", c => c.Build("Foo.csproj", "Restore"))
                .Then("restore succeeds", c => Assert.Equal(BuildResultCode.Success, c.State.MSBuild().LastBuildResult.OverallResult))
                .Run();

        [Fact]
        public void can_access_MSBuild_state()
            => Scenario()
                .UseMSBuild()
                .When("restoring packages", c => c.Build("Foo.csproj", "Restore"))
                .Then("build result is successful", c 
                    => Assert.Equal(BuildResultCode.Success, c.State.MSBuild().LastBuildResult.OverallResult))
                .Run();

        [Fact]
        public void should_invoke_target_by_name()
            => Scenario()
                .UseMSBuild()
                .When("invoking restore programmatically", c => c.State.Set(c.Build("Foo.csproj", "Restore")))
                .Then("build result is successful", c
                    => Assert.Equal(BuildResultCode.Success, c.State.Get<BuildResult>().OverallResult))
                .Run();

        [Fact]
        public void should_invoke_target_by_name_with_properties()
            => Scenario()
                .UseMSBuild()
                .When("invoking restore with properties", c 
                    => c.State.Set(c.Build("Foo.csproj", "Restore", new Dictionary<string, string> { { "xunit", "true" } })))
                .Then("restore assets contain conditional package reference", c
                    => Assert.Contains("xunit", File.ReadAllText(Path.Combine(c.State.GetTempDir(), "obj\\project.assets.json"))))
                .Run();

        [Fact]
        public void after_restore_the_build_log_can_be_opened()
            => Scenario()
                .UseMSBuild()
                .When("restoring packages", c => c.Build("Foo.csproj", "Restore"))
                .Then("can open build log", c
                    => c.State.MSBuild().OpenLog("Foo.csproj", "Restore")/*.Kill()*/)
                .Run();
    }
}
