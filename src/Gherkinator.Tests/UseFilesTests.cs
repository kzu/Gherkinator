using System.IO;
using Xunit;
using Xunit.Sdk;

namespace Gherkinator.Tests
{
    public class UseFilesTests
    {
        [Fact]
        public void setting_file_contents_inline()
            => new Scenario().UseFiles<Scenario, ScenarioContext>().Run();

        [Fact]
        public void setting_file_contents_block()
            => new Scenario().UseFiles<Scenario, ScenarioContext>().Run();

        [Fact]
        public void verifying_file_contents()
            => Assert.Throws<EqualException>(() => new Scenario().UseFiles<Scenario, ScenarioContext>().Run());

        [Fact]
        public void after_run_deletes_temporary_directory()
        {
            var state = new Scenario().UseFiles<Scenario, ScenarioContext>()
                .Sdk.BeforeThen(s => Assert.True(Directory.Exists(s.GetTempDir())))
                .Run();

            Assert.False(Directory.Exists(state.GetTempDir()));
        }
    }
}
