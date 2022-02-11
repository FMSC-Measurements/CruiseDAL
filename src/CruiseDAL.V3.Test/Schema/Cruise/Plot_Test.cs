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

namespace CruiseDAL.V3.Test.Schema.Cruise
{
    public class Plot_Test : TestBase
    {
        public Plot_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void UpdatePlotNumberCascade()
        {
            var init = new DatabaseInitializer();
            using var db = init.CreateDatabase();

            var plot = new Plot
            {
                CruiseID = init.CruiseID,
                PlotID = Guid.NewGuid().ToString(),
                PlotNumber = 1,
                CuttingUnitCode = "u1",
            };
            db.Insert(plot);

            var plotStratum = new Plot_Stratum
            {
                CruiseID = init.CruiseID,
                PlotNumber = 1,
                StratumCode = "st1",
                CuttingUnitCode = "u1",
            };
            db.Insert(plotStratum);

            var tree = new Tree
            {
                CruiseID = init.CruiseID,
                TreeID = Guid.NewGuid().ToString(),
                TreeNumber = 1,
                CuttingUnitCode = "u1",
                PlotNumber = 1,
                StratumCode = "st1",
                SampleGroupCode = "sg1",
            };
            db.Insert(tree);

            var tl = new TallyLedger
            {
                CruiseID = init.CruiseID,
                TallyLedgerID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "u1",
                PlotNumber= 1,
                StratumCode = "st1",
                SampleGroupCode = "sg1",
                TreeID = tree.TreeID,
            };
            db.Insert(tl);

            plot.PlotNumber = 2;
            db.Update(plot);

            var tlAgain = db.From<TallyLedger>().Query().First();
            tlAgain.PlotNumber.Should().Be(plot.PlotNumber);

            var treeAgain = db.From<Tree>().Query().First();
            treeAgain.PlotNumber.Should().Be(plot.PlotNumber);
        }
    }
}
