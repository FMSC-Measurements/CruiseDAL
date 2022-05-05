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

            var syncOptions = new CruiseSyncOptions();

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

            var syncOptions = new CruiseSyncOptions();

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

            var treeMeasurment = AutoFaker.Generate<TreeMeasurment>();
            treeMeasurment.TreeID = treeID;
            treeMeasurment.TreeMeasurment_CN = null;
            treeMeasurment.Created_TS = DateTime.Now;
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

            var syncOptions = new CruiseSyncOptions();

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
        public void Sync_PlotTree_Add_WithMeasurment()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_PlotTree_Add_WithMeasurment_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_PlotTree_Add_WithMeasurment_toFile");

            var syncOptions = new CruiseSyncOptions();

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

            var treeMeasurment = AutoFaker.Generate<TreeMeasurment>();
            treeMeasurment.TreeID = treeID;
            treeMeasurment.TreeMeasurment_CN = null;
            treeMeasurment.Created_TS = DateTime.Now;
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
        public void Issue_ReconTreeMeasurmentsDontImport()
        {
            var fromPath = GetTestFile("Issue_ReconTreeMeasurmentsDontImport.crz3");
            var toPath = GetTempFilePathWithExt(".crz3", "Issue_ReconTreeMeasurmentsDontImport_toFile");

            var syncOptions = new CruiseSyncOptions();

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