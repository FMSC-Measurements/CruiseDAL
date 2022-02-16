using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
