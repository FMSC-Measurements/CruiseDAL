using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using CruiseDAL.V3.Sync.Syncers;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync.Test.UnitTests.Syncers
{
    public class Species_ProductSyncer_Test : SyncTestBase
    {
        public Species_ProductSyncer_Test(ITestOutputHelper output) : base(output)
        {
        }

        // Species Product table only supports insert right now. Maybe if we add change tracking we could implement update

        [Fact]
        public void SyncRecords_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Species_Product_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Species_Product_toFile");

            var syncOptions = new TableSyncOptions();

            var init = new DatabaseInitializer();

            var spCode = init.Species.First();

            using var fromDb = init.CreateDatabaseFile(fromPath);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var newSpProd = new Species_Product
            {
                CruiseID = init.CruiseID,
                SpeciesCode = spCode,
                PrimaryProduct = "01",
                ContractSpecies = "something",
            };
            fromDb.Insert(newSpProd);

            var syncer = new Species_ProductSyncer();
            syncer.SyncRecords(init.CruiseID, fromDb.OpenConnection(), toDb.OpenConnection(), syncOptions, (IExceptionProcessor)null);

            var spProdAgain = toDb.From<Species_Product>().Where("SpeciesCode = @p1 AND PrimaryProduct = @p2")
                .Query(spCode, "01").First();

            spProdAgain.Should().BeEquivalentTo(newSpProd);
        }
    }
}
