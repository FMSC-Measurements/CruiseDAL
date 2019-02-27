using System.IO;
using Xunit.Abstractions;

namespace CruiseDAL.Tests
{
    public abstract class TestBase
    {
        protected readonly ITestOutputHelper Output;
        private string _testTempPath;

        public TestBase(ITestOutputHelper output)
        {
            Output = output;

            var testTempPath = TestTempPath;
            if(!Directory.Exists(testTempPath))
            {
                Directory.CreateDirectory(testTempPath);
            }
        }

        public string TestTempPath
        {
            get
            {
                return _testTempPath ?? (_testTempPath = Path.Combine(Path.GetTempPath(), "TestTemp", this.GetType().FullName));
            }
        }
    }
}