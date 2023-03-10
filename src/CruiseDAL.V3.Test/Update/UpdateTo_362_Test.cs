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
    public class UpdateTo_362_Test : TestBase
    {
        public UpdateTo_362_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void UpdateFrom361()
        {
            var filePath = InitializeTestFile("361_AllTables.crz3");

            using var db = new CruiseDatastore(filePath);

            var updateTo362 = new UpdateTo_3_6_2();

            using var conn = db.OpenConnection();
            updateTo362.Update(conn);

            var tableDef = db.GetTableInfo(nameof(SampleGroup));
            var bigBAFCol = tableDef.Single(x => x.Name == nameof(SampleGroup.BigBAF));
            bigBAFCol.Type.Should().Be("REAL");

        }
    }
}
