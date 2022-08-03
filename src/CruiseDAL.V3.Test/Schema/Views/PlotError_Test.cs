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
    public class PlotError_Test : TestBase
    {
        public PlotError_Test(ITestOutputHelper output) : base(output)
        {
        }


        [Fact]
        public void PlotError_null_with_trees()
        {
            var init = new DatabaseInitializer();

            using var db = init.CreateDatabase();

            var plot = new Plot
            {
                CruiseID = init.CruiseID,
                PlotID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "u1",
                PlotNumber = 1,

            };
            db.Insert(plot);

            var ps = new Plot_Stratum
            {
                CruiseID = init.CruiseID,
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                PlotNumber = 1,
                IsEmpty = true
            };
            db.Insert(ps);

            var tree = new Tree
            {
                // just requited fields
                CruiseID = init.CruiseID,
                TreeID = Guid.NewGuid().ToString(),
                TreeNumber = 1,
                CuttingUnitCode = "u1",
                PlotNumber = 1,
                StratumCode = "st1",
                SampleGroupCode = "sg1",

                CountOrMeasure = "M",
            };
            db.Insert(tree);

            var plotErrors = db.From<PlotError>().Query().ToArray();
            plotErrors.Should().Contain(x => x.Message.Contains("contains trees but is marked as null plot"));
        }

        [Fact]
        public void PlotError_nonNull_no_trees()
        {
            var init = new DatabaseInitializer();

            using var db = init.CreateDatabase();

            var plot = new Plot
            {
                CruiseID = init.CruiseID,
                PlotID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "u1",
                PlotNumber = 1,

            };
            db.Insert(plot);

            var ps = new Plot_Stratum
            {
                CruiseID = init.CruiseID,
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                PlotNumber = 1,
                IsEmpty = false
            };
            db.Insert(ps);

            var plotErrors = db.From<PlotError>().Query().ToArray();
            plotErrors.Should().Contain(x => x.Message.Contains("contains no trees but is not marked as null plot"));
        }

        [Fact]
        public void PlotError_no_plotStratum()
        {
            var init = new DatabaseInitializer();

            using var db = init.CreateDatabase();

            var plot = new Plot
            {
                CruiseID = init.CruiseID,
                PlotID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "u1",
                PlotNumber = 1,

            };
            db.Insert(plot);

            var plotErrors = db.From<PlotError>().Query().ToArray();
            plotErrors.Should().Contain(x => x.Message.Contains("no strata in plot"));
        }
    }
}
