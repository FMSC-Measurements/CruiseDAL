using CruiseDAL.DataObjects;
using CruiseDAL.V2.Models;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace CruiseDAL.V2.DataObjects
{
    public class StratumDO_Tests
    {
        [Fact]
        public void RecursiveDeleteStratum()
        {
            using (var database = new DAL())
            {
                var unit = new CuttingUnit()
                { Code = "u1" };
                database.Insert(unit);

                var stratum = new Stratum()
                { Code = "st1", Method = "STR" };
                database.Insert(stratum);

                var unitStratum = new CuttingUnitStratum()
                { CuttingUnit_CN = unit.CuttingUnit_CN.Value, Stratum_CN = stratum.Stratum_CN.Value };
                database.Insert(unitStratum);

                var sampleGroup = new SampleGroup()
                { Stratum_CN = stratum.Stratum_CN.Value, Code = "sg1", CutLeave = "C", UOM="01", PrimaryProduct="01" };
                database.Insert(sampleGroup);

                var samplerState = new SamplerState()
                { SampleGroup_CN = sampleGroup.SampleGroup_CN.Value };
                database.Insert(samplerState);

                var countTree = new CountTree()
                { CuttingUnit_CN = unit.CuttingUnit_CN.Value, SampleGroup_CN = sampleGroup.SampleGroup_CN.Value };
                database.Insert(countTree);

                var treeEstimate = new TreeEstimate()
                { CountTree_CN = countTree.CountTree_CN };
                database.Insert(countTree);

                var stratumStats = new StratumStats()
                { Stratum_CN = stratum.Stratum_CN.Value, Code = "ss1" };
                database.Insert(stratumStats);

                var sampleGroupStats = new SampleGroupStats()
                { StratumStats_CN = stratumStats.StratumStats_CN.Value, Code = "sgs1" };
                database.Insert(sampleGroupStats);

                var stDO = database.Query<StratumDO>("SELECT * FROM Stratum;").First();
                stDO.Should().NotBeNull();

                var plot = new Plot()
                {
                    CuttingUnit_CN = unit.CuttingUnit_CN.Value,
                    Stratum_CN = stratum.Stratum_CN.Value,
                    PlotNumber = 1,
                };
                database.Insert(plot);

                var tree = new Tree()
                { CuttingUnit_CN = unit.CuttingUnit_CN.Value,
                    Stratum_CN = stratum.Stratum_CN.Value,
                    SampleGroup_CN = sampleGroup.SampleGroup_CN.Value,
                    Plot_CN = plot.Plot_CN.Value,
                    TreeNumber = 1,
                    Species = "1",
                };
                database.Insert(tree);

                var log = new Log()
                {
                    Tree_CN = tree.Tree_CN.Value,
                    LogNumber = "1",
                };
                database.Insert(log);

                database.HasForeignKeyErrors().Should().BeFalse();

                StratumDO.RecursiveDeleteStratum(stDO);

                stDO.Invoking(x => x.Delete()).Should().NotThrow();

                database.GetRowCount("Stratum", "WHERE Stratum_CN = @p1", stratum.Stratum_CN)
                    .Should().Be(0);
                database.GetRowCount("Plot", "WHERE Plot_CN = @p1", plot.Plot_CN)
                    .Should().Be(0);
                database.GetRowCount("Tree", "WHERE Tree_CN = @p1", tree.Tree_CN)
                    .Should().Be(0);
                database.GetRowCount("Log", "WHERE Log_CN = @p1", log.Log_CN)
                    .Should().Be(0);
                database.GetRowCount("StratumStats", "WHERE StratumStats_CN = @p1", stratumStats.StratumStats_CN)
                    .Should().Be(0);
                database.GetRowCount("SampleGroupStats", "WHERE SampleGroupStats_CN = @p1", sampleGroupStats.SampleGroupStats_CN)
                    .Should().Be(0);

                var stuff = database.QueryGeneric("PRAGMA FOREIGN_KEY_CHECK");

                //database.Execute($"DELETE FROM SamplerState WHERE SampleGroup_CN IN (SELECT SampleGroup_CN FROM SampleGroup WHERE SampleGroup.Stratum_CN = {stDO.Stratum_CN});");

                //stuff = database.QueryGeneric("PRAGMA FOREIGN_KEY_CHECK");

                database.HasForeignKeyErrors().Should().BeFalse();
            }
        }
    }
}