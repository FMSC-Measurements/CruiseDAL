using CruiseDAL.TestCommon;
using CruiseDAL.Update;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test.Update
{
    public class UpdateTo_3_6_0_Test : TestBase
    {
        public UpdateTo_3_6_0_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void UpdateFrom356()
        {
            var filePath = InitializeTestFile("3.5.6_AllTables.crz3");

            using var ds = new CruiseDatastore(filePath);

            var updateTo360 = new UpdateTo_3_6_0();

            using var conn = ds.OpenConnection();
            updateTo360.Update(conn);

            ds.CheckTableExists(nameof(CruiseLog)).Should().BeTrue();
        }
    }
}
