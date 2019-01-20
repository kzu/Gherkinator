using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Execution;
using Xunit;
using Xunit.Abstractions;

namespace Gherkinator.Tests
{
    public class BuildSample
    {
        readonly ITestOutputHelper output;

        public BuildSample(ITestOutputHelper output) => this.output = output;

        [Fact]
        public void sdk_project_should_restore()
            => new BuildScenario()
                .When("restoring packages", c => c.Build("Foo.csproj", "Restore"))
                .Then("restore succeeds", c => Assert.Equal(BuildResultCode.Success, c.LastBuildResult.OverallResult))
                .Run();

        [Fact]
        public void can_access_MSBuild_state()
            => new BuildScenario()
                .When("restoring packages", c => c.Build("Foo.csproj", "Restore"))
                .When("building project", c => c.Build("Foo.csproj", "Build"))
                .Then("build result is successful", c 
                    => Assert.Equal(BuildResultCode.Success, c.LastBuildResult.OverallResult))
                .Run();

        [Fact]
        public void should_invoke_target_by_name()
            => new BuildScenario()
                .When("invoking restore programmatically", c => c.Set(c.Build("Foo.csproj", "Restore")))
                .Then("build result is successful", c
                    => Assert.Equal(BuildResultCode.Success, c.Get<BuildResult>().OverallResult))
                .Run();

        [Fact]
        public void should_invoke_target_by_name_with_properties()
            => new BuildScenario()
                .When("invoking restore with properties", c 
                    => c.Set(c.Build("Foo.csproj", "Restore", new Dictionary<string, string> { { "xunit", "true" } })))
                .Then("restore assets contain conditional package reference", c
                    => Assert.Contains("xunit", File.ReadAllText(Path.Combine(c.GetTempDir(), "obj\\project.assets.json"))))
                .Run();

        [Conditional("DEBUG")]
        [Fact]
        public void after_restore_the_build_log_can_be_opened()
            => new BuildScenario()
                .When("restoring packages", c => c.Build("Foo.csproj", "Restore"))
                .Then("can open build log", c
                    => c.OpenLog("Foo.csproj", "Restore")/*.Kill()*/)
                .Run();
    }
}
