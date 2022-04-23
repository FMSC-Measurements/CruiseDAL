using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync
{
    public partial class CruiseSyncer_Test : Datastore_TestBase
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
        public void Sync_Sale_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sale_Add_fromFile"); 
            var toPath = GetTempFilePathWithExt(".crz3", "Sale_Add_toFile");

            var syncOptions = new CruiseSyncOptions();

            var cruiseID = CruiseID;
            var saleID = SaleID;

            using var fromDb = CreateDatabaseFile(fromPath);

            var sale = fromDb.From<Sale>()
                .Where("SaleID = @p1")
                .Query(saleID)
                .FirstOrDefault();

            using var toDb = new CruiseDatastore_V3(toPath, true);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var saleAgain = toDb.From<Sale>().Where("SaleID = @p1").Query(saleID).FirstOrDefault();

            saleAgain.Should().BeEquivalentTo(sale, x => x.Excluding(y => y.Modified_TS));
        }

        [Fact]
        public void Sync_Sale_Update()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sale_Update_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sale_Update_toFile");

            var syncOptions = new CruiseSyncOptions();

            var cruiseID = CruiseID;
            var saleID = SaleID;

            using var fromDb = CreateDatabaseFile(fromPath);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var sale = fromDb.From<Sale>()
                .Where("SaleID = @p1")
                .Query(saleID)
                .FirstOrDefault();
            sale.Remarks = Rand.String();
            fromDb.Update(sale);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var saleAgain = toDb.From<Sale>().Where("SaleID = @p1").Query(saleID).FirstOrDefault();

            saleAgain.Should().BeEquivalentTo(sale, x => x.Excluding(y => y.Modified_TS));
        }

        [Fact]
        public void Sync_Cruise_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Cruise_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Cruise_Add_toFile");

            var syncOptions = new CruiseSyncOptions();

            var init = new DatabaseInitializer();
            var cruiseID = init.CruiseID;
            var saleID = init.SaleID;
            var saleNumber = init.SaleNumber;

            using var fromDb = init.CreateDatabaseFile(fromPath);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var newCruise = new Cruise
            {
                CruiseID = Guid.NewGuid().ToString(),
                CruiseNumber = "123456789",
                SaleID = saleID,
                SaleNumber = saleNumber,
            };
            fromDb.Insert(newCruise);
            newCruise = fromDb.From<Cruise>().Where("CruiseID = @p1")
                .Query(newCruise.CruiseID).FirstOrDefault();

            var syncer = new CruiseSyncer();
            syncer.Sync(newCruise.CruiseID, fromDb, toDb, syncOptions);

            var newCruiseAgain = toDb.From<Cruise>()
                .Where("CruiseID = @p1")
                .Query(newCruise.CruiseID).FirstOrDefault();

            newCruiseAgain.Should().NotBeNull();
            newCruiseAgain.Should().BeEquivalentTo(newCruise, x => x
                .Excluding(y => y.Modified_TS));
        }

        [Fact]
        public void Sync_Cruise_Add_NonMatchingSaleID()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Cruise_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Cruise_Add_toFile");

            var syncOptions = new CruiseSyncOptions();

            var init = new DatabaseInitializer();
            var cruiseID = init.CruiseID;
            var saleID = init.SaleID;
            var saleNumber = init.SaleNumber;

            using var fromDb = init.CreateDatabaseFile(fromPath);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var newCruise = new Cruise
            {
                CruiseID = Guid.NewGuid().ToString(),
                CruiseNumber = "123456789",
                SaleID = Guid.NewGuid().ToString(),
                SaleNumber = saleNumber,
            };
            fromDb.Insert(newCruise);
            newCruise = fromDb.From<Cruise>().Where("CruiseID = @p1")
                .Query(newCruise.CruiseID).FirstOrDefault();

            var syncer = new CruiseSyncer();
            syncer.Sync(newCruise.CruiseID, fromDb, toDb, syncOptions);

            var newCruiseAgain = toDb.From<Cruise>()
                .Where("CruiseID = @p1")
                .Query(newCruise.CruiseID).FirstOrDefault();

            newCruiseAgain.Should().NotBeNull();
            newCruiseAgain.Should().BeEquivalentTo(newCruise, x => x
                .Excluding(y => y.Modified_TS));
        }

        [Fact]
        public void Sync_Cruise_Update()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Cruise_Update_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Cruise_Update_toFile");

            var syncOptions = new CruiseSyncOptions();

            var cruiseID = CruiseID;
            var saleID = SaleID;

            using var fromDb = CreateDatabaseFile(fromPath);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var cruise = fromDb.From<Cruise>()
                .Where("CruiseID = @p1")
                .Query(cruiseID)
                .FirstOrDefault();
            cruise.Remarks = Rand.String();
            fromDb.Update(cruise);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var cruiseAgain = toDb.From<Cruise>()
                .Where("CruiseID = @p1")
                .Query(cruiseID).FirstOrDefault();

            cruiseAgain.Should().BeEquivalentTo(cruise, x => x.Excluding(y => y.Modified_TS));
        }

        [Fact]
        public void Sync_Device_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Design = SyncFlags.Insert,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);
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

        [Fact]
        public void Sync_UnitStratum_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Design = SyncFlags.Insert,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);
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
        public void Sync_SubPopulation_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "SubPopulation_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "SubPopulation_Add_toFile");

            var syncOptions = new CruiseSyncOptions();

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var sampleGroup = SampleGroups[0];
            var subPopID = Guid.NewGuid().ToString();
            var newSubpopulation = new SubPopulation()
            {
                CruiseID = cruiseID,
                SubPopulationID = subPopID,
                StratumCode = sampleGroup.StratumCode,
                SampleGroupCode = sampleGroup.SampleGroupCode,
                SpeciesCode = Species[3],
                LiveDead = "L",
            };
            fromDb.Insert(newSubpopulation);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var subPopulationAgain = toDb.From<SubPopulation>()
                .Where("SubPopulationID = @p1")
                .Query(subPopID).FirstOrDefault();
            subPopulationAgain.Should().BeEquivalentTo(newSubpopulation, x => x
                .Excluding(y => y.Modified_TS)
                .Excluding(y => y.Created_TS)
                .Excluding(y => y.CreatedBy));
        }

        

        

        [Fact]
        public void Sync_Plot_Stratum_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Add_toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Design = SyncFlags.Insert,
                FieldData = SyncFlags.Insert,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            var newPlot = new Plot()
            {
                CruiseID = cruiseID,
                PlotID = Guid.NewGuid().ToString(),
                CuttingUnitCode = Units[0],
                PlotNumber = 1,
            };
            fromDb.Insert(newPlot);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var newPlotStratum = new Plot_Stratum()
            {
                CruiseID = cruiseID,
                PlotNumber = newPlot.PlotNumber,
                CuttingUnitCode = newPlot.CuttingUnitCode,
                StratumCode = PlotStrata[0].StratumCode,
            };
            fromDb.Insert(newPlotStratum);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("Plot", "WHERE PlotID = @p1", newPlot.PlotID)
                .Should().Be(1);
            toDb.ExecuteScalar2<int>("SELECT COUNT(*) FROM Plot_Stratum WHERE CruiseID = @CruiseID AND PlotNumber = @PlotNumber AND CuttingUnitCode = @CuttingUnitCode", newPlotStratum)
                .Should().Be(1);
        }

        [Fact]
        public void Sync_Plot_Stratum_Update()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Add_toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Design = SyncFlags.Insert,
                FieldData = SyncFlags.InsertUpdate,
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

            var plotStratum = new Plot_Stratum()
            {
                CruiseID = cruiseID,
                PlotNumber = plot.PlotNumber,
                CuttingUnitCode = plot.CuttingUnitCode,
                StratumCode = PlotStrata[0].StratumCode,
            };
            fromDb.Insert(plotStratum);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            plotStratum.KPI = Rand.Double();
            plotStratum.ThreePRandomValue = Rand.Int();
            toDb.Update(plotStratum);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var plotStratumAgain = toDb
                .From<Plot_Stratum>()
                .Where("Plot_Stratum_CN = @p1")
                .Query(plotStratum.Plot_Stratum_CN)
                .FirstOrDefault();

            plotStratumAgain.Should().BeEquivalentTo(plotStratum, config => config.Excluding(x => x.Modified_TS));
        }

        [Fact]
        public void Sync_PlotLocation_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Add_toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Design = SyncFlags.Insert,
                FieldData = SyncFlags.Insert,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            var newPlot = new Plot()
            {
                CruiseID = cruiseID,
                PlotID = Guid.NewGuid().ToString(),
                CuttingUnitCode = Units[0],
                PlotNumber = 1,
            };
            fromDb.Insert(newPlot);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var newPlotLocation = new PlotLocation()
            {
                PlotID = newPlot.PlotID,
            };
            fromDb.Insert(newPlotLocation);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("Plot", "WHERE PlotID = @p1", newPlot.PlotID)
                .Should().Be(1);
            toDb.GetRowCount("PlotLocation", "WHERE PlotID = @p1", newPlot.PlotID)
                .Should().Be(1);
        }

        [Fact]
        public void Sync_PlotLocation_Update()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Add_toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Design = SyncFlags.Insert,
                FieldData = SyncFlags.Update,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            var newPlot = new Plot()
            {
                CruiseID = cruiseID,
                PlotID = Guid.NewGuid().ToString(),
                CuttingUnitCode = Units[0],
                PlotNumber = 1,
            };
            fromDb.Insert(newPlot);

            var plotLocation = new PlotLocation()
            {
                PlotID = newPlot.PlotID,
            };
            fromDb.Insert(plotLocation);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            plotLocation.Latitude = Rand.Double();
            plotLocation.Longitude = Rand.Double();
            fromDb.Update(plotLocation);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var plotLocationAgain = toDb.From<PlotLocation>()
                .Where("PlotID = @p1")
                .Query(plotLocation.PlotID)
                .FirstOrDefault();

            plotLocationAgain.Should().BeEquivalentTo(plotLocation, config => config.Excluding(x => x.Modified_TS));
        }

        [Fact]
        public void Sync_TallyLedger_With_PlotTree_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_Unit_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_Unit_Add_toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Design = SyncFlags.Insert,
                FieldData = SyncFlags.Insert,
                TreeDataFlags = SyncFlags.Insert,
                TreeFlags = SyncFlags.Insert,
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

            var plotStratum = new Plot_Stratum()
            {
                CruiseID = cruiseID,
                PlotNumber = plot.PlotNumber,
                CuttingUnitCode = plot.CuttingUnitCode,
                StratumCode = PlotStrata[0].StratumCode,
            };
            fromDb.Insert(plotStratum);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var newPlotTree = new Tree()
            {
                TreeID = Guid.NewGuid().ToString(),
                CruiseID = cruiseID,
                CuttingUnitCode = plot.CuttingUnitCode,
                PlotNumber = plot.PlotNumber,
                StratumCode = plotStratum.StratumCode,
                SampleGroupCode = SampleGroups[0].SampleGroupCode,
                SpeciesCode = Species[0],
                TreeNumber = 1,
            };
            fromDb.Insert(newPlotTree);

            var newTallyLedger = new TallyLedger()
            {
                CruiseID = cruiseID,
                TallyLedgerID = Guid.NewGuid().ToString(),
                CuttingUnitCode = newPlotTree.CuttingUnitCode,
                StratumCode = newPlotTree.StratumCode,
                PlotNumber = newPlotTree.PlotNumber,
                SampleGroupCode = newPlotTree.SampleGroupCode,
                TreeID = newPlotTree.TreeID,
                TreeCount = 1,
            };
            fromDb.Insert(newTallyLedger);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("TallyLedger", "WHERE TallyLedgerID = @p1", newTallyLedger.TallyLedgerID)
                .Should().Be(1);
        }

        [Fact]
        public void Sync_TallyLedger_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_Unit_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_Unit_Add_toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Design = SyncFlags.Insert,
                FieldData = SyncFlags.Insert,
                TreeDataFlags = SyncFlags.Insert,
                TreeFlags = SyncFlags.Insert,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var newTallyLedger = new TallyLedger()
            {
                CruiseID = cruiseID,
                TallyLedgerID = Guid.NewGuid().ToString(),
                CuttingUnitCode = Units[0],
                StratumCode = SampleGroups[0].StratumCode,
                SampleGroupCode = SampleGroups[0].SampleGroupCode,
                TreeCount = 1,
            };
            fromDb.Insert(newTallyLedger);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("TallyLedger", "WHERE TallyLedgerID = @p1", newTallyLedger.TallyLedgerID)
                .Should().Be(1);
        }

        [Fact]
        public void Sync_Log_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Add_toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                FieldData = SyncFlags.Insert,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            var tree = new Tree()
            {
                TreeID = Guid.NewGuid().ToString(),
                CruiseID = cruiseID,
                CuttingUnitCode = Units[0],
                StratumCode = Strata[0].StratumCode,
                SampleGroupCode = SampleGroups[0].SampleGroupCode,
                SpeciesCode = Species[0],
                TreeNumber = 1,
            };
            fromDb.Insert(tree);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var newLog = new Log()
            {
                LogID = Guid.NewGuid().ToString(),
                CruiseID = cruiseID,
                LogNumber = "01",
                TreeID = tree.TreeID,
            };
            fromDb.Insert(newLog);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("Log", "WHERE LogID = @p1", newLog.LogID)
                .Should().Be(1);
        }

        [Fact]
        public void Sync_Reports_Add()
        {
            var rand = new Bogus.Randomizer();
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_Reports_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_Reports_Add_toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Processing = SyncFlags.Insert,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var reportID = Guid.NewGuid().ToString();
            var report = new Reports
            {
                CruiseID = cruiseID,
                ReportID = reportID,
                Title = rand.String(),
            };
            fromDb.Insert(report);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("Reports", "WHERE ReportID = @p1", reportID).Should().Be(1);
        }

        public void Sync_Reports_Update()
        {
        }

        [Fact]
        public void SyncValueEquations_Add()
        {
            var rand = new Bogus.Randomizer();
            var fromPath = GetTempFilePathWithExt(".crz3", "SyncValueEquations_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "SyncValueEquations_Add_toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Processing = SyncFlags.Insert,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var valueEq = new ValueEquation
            {
                CruiseID = cruiseID,
                Species = "sp1",
                PrimaryProduct = "01",
                ValueEquationNumber = "something",
            };
            fromDb.Insert(valueEq);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("ValueEquation", "WHERE ValueEquationNumber = @p1", valueEq.ValueEquationNumber).Should().Be(1);
        }

        [Fact]
        public void SyncVolumeEquations_Add()
        {
            var rand = new Bogus.Randomizer();
            var fromPath = GetTempFilePathWithExt(".crz3", "SyncVolumeEquations_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "SyncVolumeEquations_Add_toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Processing = SyncFlags.Insert,
            };

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var volEq = new VolumeEquation
            {
                CruiseID = cruiseID,
                Species = "sp1",
                PrimaryProduct = "01",
                VolumeEquationNumber = "something"
            };
            fromDb.Insert(volEq);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("VolumeEquation", "WHERE VolumeEquationNumber = @p1", volEq.VolumeEquationNumber).Should().Be(1);
        }

        [Fact]
        public void SyncTreeDefaultValues_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "SyncTreeDefaultValues_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "SyncTreeDefaultValues_Add_toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Processing = SyncFlags.Insert,
            };

            var init = new DatabaseInitializer()
            {
                TreeDefaults = null,
            };
            var cruiseID = init.CruiseID;
            var saleID = init.SaleID;
            using var fromDb = init.CreateDatabaseFile(fromPath);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var tdv = new TreeDefaultValue
            {
                CruiseID = cruiseID,
                SpeciesCode = "sp1",
                PrimaryProduct = "01",
            };
            fromDb.Insert(tdv);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.From<TreeDefaultValue>().Count().Should().Be(1);
        }

        [Fact]
        public void SyncStratumTemplates_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "SyncStratumTemplates_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "SyncStratumTemplates_Add_toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Processing = SyncFlags.Insert,
            };

            var init = new DatabaseInitializer();
            var cruiseID = init.CruiseID;
            var saleID = init.SaleID;
            using var fromDb = init.CreateDatabaseFile(fromPath);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var st = new StratumTemplate
            {
                CruiseID = cruiseID,
                StratumTemplateName = "something",
            };
            fromDb.Insert(st);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.From<StratumTemplate>().Count().Should().Be(1);
        }

        [Fact]
        public void SyncStratumTemplateTreeFieldSetup_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "SyncStratumTemplateTreeFieldSetup_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "SyncStratumTemplateTreeFieldSetup_Add_toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Processing = SyncFlags.Insert,
            };

            var init = new DatabaseInitializer();
            var cruiseID = init.CruiseID;
            var saleID = init.SaleID;
            using var fromDb = init.CreateDatabaseFile(fromPath);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var st = new StratumTemplate
            {
                CruiseID = cruiseID,
                StratumTemplateName = "something",
            };
            fromDb.Insert(st);

            var sttfs = new StratumTemplateTreeFieldSetup
            {
                CruiseID = cruiseID,
                StratumTemplateName = "something",
                Field = "DBH",
            };
            fromDb.Insert(sttfs);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.From<StratumTemplateTreeFieldSetup>().Count().Should().Be(1);
        }

        [Fact]
        public void SyncStratumTemplateLogFieldSetup_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "SyncStratumTemplateLogFieldSetup_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "SyncStratumTemplateLogFieldSetup_Add_toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Processing = SyncFlags.Insert,
            };

            var init = new DatabaseInitializer();
            var cruiseID = init.CruiseID;
            var saleID = init.SaleID;
            using var fromDb = init.CreateDatabaseFile(fromPath);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var st = new StratumTemplate
            {
                CruiseID = cruiseID,
                StratumTemplateName = "something",
            };
            fromDb.Insert(st);

            var sttfs = new StratumTemplateLogFieldSetup
            {
                CruiseID = cruiseID,
                StratumTemplateName = "something",
                Field = "Grade",
            };
            fromDb.Insert(sttfs);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.From<StratumTemplateLogFieldSetup>().Count().Should().Be(1);
        }
    }
}