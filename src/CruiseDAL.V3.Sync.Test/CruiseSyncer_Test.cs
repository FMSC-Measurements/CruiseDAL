using AutoBogus;
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
        public void Sync_Sale_Add()
        {
            var fromPath = base.GetTempFilePath(".crz3", "Sale_Add_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Sale_Add_toFile");

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
            var fromPath = base.GetTempFilePath(".crz3", "Sale_Update_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Sale_Update_toFile");

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
            var fromPath = base.GetTempFilePath(".crz3", "Cruise_Add_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Cruise_Add_toFile");

            var syncOptions = new CruiseSyncOptions();

            var cruiseID = CruiseID;
            var saleID = SaleID;

            using var fromDb = CreateDatabaseFile(fromPath);

            var cruise = fromDb.From<Cruise>()
                .Where("CruiseID = @p1")
                .Query(cruiseID)
                .FirstOrDefault();

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var cruiseAgain = toDb.From<Cruise>()
                .Where("CruiseID = @p1")
                .Query(cruiseID).FirstOrDefault();

            cruiseAgain.Should().BeEquivalentTo(cruise, x => x.Excluding(y => y.Modified_TS));
        }

        [Fact]
        public void Sync_Cruise_Update()
        {
            var fromPath = base.GetTempFilePath(".crz3", "Cruise_Update_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Cruise_Update_toFile");

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
            var fromPath = base.GetTempFilePath(".crz3", "fromFile");
            var toPath = base.GetTempFilePath(".crz3", "toFile");

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
        public void Sync_Unit_Add()
        {
            var fromPath = base.GetTempFilePath(".crz3", "Sync_Unit_Add_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Sync_Unit_Add_toFile");

            var syncOptions = new CruiseSyncOptions();

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

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

            var syncOptions = new CruiseSyncOptions();

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

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
        public void Sync_Stratum_Add()
        {
            var fromPath = base.GetTempFilePath(".crz3", "Stratum_Add_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Stratum_Add_toFile");

            var syncOptions = new CruiseSyncOptions();

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var stratumID = Guid.NewGuid().ToString();
            fromDb.Insert(new Stratum()
            {
                CruiseID = cruiseID,
                StratumID = stratumID,
                StratumCode = "10",
                Method = "100",
            });
            var newStratum = fromDb.From<Stratum>().Where("StratumID = @p1").Query(stratumID).Single();

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var stratumAgain = toDb.From<Stratum>().Where("StratumID =  @p1")
                .Query(stratumID).FirstOrDefault();
            stratumAgain.Should().BeEquivalentTo(newStratum, x => x.Excluding(y => y.Modified_TS));
        }

        [Fact]
        public void Sync_Stratum_Update()
        {
            var fromPath = base.GetTempFilePath(".crz3", "Stratum_Updated_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Stratum_Update_toFile");

            var syncOptions = new CruiseSyncOptions();

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            var stratumID = Guid.NewGuid().ToString();
            var stratum = new Stratum()
            {
                CruiseID = cruiseID,
                StratumID = stratumID,
                StratumCode = "10",
                Method = "100",
            };
            fromDb.Insert(stratum);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            stratum.Hotkey = Rand.String();
            fromDb.Update(stratum);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var stratumAgain = toDb.From<Stratum>().Where("StratumID =  @p1")
                .Query(stratumID).FirstOrDefault();
            stratumAgain.Should().BeEquivalentTo(stratum, x => x.Excluding(y => y.Modified_TS));
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
        public void Sync_SampleGroup_Add()
        {
            var fromPath = base.GetTempFilePath(".crz3", "SampleGroup_Add_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "SampleGroup_Add_toFile");

            var syncOptions = new CruiseSyncOptions();

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var sampleGroupID = Guid.NewGuid().ToString();
            fromDb.Insert(new SampleGroup()
            {
                CruiseID = cruiseID,
                SampleGroupID = sampleGroupID,
                SampleGroupCode = "10",
                StratumCode = Strata[0].StratumCode,
            });
            var newSampleGroup = fromDb.From<SampleGroup>().Where("SampleGroupID = @p1").Query(sampleGroupID).Single();

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var sampleGroupAgain = toDb.From<SampleGroup>().Where("SampleGroupID =  @p1")
                .Query(sampleGroupID).FirstOrDefault();
            sampleGroupAgain.Should().BeEquivalentTo(newSampleGroup);
        }

        [Fact]
        public void Sync_SampleGroup_Update()
        {
            var fromPath = base.GetTempFilePath(".crz3", "SampleGroup_Update_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "SampleGroup_Update_toFile");

            var syncOptions = new CruiseSyncOptions();

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            var sampleGroupID = Guid.NewGuid().ToString();
            var sampleGroup = new SampleGroup()
            {
                CruiseID = cruiseID,
                SampleGroupID = sampleGroupID,
                SampleGroupCode = "10",
                StratumCode = Strata[0].StratumCode,
            };
            fromDb.Insert(sampleGroup);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            sampleGroup.Description = Rand.String();
            fromDb.Update(sampleGroup);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var sampleGroupAgain = toDb.From<SampleGroup>().Where("SampleGroupID =  @p1")
                .Query(sampleGroupID).FirstOrDefault();
            sampleGroupAgain.Should().BeEquivalentTo(sampleGroup, x => x.Excluding(y => y.Modified_TS));
        }

        [Fact]
        public void Sync_SubPopulation_Add()
        {
            var fromPath = base.GetTempFilePath(".crz3", "SubPopulation_Add_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "SubPopulation_Add_toFile");

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
        public void Sync_Plot_Add()
        {
            var fromPath = base.GetTempFilePath(".crz3", "Sync_Plot_Add_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Sync_Plot_Add_toFile");

            var syncOptions = new CruiseSyncOptions()
            {
                Design = SyncFlags.Insert,
                FieldData = SyncFlags.Insert,
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

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            toDb.GetRowCount("Plot", "WHERE PlotID = @p1", newPlot.PlotID)
                .Should().Be(1);
        }

        [Fact]
        public void Sync_Plot_Update()
        {
            var fromPath = base.GetTempFilePath(".crz3", "Sync_Plot_Add_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Sync_Plot_Add_toFile");

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

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            plot.Remarks = Rand.String();
            plot.Slope = Rand.Double();
            plot.PlotNumber = Rand.Int();
            fromDb.Update(plot);

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var plotAgian = toDb
                .From<Plot>()
                .Where("PlotID = @p1")
                .Query(plot.PlotID)
                .FirstOrDefault();

            plotAgian.Should().BeEquivalentTo(plot, config => config.Excluding(x => x.Modified_TS));
        }

        [Fact]
        public void Sync_Plot_Stratum_Add()
        {
            var fromPath = base.GetTempFilePath(".crz3", "Sync_Plot_Add_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Sync_Plot_Add_toFile");

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
            var fromPath = base.GetTempFilePath(".crz3", "Sync_Plot_Add_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Sync_Plot_Add_toFile");

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
            var fromPath = base.GetTempFilePath(".crz3", "Sync_Plot_Add_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Sync_Plot_Add_toFile");

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
            var fromPath = base.GetTempFilePath(".crz3", "Sync_Plot_Add_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Sync_Plot_Add_toFile");

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
            var fromPath = base.GetTempFilePath(".crz3", "Sync_Unit_Add_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Sync_Unit_Add_toFile");

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
            var fromPath = base.GetTempFilePath(".crz3", "Sync_Unit_Add_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Sync_Unit_Add_toFile");

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
            var fromPath = base.GetTempFilePath(".crz3", "Sync_Plot_Add_fromFile");
            var toPath = base.GetTempFilePath(".crz3", "Sync_Plot_Add_toFile");

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
    }
}