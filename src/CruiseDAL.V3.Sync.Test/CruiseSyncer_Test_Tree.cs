using AutoBogus;
using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace CruiseDAL.V3.Sync
{
    public partial class CruiseSyncer_Test
    {
        [Fact]
        public void Sync_Tree_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Tree_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Tree_Add_toFile");

            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                Tree = SyncOption.Insert,
            };

            var init = new DatabaseInitializer();
            var cruiseID = init.CruiseID;
            var saleID = init.SaleID;

            // initialize source database
            using var fromDb = init.CreateDatabaseFile(fromPath);

            var treeID = Guid.NewGuid().ToString();
            var tree = new Tree()
            {
                CruiseID = cruiseID,
                TreeID = treeID,
                TreeNumber = 1,
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",
                SpeciesCode = "sp1",
                LiveDead = "L",
                CountOrMeasure = "M",
                CreatedBy = "test",
                Created_TS = DateTime.Now,
            };
            fromDb.Insert(tree);

            // create destination db
            using var toDb = new CruiseDatastore_V3(toPath, true);

            // run sync
            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var treeAgain = toDb.From<Tree>().Where("TreeID = @p1").Query(treeID).Single();
            treeAgain.Should().NotBeNull();
            treeAgain.Should().BeEquivalentTo(tree);
        }

        [Fact]
        public void Sync_Tree_Add_WithMeasurment()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_Tree_Add_WithMeasurment_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_Tree_Add_WithMeasurment_toFile");

            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                Tree = SyncOption.Insert,
                TreeMeasurment = SyncOption.Insert
            };

            var init = new DatabaseInitializer();
            var cruiseID = init.CruiseID;
            var saleID = init.SaleID;

            // initialize source database
            using var fromDb = init.CreateDatabaseFile(fromPath);

            var treeID = Guid.NewGuid().ToString();
            var tree = new Tree()
            {
                CruiseID = cruiseID,
                TreeID = treeID,
                TreeNumber = 1,
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",
                SpeciesCode = "sp1",
                LiveDead = "L",
                CountOrMeasure = "M",
                CreatedBy = "test",
                Created_TS = DateTime.Now,
            };
            fromDb.Insert(tree);

            var treeMeasurment = new TreeMeasurment()
            {
                TreeID = treeID,
                Aspect = 101,
                DBH = 10,
                ClearFace = "",
                CrownRatio = 1.0,
                DefectCode = "",
                DBHDoubleBarkThickness = 13.0,
                DiameterAtDefect = 13.1,
                DRC = 13.2,
                FormClass = 13.3,
                Grade = "",
                HeightToFirstLiveLimb = 13.4,
                HiddenPrimary = 13.5,
                IsFallBuckScale = false,
                MerchHeightPrimary = 13.6,
                MerchHeightSecondary = 13.7,
                PoleLength = 13.8,
                RecoverablePrimary = 13.9,
                SeenDefectPrimary = 14.0,
                SeenDefectSecondary = 14.1,
                Slope = 15,
                TopDIBPrimary = 15.1,
                TopDIBSecondary = 15.2,
                TotalHeight = 15.3,
                UpperStemDiameter = 15.4,
                UpperStemHeight = 15.5,
                VoidPercent = 15.6,
                Created_TS = DateTime.Now,
                CreatedBy = "me",
            };
            fromDb.Insert(treeMeasurment);

            // create destination db
            using var toDb = new CruiseDatastore_V3(toPath, true);

            // run sync
            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var treeAgain = toDb.From<Tree>().Where("TreeID = @p1").Query(treeID).Single();
            treeAgain.Should().NotBeNull();
            treeAgain.Should().BeEquivalentTo(tree);

            var treeMeasurmentAgain = toDb.From<TreeMeasurment>().Where("TreeID = @p1").Query(treeID).Single();
            treeMeasurmentAgain.Should().NotBeNull();
            treeMeasurmentAgain.Should().BeEquivalentTo(treeMeasurment);
        }

        [Fact]
        public void Sync_PlotTree_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Tree_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Tree_Add_toFile");

            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                Tree = SyncOption.Insert,
                Plot = SyncOption.Insert
            };

            var init = new DatabaseInitializer();
            var cruiseID = init.CruiseID;
            var saleID = init.SaleID;

            // initialize source database
            using var fromDb = init.CreateDatabaseFile(fromPath);

            var plotID = Guid.NewGuid().ToString();
            var plot = new Plot()
            {
                CruiseID = cruiseID,
                PlotID = plotID,
                PlotNumber = 1,
                CuttingUnitCode = "u1",
            };
            fromDb.Insert(plot);

            // create destination db
            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath, false);

            var treeID = Guid.NewGuid().ToString();
            var tree = new Tree()
            {
                CruiseID = cruiseID,
                TreeID = treeID,
                TreeNumber = 1,
                CuttingUnitCode = "u1",
                PlotNumber = 1,
                StratumCode = "st1",
                SampleGroupCode = "sg1",
                SpeciesCode = "sp1",
                LiveDead = "L",
                CountOrMeasure = "M",
                CreatedBy = "test",
                Created_TS = DateTime.Now,
            };
            fromDb.Insert(tree);

            

            // run sync
            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var treeAgain = toDb.From<Tree>().Where("TreeID = @p1").Query(treeID).Single();
            treeAgain.Should().NotBeNull();
            treeAgain.Should().BeEquivalentTo(tree);
        }

        [Fact]
        public void Sync_PlotTree_Add_WithMeasurment()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_PlotTree_Add_WithMeasurment_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_PlotTree_Add_WithMeasurment_toFile");

            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                Tree = SyncOption.Insert,
                Plot = SyncOption.Insert,
                TreeMeasurment = SyncOption.Insert,
            };

            var init = new DatabaseInitializer();
            var cruiseID = init.CruiseID;
            var saleID = init.SaleID;

            // initialize source database
            using var fromDb = init.CreateDatabaseFile(fromPath);

            var plotID = Guid.NewGuid().ToString();
            var plot = new Plot()
            {
                CruiseID = cruiseID,
                PlotID = plotID,
                PlotNumber = 1,
                CuttingUnitCode = "u1",
            };
            fromDb.Insert(plot);

            var treeID = Guid.NewGuid().ToString();
            var tree = new Tree()
            {
                CruiseID = cruiseID,
                TreeID = treeID,
                TreeNumber = 1,
                CuttingUnitCode = "u1",
                PlotNumber = 1,
                StratumCode = "st1",
                SampleGroupCode = "sg1",
                SpeciesCode = "sp1",
                LiveDead = "L",
                CountOrMeasure = "M",
                CreatedBy = "test",
                Created_TS = DateTime.Now,
            };
            fromDb.Insert(tree);

            var treeMeasurment = new TreeMeasurment()
            {
                TreeID = treeID,
                Aspect = 101,
                DBH = 10,
                ClearFace = "",
                CrownRatio = 1.0,
                DefectCode = "",
                DBHDoubleBarkThickness = 13.0,
                DiameterAtDefect = 13.1,
                DRC = 13.2,
                FormClass = 13.3,
                Grade = "",
                HeightToFirstLiveLimb = 13.4,
                HiddenPrimary = 13.5,
                IsFallBuckScale = false,
                MerchHeightPrimary = 13.6,
                MerchHeightSecondary = 13.7,
                PoleLength = 13.8,
                RecoverablePrimary = 13.9,
                SeenDefectPrimary = 14.0,
                SeenDefectSecondary = 14.1,
                Slope = 15,
                TopDIBPrimary = 15.1,
                TopDIBSecondary = 15.2,
                TotalHeight = 15.3,
                UpperStemDiameter = 15.4,
                UpperStemHeight = 15.5,
                VoidPercent = 15.6,
                Created_TS = DateTime.Now,
                CreatedBy = "me",
            };
            fromDb.Insert(treeMeasurment);

            // create destination db
            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath, false);

            // run sync
            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var treeAgain = toDb.From<Tree>().Where("TreeID = @p1").Query(treeID).Single();
            treeAgain.Should().NotBeNull();
            treeAgain.Should().BeEquivalentTo(tree);

            var treeMeasurmentAgain = toDb.From<TreeMeasurment>().Where("TreeID = @p1").Query(treeID).Single();
            treeMeasurmentAgain.Should().NotBeNull();
            treeMeasurmentAgain.Should().BeEquivalentTo(treeMeasurment);
        }

        [Fact]
        public void Issue_ReconTreeMeasurmentsDontImport()
        {
            var fromPath = GetTestFile("Issue_ReconTreeMeasurmentsDontImport.crz3");
            var toPath = GetTempFilePathWithExt(".crz3", "Issue_ReconTreeMeasurmentsDontImport_toFile");

            var syncOptions = new TableSyncOptions(SyncOption.Lock)
            {
                Tree = SyncOption.Insert,
                TreeMeasurment = SyncOption.Insert,
            };

            // initialize source database
            using var fromDb = new CruiseDatastore_V3(fromPath);

            var cruiseID = fromDb.From<Cruise>().Query().Single().CruiseID;

            // create destination db
            using var toDb = new CruiseDatastore_V3(toPath, true);

            // run sync
            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var treeMeasurments = toDb.From<TreeMeasurment>().Query().ToArray();
            treeMeasurments.Should().NotBeEmpty();
        }
    }
}