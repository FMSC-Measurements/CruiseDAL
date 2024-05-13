using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CruiseDAL.TestCommon;
using CruiseDAL.Update;
using CruiseDAL.V3.Models;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test.Update
{
    public class UpdateTo_366_Test : TestBase
    {
        public UpdateTo_366_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void UpdateFrom365()
        {
            var filePath = InitializeTestFile("364_AllTables.crz3");

            using var db = new CruiseDatastore_V3(filePath);

            var tars = db.From<TreeAuditRule>().Query().ToArray();
            var taRess = db.From<TreeAuditResolution>().Query().ToArray();

            var updateTo365 = new UpdateTo_3_6_5();

            using var conn = db.OpenConnection();
            updateTo365.Update(conn);

            var updateTo366 = new UpdateTo_3_6_6();
            updateTo366.Update(conn);

            foreach(var tar in tars)
            {
                db.From<TreeAuditRule>().Where("TreeAuditRuleID = @p1").Count(tar.TreeAuditRuleID).Should().Be(0, tar.TreeAuditRuleID);
                db.From<TreeAuditRule>().Where("TreeAuditRule_CN = @p1").Count(tar.TreeAuditRule_CN).Should().Be(1, tar.TreeAuditRule_CN.ToString());
            }

            foreach(var tares in taRess)
            {
                db.From<TreeAuditResolution>().Where("TreeAuditResolution_CN = @p1").Count(tares.TreeAuditResolution_CN).Should().Be(1, tares.TreeAuditResolution_CN.ToString());
            }

            db.QueryGeneric("PRAGMA foreign_key_check;").Should().BeEmpty();


        }
    }
}
