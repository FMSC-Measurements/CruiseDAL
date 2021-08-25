using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace CruiseDAL.V3.Test.Schema.Views
{
    public class TreeAuditError_Test
    {
        [Fact]
        public void ReadTreeAuditErrors()
        {
            var init = new DatabaseInitializer();

            using var db = init.CreateDatabase();

            var tree = new Tree
            {
                CruiseID = init.CruiseID,
                TreeID = Guid.NewGuid().ToString(),
                TreeNumber = 1,
                CuttingUnitCode = "u1",
                StratumCode = "st3",
                SampleGroupCode = "sg1",
                SpeciesCode = "sp1",
                CountOrMeasure = "M",
                LiveDead = "L",
            };
            db.Insert(tree);

            var tm = new TreeMeasurment
            {
                TreeID = tree.TreeID,
                DBH = 101,
            };
            db.Insert(tm);

            var tar = new TreeAuditRule
            {
                CruiseID = init.CruiseID,
                TreeAuditRuleID = Guid.NewGuid().ToString(),
                Field = "DBH",
                Max = 100,
            };

            db.Insert(tar);

            var tars = new TreeAuditRuleSelector
            {
                CruiseID = init.CruiseID,
                TreeAuditRuleID = tar.TreeAuditRuleID,
                SpeciesCode = "sp1",
                PrimaryProduct = "01",
                LiveDead = "L",
            };
            db.Insert(tars);

            var tfs = new TreeFieldSetup
            {
                CruiseID = init.CruiseID,
                Field = "DBH",
                StratumCode = "st3",
            };
            db.Insert(tfs);

            var tar2 = new TreeAuditRule
            {
                CruiseID = init.CruiseID,
                TreeAuditRuleID = Guid.NewGuid().ToString(),
                Field = "DBH",
                Max = 99,
            };

            db.Insert(tar2);

            var tars2 = new TreeAuditRuleSelector
            {
                CruiseID = init.CruiseID,
                TreeAuditRuleID = tar.TreeAuditRuleID,
                SpeciesCode = "sp1",
            };
            db.Insert(tars2);

            var taeRecords = db.QueryGeneric("SELECT * FROM TreeAuditError").ToArray();
            taeRecords.Should().HaveCount(1);
        }
    }
}