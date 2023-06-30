using CruiseDAL.TestCommon;
using CruiseDAL.Update;
using CruiseDAL.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test.Update
{
    public class UpdateTo_364_Test : TestBase
    {
        public UpdateTo_364_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void UpdateFrom362()
        {
            var filePath = InitializeTestFile("363_AllTables.crz3");

            using var db = new CruiseDatastore(filePath);
            var cruise = db.From<Cruise>().Query().Single();

            var updateTo363 = new UpdateTo_3_6_4();

            using var conn = db.OpenConnection();
            updateTo363.Update(conn);

        }
    }
}
