using CruiseDAL.TestCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync.Test.UnitTests
{
    public class CruiseCopier2_Test : TestBase
    {
        public CruiseCopier2_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Copy()
        {
            var initializer = new DatabaseInitializer();
            using (var srcDb = initializer.CreateDatabaseWithAllTables())
            using (var destDb = new CruiseDatastore_V3())
            {
                var copier = new CruiseCopier2();
                copier.Copy(srcDb, destDb, initializer.CruiseID);
            }
        }
    }
}
