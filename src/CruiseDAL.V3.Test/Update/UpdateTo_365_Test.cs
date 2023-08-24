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
    public class UpdateTo_365_Test : TestBase
    {
        public UpdateTo_365_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void UpdateFrom364()
        {
            var filePath = InitializeTestFile("364_AllTables.crz3");

            using var db = new CruiseDatastore(filePath);

            var updateTo363 = new UpdateTo_3_6_5();

            using var conn = db.OpenConnection();
            updateTo363.Update(conn);

        }

        [Fact]
        public void NatCruiseIssue_120()
        {
            var filePath = InitializeTestFile("22301_Ojito IRTC_TS.crz3");

            using var db = new CruiseDatastore(filePath);

            var updateTo363 = new UpdateTo_3_6_5();

            using var conn = db.OpenConnection();
            updateTo363.Update(conn);

            var tallyLedgers = db.From<TallyLedger>().Query().ToArray();

        }
    }
}
