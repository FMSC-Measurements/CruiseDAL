using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test.Schema.Views
{
    public class TreeError_Test : TestBase
    {
        public TreeError_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void TreeError_MeasureTree_No_TreeMeasurmentRecord()
        {
            var init = new DatabaseInitializer();

            using var db = init.CreateDatabase();

            var tree = new Tree
            {
                // just requited fields
                CruiseID = init.CruiseID,
                TreeID = Guid.NewGuid().ToString(),
                TreeNumber = 1,
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",

                CountOrMeasure = "M",
            };
            db.Insert(tree);

            var treeErrors = db.From<TreeError>().Query().ToArray();
            treeErrors.Should().Contain(x => x.Field == "heights");
            treeErrors.Should().Contain(x => x.Field == "diameters");

        }

        [Fact]
        public void TreeError_MeasureTree_WithHeights()
        {
            var init = new DatabaseInitializer();

            using var db = init.CreateDatabase();

            var tree = new Tree
            {
                // just requited fields
                CruiseID = init.CruiseID,
                TreeID = Guid.NewGuid().ToString(),
                TreeNumber = 1,
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",

                CountOrMeasure = "M",
            };
            db.Insert(tree);

            var tm = new TreeMeasurment
            {
                TreeID = tree.TreeID,
                TotalHeight = 101,
            };
            db.Insert(tm);

            var treeErrors = db.From<TreeError>().Query().ToArray();
            treeErrors.Should().NotContain(x => x.Field == "heights");
        }

        [Fact]
        public void TreeError_MeasureTree_NoValues()
        {
            var init = new DatabaseInitializer();

            using var db = init.CreateDatabase();

            var tree = new Tree
            {
                // just requited fields
                CruiseID = init.CruiseID,
                TreeID = Guid.NewGuid().ToString(),
                TreeNumber = 1,
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",

                CountOrMeasure = "M",
            };
            db.Insert(tree);

            var tm = new TreeMeasurment
            {
                TreeID = tree.TreeID,
            };
            db.Insert(tm);

            var treeErrors = db.From<TreeError>().Query().ToArray();
            treeErrors.Should().Contain(x => x.Field == "SpeciesCode");
            treeErrors.Should().Contain(x => x.Field == "LiveDead");
            treeErrors.Should().Contain(x => x.Field == "heights");
            treeErrors.Should().Contain(x => x.Field == "diameters");
        }

        [Fact]
        public void TreeError_MeasureTree_SeenDefectPrimary()
        {
            var init = new DatabaseInitializer();

            using var db = init.CreateDatabase();

            var tree = new Tree
            {
                // just requited fields
                CruiseID = init.CruiseID,
                TreeID = Guid.NewGuid().ToString(),
                TreeNumber = 1,
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",

                CountOrMeasure = "M",
            };
            db.Insert(tree);

            var tm = new TreeMeasurment
            {
                TreeID = tree.TreeID,
                RecoverablePrimary = 3,
                SeenDefectPrimary = 2,
            };
            db.Insert(tm);

            var treeErrors = db.From<TreeError>().Query().ToArray();
            treeErrors.Should().Contain(x => x.Field == "SeenDefectPrimary");

            tm.SeenDefectPrimary = 3;
            db.Update(tm);

            treeErrors = db.From<TreeError>().Query().ToArray();
            treeErrors.Should().NotContain(x => x.Field == "SeenDefectPrimary");


        }
    }
}
