﻿using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync.Test.UnitTests
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
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_Unit_Delete_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_Unit_Delete_toFile");

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
            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                CuttingUnit = SyncOption.Delete,
            };


            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("CuttingUnit", "WHERE CuttingUnitID = @p1", newUnit.CuttingUnitID)
                .Should().Be(0);
        }

        [Fact]
        public void Sync_Stratum_Delete()
        {
            var init = new DatabaseInitializer();
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_Stratum_Delete_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_Stratum_Delete_toFile");

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
            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                Stratum = SyncOption.Delete,
            };
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("Stratum", "WHERE StratumID = @p1", newStratum.StratumID)
                .Should().Be(0);
        }

        [Fact]
        public void Sync_SampleGroup_Delete()
        {
            var init = new DatabaseInitializer();
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_SampleGroup_Delete_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_SampleGroup_Delete_toFile");

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

            var newSG = new SampleGroup
            {
                CruiseID = cruiseID,
                StratumCode = newStratum.StratumCode,
                SampleGroupID = Guid.NewGuid().ToString(),
                SampleGroupCode = "newSt1",
            };
            fromDb.Insert(newSG);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            fromDb.Delete(newStratum);
            fromDb.Delete(newSG);

            var syncer = new DeleteSysncer();
            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                SampleGroup = SyncOption.Delete,
            };
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("Stratum", "WHERE StratumID = @p1", newStratum.StratumID)
                .Should().Be(1);
            toDb.GetRowCount("SampleGroup", "WHERE SampleGroupID = @p1", newSG.SampleGroupID)
                .Should().Be(0);
        }

        [Fact]
        public void Sync_TallyLedger_Delete()
        {
            var init = new DatabaseInitializer();
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_SampleGroup_Delete_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_SampleGroup_Delete_toFile");

            using var fromDb = init.CreateDatabaseFile(fromPath);
            var cruiseID = init.CruiseID;
            var saleID = init.SaleID;

            var tallyLedger = new TallyLedger
            {
                CruiseID = cruiseID,
                TallyLedgerID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",
            };
            fromDb.Insert(tallyLedger);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            fromDb.Delete(tallyLedger);

            var syncer = new DeleteSysncer();
            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                TallyLedger = SyncOption.Delete,
            };
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("TallyLedger", "WHERE TallyLedgerID = @p1", tallyLedger.TallyLedgerID)
                .Should().Be(0);
            toDb.GetRowCount("TallyLedger_Tombstone", "WHERE TallyLedgerID = @p1", tallyLedger.TallyLedgerID)
                .Should().Be(1);
        }
    }
}