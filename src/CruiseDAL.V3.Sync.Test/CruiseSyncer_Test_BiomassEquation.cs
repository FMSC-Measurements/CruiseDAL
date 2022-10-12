using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CruiseDAL.V3.Sync
{
    public partial class CruiseSyncer_Test 
    {
        [Fact]
        public void Sync_BiomassEquation_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_BiomassEquations_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_BiomassEquations_Add_toFile");

            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                Processing = SyncOption.InsertUpdate,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var newBiomassEquation = new BiomassEquation
            {
                CruiseID = cruiseID,
                Component = "sfdf",
                Species = "sp1",
                Product = "01",
                LiveDead = "L",
            };
            fromDb.Insert(newBiomassEquation);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.From<BiomassEquation>().Count().Should().Be(1);
        }
    }
}
