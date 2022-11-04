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
        public void Sync_Plot_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Add_toFile");

            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                Plot = SyncOption.Insert,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var newPlot = new Plot()
            {
                CruiseID = cruiseID,
                PlotID = Guid.NewGuid().ToString(),
                CuttingUnitCode = Units[0],
                PlotNumber = 1,
            };
            fromDb.Insert(newPlot);

            var syncer = new CruiseDatabaseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("Plot", "WHERE PlotID = @p1", newPlot.PlotID)
                .Should().Be(1);
        }

        [Fact]
        public void Sync_Plot_Update()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Update_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Update_toFile");

            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                Plot = SyncOption.Update,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            var plot = new Plot()
            {
                CruiseID = cruiseID,
                PlotID = Guid.NewGuid().ToString(),
                CuttingUnitCode = Units[0],
                PlotNumber = 1,
            };
            fromDb.Insert(plot);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            plot.Remarks = "something";
            plot.Slope = 101;
            plot.PlotNumber = 2;
            fromDb.Update(plot);

            var syncer = new CruiseDatabaseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var plotAgian = toDb
                .From<Plot>()
                .Where("PlotID = @p1")
                .Query(plot.PlotID)
                .FirstOrDefault();

            plotAgian.Should().BeEquivalentTo(plot, config => config.Excluding(x => x.Modified_TS));
        }

        [Fact]
        public void Sync_Plot_Update_PlotNumber()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Update_plotNum_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Update_plotNum_toFile");

            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                Plot = SyncOption.Update,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            var plot = new Plot()
            {
                CruiseID = cruiseID,
                PlotID = Guid.NewGuid().ToString(),
                CuttingUnitCode = Units[0],
                PlotNumber = 1,
            };
            fromDb.Insert(plot);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            plot.PlotNumber = 2;
            fromDb.Update(plot);

            var syncer = new CruiseDatabaseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var plotAgian = toDb
                .From<Plot>()
                .Where("PlotID = @p1")
                .Query(plot.PlotID)
                .FirstOrDefault();

            plotAgian.Should().BeEquivalentTo(plot, config => config.Excluding(x => x.Modified_TS));
        }
    }
}
