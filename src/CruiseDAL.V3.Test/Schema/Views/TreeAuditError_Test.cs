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

            var taeRecords = db.QueryGeneric("SELECT * FROM TreeAuditError").ToArray();
            taeRecords.Should().HaveCount(1);
        }

        [Fact]
        public void ReadTreeAuditErrors_With_Two_Selectors()
        {
            var init = new DatabaseInitializer();

            using var db = init.CreateDatabase();

            var tfs = new TreeFieldSetup
            {
                CruiseID = init.CruiseID,
                Field = "DBH",
                StratumCode = "st3",
            };
            db.Insert(tfs);

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

            var tars2 = new TreeAuditRuleSelector
            {
                CruiseID = init.CruiseID,
                TreeAuditRuleID = tar.TreeAuditRuleID,
                SpeciesCode = "sp1",
            };
            db.Insert(tars2);

            var taeRecords = db.QueryGeneric("SELECT * FROM TreeAuditError").ToArray();
            taeRecords.Should().HaveCount(1); // not sure how to make TreeAuditErrors return only one error per tree, but its a low impact issue
        }

        [Fact]
        public void ReadTreeAuditErrors_With_Two_AuditRules()
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
                TreeAuditRuleID = tar2.TreeAuditRuleID,
                SpeciesCode = "sp1",
            };
            db.Insert(tars2);

            var taeRecords = db.QueryGeneric("SELECT * FROM TreeAuditError").ToArray();
            taeRecords.Should().HaveCount(2); // not sure how to make TreeAuditErrors return only one error per tree, but its a low impact issue
        }

        [Theory]
        [InlineData(101.0, 0.0, 100, true)]
        [InlineData(100.01, 0.0, 100, true)]
        [InlineData(100.01, 0.0, 100.01, false)]

        [InlineData(100.011, 0.0, 100.01, false)]
        [InlineData(100.014, 0.0, 100.01, false)]
        [InlineData(100.015, 0.0, 100.01, false)]// sqlite round() rounds down from midpoint
        [InlineData(100.016, 0.0, 100.01, true)]

        [InlineData(100.02, 0.0, 100.011, true)]
        [InlineData(100.02, 0.0, 100.014, true)]
        [InlineData(100.02, 0.0, 100.015, true)]
        [InlineData(100.02, 0.0, 100.016, false)]


        //[InlineData(102.02, 0.0, 102.011, true)]
        //[InlineData(102.02, 0.0, 102.014, true)]
        //[InlineData(102.02, 0.0, 102.015, false)]
        //[InlineData(102.02, 0.0, 102.016, false)]

        public void FailingAudit_Rounding(double treeValue, double min, double max, bool isWarningExpected)
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
                DBH = treeValue,
            };
            db.Insert(tm);

            var tar = new TreeAuditRule
            {
                CruiseID = init.CruiseID,
                TreeAuditRuleID = Guid.NewGuid().ToString(),
                Field = "DBH",
                Min = min,
                Max = max,
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

            var taeRecords = db.QueryGeneric("SELECT * FROM TreeAuditError").ToArray();
            if (isWarningExpected)
            { taeRecords.Should().HaveCount(1); }
            else
            { taeRecords.Should().HaveCount(0); }
        }
    }
}