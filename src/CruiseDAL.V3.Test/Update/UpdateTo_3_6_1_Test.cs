using CruiseDAL.TestCommon;
using CruiseDAL.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test.Update
{
    public class UpdateTo_3_6_1_Test : TestBase
    {
        public UpdateTo_3_6_1_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void UpdateFrom360()
        {
            var filePath = InitializeTestFile("360_AllTables.crz3");

            using var db = new CruiseDatastore(filePath);

            var updateTo360 = new UpdateTo_3_6_1();

            using var conn = db.OpenConnection();
            updateTo360.Update(conn);
        }
    }
}
