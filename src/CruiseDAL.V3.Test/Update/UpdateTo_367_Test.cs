using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CruiseDAL.TestCommon;
using CruiseDAL.Update;
using CruiseDAL.V3.Models;
using FluentAssertions;
using FMSC.ORM;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test.Update
{
    public class UpdateTo_367_Test : TestBase
    {
        public UpdateTo_367_Test(ITestOutputHelper output) : base(output)
        {}

        [Fact]
        public void UpdateFrom366()
        {
            var filePath = InitializeTestFile("364_AllTables.crz3");

            var secondCruiseInit = new DatabaseInitializer(false);


            using var db = new CruiseDatastore_V3(filePath);

            // add a second cruise to file
            secondCruiseInit.PopulateDatabase(db);

            db.Execute("UPDATE TreeAuditRule SET CruiseID = @p1", secondCruiseInit.CruiseID);

            //var tars = db.From<TreeAuditRule>().Query().ToArray();
            //var taRess = db.From<TreeAuditResolution>().Query().ToArray();

            var updateTo365 = new UpdateTo_3_6_5();

            using var conn = db.OpenConnection();
            updateTo365.Update(conn);

            var updateTo366 = new UpdateTo_3_6_6();
            updateTo366.Update(conn);

            db.QueryScalar<int>("SELECT TreeAuditRuleSelector_CN FROM TreeAuditRuleSelector LEFT JOIN TreeAuditRule AS tar USING (TreeAuditRuleID, CruiseID) WHERE tar.RowID IS NULL")
                .Count().Should().BeGreaterThan(0);

            db.QueryScalar<int>("SELECT TreeAuditResolution_CN FROM TreeAuditResolution LEFT JOIN TreeAuditRule AS tar USING (TreeAuditRuleID, CruiseID) WHERE tar.RowID IS NULL")
                .Count().Should().BeGreaterThan(0);


            var updateTo367 = new UpdateTo_3_6_7();
            updateTo367.Update(conn);

            db.QueryScalar<int>("SELECT TreeAuditRuleSelector_CN FROM TreeAuditRuleSelector LEFT JOIN TreeAuditRule AS tar USING (TreeAuditRuleID, CruiseID) WHERE tar.RowID IS NULL")
                .Count().Should().Be(0);

            db.QueryScalar<int>("SELECT TreeAuditResolution_CN FROM TreeAuditResolution LEFT JOIN TreeAuditRule AS tar USING (TreeAuditRuleID, CruiseID) WHERE tar.RowID IS NULL")
                .Count().Should().Be(0);
        }



        // make sure new foreign keys are working, changing cruiseID should break foreign keys
        [Fact]
        public void UpdateFrom366_EnsureNewForeignKeys()
        {
            var filePath = InitializeTestFile("364_AllTables.crz3");

            


            using var db = new CruiseDatastore_V3(filePath);

            var secondCruiseInit = new DatabaseInitializer(false);
            // add a second cruise to file
            secondCruiseInit.PopulateDatabase(db);

            var updateTo365 = new UpdateTo_3_6_5();

            using var conn = db.OpenConnection();
            updateTo365.Update(conn);

            var updateTo366 = new UpdateTo_3_6_6();
            updateTo366.Update(conn);


            var updateTo367 = new UpdateTo_3_6_7();
            updateTo367.Update(conn);


            db.Invoking(x => x.Execute("UPDATE TreeAuditRule SET CruiseID = @p1", secondCruiseInit.CruiseID)).Should().Throw<ConstraintException>();
        }
    }
}
