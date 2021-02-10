using Bogus;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync
{
    public class CruiseSyncer_Test : Datastore_TestBase
    {
        public CruiseSyncer_Test(ITestOutputHelper output) : base(output)
        {
        }

        //public void Sync_Copy()
        //{
        //    var sourcePath = "";
        //    var destPath = GetTempFilePath(".crz3");
        //}

        [Fact]
        public void Sync_Unit_Add()
        {
            var fromPath = base.GetTempFilePath(".crz3", "fromFile");
            var toPath = base.GetTempFilePath(".crz3", "toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Design = SyncFlags.Insert,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabase(fromPath, cruiseID, saleID);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var newUnit = new CuttingUnit()
            {
                CruiseID = cruiseID,
                CuttingUnitID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "newUnitCode1",
            };
            fromDb.Insert(newUnit);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("CuttingUnit", "WHERE CuttingUnitID = @p1", newUnit.CuttingUnitID)
                .Should().Be(1);
        }

        [Fact]
        public void Sync_Unit_Update()
        {
            var fromPath = base.GetTempFilePath(".crz3", "fromFile");
            var toPath = base.GetTempFilePath(".crz3", "toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Design = SyncFlags.Update,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabase(fromPath, cruiseID, saleID);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var unit = fromDb.From<CuttingUnit>().Query().First();
            unit.Area = Rand.Int();
            unit.Description = Rand.String();
            unit.LoggingMethod = "401";
            unit.PaymentUnit = Rand.AlphaNumeric(3);
            unit.Rx = Rand.AlphaNumeric(3);
            unit.ModifiedBy = Rand.AlphaNumeric(4);
            fromDb.Update(unit);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var unitAgain = toDb.From<CuttingUnit>().Where("CuttingUnitID = @p1").Query(unit.CuttingUnitID).First();

            unitAgain.Should().BeEquivalentTo(unit, x => x.Excluding(y => y.Modified_TS));
        }


        [Fact]
        public void Sync_UnitStratum_Add()
        {
            var fromPath = base.GetTempFilePath(".crz3", "fromFile");
            var toPath = base.GetTempFilePath(".crz3", "toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Design = SyncFlags.Insert,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabase(fromPath, cruiseID, saleID);
            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var newCUST = new CuttingUnit_Stratum()
            {
                CruiseID = cruiseID,
                CuttingUnitCode = Units[1],
                StratumCode = NonPlotStrata[0].StratumCode,
            };
            fromDb.Insert(newCUST);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("CuttingUnit_Stratum", "WHERE CuttingUnitCode = @p1 AND StratumCode = @p2", newCUST.CuttingUnitCode, newCUST.StratumCode)
                .Should().Be(1);
        }

        [Fact]
        public void Sync_Device_Add()
        {
            var fromPath = base.GetTempFilePath(".crz3", "fromFile");
            var toPath = base.GetTempFilePath(".crz3", "toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Design = SyncFlags.Insert,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabase(fromPath, cruiseID, saleID);
            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var newDevice = new Device()
            {
                CruiseID = cruiseID,
                DeviceID = Rand.Guid().ToString(),
            };
            fromDb.Insert(newDevice);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("Device", "WHERE DeviceID = @p1 AND CruiseID = @p2", newDevice.DeviceID, newDevice.CruiseID)
                .Should().Be(1);
        }


    }
}
