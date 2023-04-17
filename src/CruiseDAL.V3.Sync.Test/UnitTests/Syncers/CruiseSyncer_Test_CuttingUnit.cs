using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CruiseDAL.V3.Sync.Test.UnitTests.Syncers
{
    public partial class CruiseSyncer_Test
    {
        [Fact]
        public void Sync_CuttingUnit_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "CuttingUnit_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "CuttingUnit_Add_toFile");

            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                CuttingUnit = SyncOption.Insert,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var cuttingUnitID = Guid.NewGuid().ToString();
            fromDb.Insert(new CuttingUnit()
            {
                CruiseID = cruiseID,
                CuttingUnitID = cuttingUnitID,
                CuttingUnitCode = "10",
            });
            var newCuttingUnit = fromDb.From<CuttingUnit>().Where("CuttingUnitID = @p1").Query(cuttingUnitID).Single();

            var syncer = new CruiseDatabaseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var cuttingUnitAgain = toDb.From<CuttingUnit>().Where("CuttingUnitID =  @p1")
                .Query(cuttingUnitID).FirstOrDefault();
            cuttingUnitAgain.Should().BeEquivalentTo(newCuttingUnit, x => x.Excluding(y => y.Modified_TS));
        }

        [Fact]
        public void Sync_CuttingUnit_Update()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "CuttingUnit_Updated_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "CuttingUnit_Update_toFile");

            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                CuttingUnit = SyncOption.Update,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            // initialize source database
            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);
            var cuttingUnitID = Guid.NewGuid().ToString();
            var cuttingUnit = new CuttingUnit()
            {
                CruiseID = cruiseID,
                CuttingUnitID = cuttingUnitID,
                CuttingUnitCode = "10",
            };
            fromDb.Insert(cuttingUnit);

            // initialize dest database 
            // as exact copy of source database
            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            // modify cutting unit value and save to source database
            cuttingUnit.Area = Rand.Int(1);
            cuttingUnit.Description = Rand.Words();
            cuttingUnit.LoggingMethod = "401";
            cuttingUnit.PaymentUnit = Rand.AlphaNumeric(3);
            cuttingUnit.Rx = Rand.AlphaNumeric(3);
            cuttingUnit.ModifiedBy = Rand.AlphaNumeric(4);
            fromDb.Update(cuttingUnit);

            // run sync
            var syncer = new CruiseDatabaseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var cuttingUnitAgain = toDb.From<CuttingUnit>().Where("CuttingUnitID =  @p1")
                .Query(cuttingUnitID).FirstOrDefault();
            cuttingUnitAgain.Should().BeEquivalentTo(cuttingUnit, x => x.Excluding(y => y.Modified_TS));
        }

        [Fact]
        public void Sync_CuttingUnit_Update_CuttingUnitCode()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "CuttingUnit_Updated_code_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "CuttingUnit_Update_code_toFile");

            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                CuttingUnit = SyncOption.Update,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            // initialize source database
            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);
            var cuttingUnitID = Guid.NewGuid().ToString();
            var cuttingUnit = new CuttingUnit()
            {
                CruiseID = cruiseID,
                CuttingUnitID = cuttingUnitID,
                CuttingUnitCode = "10",
            };
            fromDb.Insert(cuttingUnit);

            // initialize dest database 
            // as exact copy of source database
            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            // modify cutting unit value and save to source database
            cuttingUnit.CuttingUnitCode = "11";
            fromDb.Update(cuttingUnit);

            // run sync
            var syncer = new CruiseDatabaseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var cuttingUnitAgain = toDb.From<CuttingUnit>().Where("CuttingUnitID =  @p1")
                .Query(cuttingUnitID).FirstOrDefault();
            cuttingUnitAgain.CuttingUnitCode.Should().BeEquivalentTo(cuttingUnit.CuttingUnitCode);
        }
    }
}
