using CruiseDAL.TestCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync.Test.UnitTests
{
    public class TableSyncOptions_Test : TestBase
    {
        public TableSyncOptions_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ToString_Test()
        {
            var options = new TableSyncOptions();
            Output.WriteLine(options.ToString());
        }
    }
}
