using System.IO;
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
    }
}
