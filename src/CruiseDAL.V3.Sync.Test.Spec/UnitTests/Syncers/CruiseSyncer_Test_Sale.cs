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
        public void Sync_Sale_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sale_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sale_Add_toFile");

            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                Sale = SyncOption.Insert,
                Cruise = SyncOption.Insert,
            };

            var cruiseID = CruiseID;
            var saleID = SaleID;

            using var fromDb = CreateDatabaseFile(fromPath);

            var sale = fromDb.From<Sale>()
                .Where("SaleID = @p1")
                .Query(saleID)
                .FirstOrDefault();

            using var toDb = new CruiseDatastore_V3(toPath, true);

            var syncer = new CruiseDatabaseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var saleAgain = toDb.From<Sale>().Where("SaleID = @p1").Query(saleID).FirstOrDefault();

            saleAgain.Should().BeEquivalentTo(sale, x => x.Excluding(y => y.Modified_TS));
        }

        [Fact]
        public void Sync_Sale_Update()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sale_Update_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sale_Update_toFile");

            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                Sale = SyncOption.Update,
            };

            var cruiseID = CruiseID;
            var saleID = SaleID;

            using var fromDb = CreateDatabaseFile(fromPath);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var sale = fromDb.From<Sale>()
                .Where("SaleID = @p1")
                .Query(saleID)
                .FirstOrDefault();
            sale.Remarks = Rand.Words();
            fromDb.Update(sale);

            var syncer = new CruiseDatabaseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var saleAgain = toDb.From<Sale>().Where("SaleID = @p1").Query(saleID).FirstOrDefault();

            saleAgain.Should().BeEquivalentTo(sale, x => x.Excluding(y => y.Modified_TS));
        }

        [Fact]
        public void Sync_Sale_Update_ModifySaleNumber()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_Sale_Update_ModifySaleNumber_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_Sale_Update_ModifySaleNumber_toFile");

            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                Sale = SyncOption.Update,
            };

            var cruiseID = CruiseID;
            var saleID = SaleID;

            using var fromDb = CreateDatabaseFile(fromPath);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var newSaleNumber = "12345";

            var sale = fromDb.From<Sale>()
                .Where("SaleID = @p1")
                .Query(saleID)
                .FirstOrDefault();
            sale.SaleNumber = newSaleNumber;
            fromDb.Update(sale);

            var cruise = fromDb.From<Cruise>().Query().FirstOrDefault();

            var syncer = new CruiseDatabaseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var saleAgain = toDb.From<Sale>().Where("SaleID = @p1").Query(saleID).FirstOrDefault();

            saleAgain.Should().BeEquivalentTo(sale, x => x.Excluding(y => y.Modified_TS));

            var cruiseAgain = toDb.From<Cruise>().Where("SaleNumber = @p1").Query(newSaleNumber).FirstOrDefault();
            cruiseAgain.Should().NotBeNull();
            cruiseAgain.Should().BeEquivalentTo(cruise, x => x.Excluding(y => y.Modified_TS));
        }
    }
}
