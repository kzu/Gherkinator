using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using static Gherkinator.Syntax;
using Gherkinator.Sdk;

namespace Gherkinator.Tests
{
    public class UseFilesTests
    {
        [Fact]
        public void setting_file_contents_inline()
            => Scenario().UseFiles().Run();

        [Fact]
        public void setting_file_contents_block()
            => Scenario().UseFiles().Run();

        [Fact]
        public void verifying_file_contents()
            => Assert.Throws<EqualException>(() => Scenario().UseFiles().Run());

        [Fact]
        public void after_run_deletes_temporary_directory()
        {
            var state = Scenario().UseFiles()
                .BeforeThen(s => Assert.True(Directory.Exists(s.GetTempDir())))
                .Run();

            Assert.False(Directory.Exists(state.GetTempDir()));
        }
    }
}
