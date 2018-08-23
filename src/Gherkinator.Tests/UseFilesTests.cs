using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using static Gherkinator.Syntax;

namespace Gherkinator.Tests
{
    public class UseFilesTests
    {
        [Fact]
        public void setting_file_contents_inline()
            => Scenario().UseFiles().Run().Dispose();

        [Fact]
        public void setting_file_contents_block()
            => Scenario().UseFiles().Run().Dispose();

        [Fact]
        public void verifying_file_contents()
            => Assert.Throws<EqualException>(() => Scenario().UseFiles().Run().Dispose());

        [Fact]
        public void disposing_deletes_temporary_directory()
        {
            var state = Scenario().UseFiles().Run();
            Assert.True(Directory.Exists(state.GetTempDir()));
            state.Dispose();
            Assert.False(Directory.Exists(state.GetTempDir()));
        }
    }
}
