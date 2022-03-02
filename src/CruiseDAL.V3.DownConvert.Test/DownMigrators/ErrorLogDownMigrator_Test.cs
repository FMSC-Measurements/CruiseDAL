using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.DownConvert.Test.DownMigrators
{
    public class ErrorLogDownMigrator_Test : TestBase
    {
        public ErrorLogDownMigrator_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void MigrateTreeErrors_countTree()
        {
            var toPath = GetTempFilePath("MigrateTreeErrors_countTree.cruise");
            var fromPath = GetTempFilePath("MigrateTreeErrors_countTree.crz3");

            var init = new DatabaseInitializer();
            var cruiseID = init.CruiseID;
            using var db = init.CreateDatabaseFile(fromPath);

            var tree = new Tree
            {
                CruiseID = cruiseID,
                TreeID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",
                TreeNumber = 1,
                CountOrMeasure = "C",
            };

            db.Insert(tree);

            var tm = new TreeMeasurment
            {
                TreeID = tree.TreeID,
            };
            db.Insert(tm);

            using var toDb = new DAL(toPath, true);

            var downMigrator = new DownMigrator();
            downMigrator.MigrateFromV3ToV2(cruiseID, db, toDb);

            var errorLogs = toDb.From<CruiseDAL.V2.Models.ErrorLog>().Query().ToArray();

            errorLogs.Should().HaveCount(2);
            errorLogs.Should().Contain(x => x.Message == "Species Code Is Missing");
            errorLogs.Should().Contain(x => x.Message == "Live/Dead Value Is Missing");
        }

        [Fact]
        public void MigrateTreeErrors_measureTree()
        {
            var toPath = GetTempFilePath("MigrateTreeErrors_measureTree.cruise");
            var fromPath = GetTempFilePath("MigrateTreeErrors_measureTree.crz3");

            var init = new DatabaseInitializer();
            var cruiseID = init.CruiseID;
            using var db = init.CreateDatabaseFile(fromPath);

            var tree = new Tree
            {
                CruiseID = cruiseID,
                TreeID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",
                TreeNumber = 1,
                CountOrMeasure = "M",
            };

            db.Insert(tree);

            var tm = new TreeMeasurment
            {
                TreeID = tree.TreeID,
            };
            db.Insert(tm);

            using var toDb = new DAL(toPath, true);

            var downMigrator = new DownMigrator();
            downMigrator.MigrateFromV3ToV2(cruiseID, db, toDb);

            var errorLogs = toDb.From<CruiseDAL.V2.Models.ErrorLog>().Query().ToArray();

            errorLogs.Should().HaveCount(4);
            errorLogs.Should().Contain(x => x.Message == "Species Code Is Missing");
            errorLogs.Should().Contain(x => x.Message == "Live/Dead Value Is Missing");
            errorLogs.Should().Contain(x => x.Message == "Aleast One Height Parameter Must Be Greater Than 0");
            errorLogs.Should().Contain(x => x.Message == "DBH or DRC must be greater than 0");
        }

        [Fact]
        public void MigrateTreeErrors_insuranceTree()
        {
            var toPath = GetTempFilePath("MigrateTreeErrors_insuranceTree.cruise");
            var fromPath = GetTempFilePath("MigrateTreeErrors_insuranceTree.crz3");

            var init = new DatabaseInitializer();
            var cruiseID = init.CruiseID;
            using var db = init.CreateDatabaseFile(fromPath);

            var tree = new Tree
            {
                CruiseID = cruiseID,
                TreeID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",
                TreeNumber = 1,
                CountOrMeasure = "I",
            };

            db.Insert(tree);

            var tm = new TreeMeasurment
            {
                TreeID = tree.TreeID,
            };
            db.Insert(tm);

            using var toDb = new DAL(toPath, true);

            var downMigrator = new DownMigrator();
            downMigrator.MigrateFromV3ToV2(cruiseID, db, toDb);

            var errorLogs = toDb.From<CruiseDAL.V2.Models.ErrorLog>().Query().ToArray();

            errorLogs.Should().HaveCount(2);
            errorLogs.Should().Contain(x => x.Message == "Species Code Is Missing");
            errorLogs.Should().Contain(x => x.Message == "Live/Dead Value Is Missing");
        }

        [Fact]
        public void MigrateTreeErrors_multipleWarningsPerTreeField()
        {
            var toPath = GetTempFilePath("MigrateTreeErrors_multipleWarningsPerTreeField.cruise");
            var fromPath = GetTempFilePath("MigrateTreeErrors_multipleWarningsPerTreeField.crz3");

            var init = new DatabaseInitializer();

            using var db = init.CreateDatabaseFile(fromPath);

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
                // we need to set height, otherwise we will get an error on the tree and we are just testing warnings
                TotalHeight = 1, 
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

            var tares = new TreeAuditResolution
            {
                CruiseID = init.CruiseID,
                TreeAuditRuleID = tar.TreeAuditRuleID,
                Initials = "bc",
                Resolution = "something",
                TreeID = tree.TreeID,
            };
            db.Insert(tares);

            var treeErrors = db.From<CruiseDAL.V3.Models.TreeError>().Query().ToArray();

            using var toDb = new DAL(toPath, true);

            var downMigrator = new DownMigrator();
            downMigrator.MigrateFromV3ToV2(init.CruiseID, db, toDb);

            var errors = toDb.From<CruiseDAL.V2.Models.ErrorLog>().Query().ToArray();
            errors.Should().HaveCount(1);

            var error = errors.Single();
            error.Suppress.Should().Be(false);

        }
    }
}