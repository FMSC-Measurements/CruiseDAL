using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync
{
    public class DeleteSyncer_Test : TestBase
    {
        public DeleteSyncer_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Sync_Unit_Delete()
        {
            var init = new DatabaseInitializer();
            var fromPath = base.GetTempFilePath(".crz3", "Sync_Unit_Delete_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Sync_Unit_Delete_toFile");

            using var fromDb = init.CreateDatabaseFile(fromPath);
            var cruiseID = init.CruiseID;
            var saleID = init.SaleID;

            var newUnit = new CuttingUnit()
            {
                CruiseID = cruiseID,
                CuttingUnitID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "newUnitCode1",
            };
            fromDb.Insert(newUnit);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            fromDb.Delete(newUnit);

            var syncer = new DeleteSysncer();
            var syncOptions = new CruiseSyncOptions();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("CuttingUnit", "WHERE CuttingUnitID = @p1", newUnit.CuttingUnitID)
                .Should().Be(0);
        }

        [Fact]
        public void Sync_Stratum_Delete()
        {
            var init = new DatabaseInitializer();
            var fromPath = base.GetTempFilePath(".crz3", "Sync_Stratum_Delete_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Sync_Stratum_Delete_toFile");

            using var fromDb = init.CreateDatabaseFile(fromPath);
            var cruiseID = init.CruiseID;
            var saleID = init.SaleID;

            var newStratum = new Stratum
            {
                CruiseID = cruiseID,
                StratumID = Guid.NewGuid().ToString(),
                StratumCode = "newStratumCode1",
            };
            fromDb.Insert(newStratum);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            fromDb.Delete(newStratum);

            var syncer = new DeleteSysncer();
            var syncOptions = new CruiseSyncOptions();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("Stratum", "WHERE StratumID = @p1", newStratum.StratumID)
                .Should().Be(0);
        }

        [Fact]
        public void Sync_SampleGroup_Delete()
        {
            var init = new DatabaseInitializer();
            var fromPath = base.GetTempFilePath(".crz3", "Sync_SampleGroup_Delete_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Sync_SampleGroup_Delete_toFile");

            using var fromDb = init.CreateDatabaseFile(fromPath);
            var cruiseID = init.CruiseID;
            var saleID = init.SaleID;

            var newStratum = new Stratum
            {
                CruiseID = cruiseID,
                StratumID = Guid.NewGuid().ToString(),
                StratumCode = "newStratumCode1",
            };
            fromDb.Insert(newStratum);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            fromDb.Delete(newStratum);

            var syncer = new DeleteSysncer();
            var syncOptions = new CruiseSyncOptions();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("Stratum", "WHERE StratumID = @p1", newStratum.StratumID)
                .Should().Be(0);
        }
    }
}