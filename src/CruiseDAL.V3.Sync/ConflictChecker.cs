using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync
{
    public class ConflictChecker
    {
        public IEnumerable<Conflict> CheckCuttingUnits(DbConnection source, DbConnection destination, string cruiseID)
        {
            var sourceItems = source.From<CuttingUnit>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var item in sourceItems)
            {
                var conflictItem = destination.From<CuttingUnit>()
                    .Where("CruiseID = @CruiseID AND CuttingUnitCode = @CuttingUnitCode AND CuttingUnitID != @CuttingUnitID")
                    .Query2(item).FirstOrDefault();

                if (conflictItem != null)
                {
                    var unitCode = item.CuttingUnitCode;
                    var treeConflicts = CheckTreesByUnitCode(source, destination, cruiseID, unitCode);
                    var plotTreeConflicts = CheckPlotTreesByUnitCode(source, destination, cruiseID, unitCode);
                    var plotConflicts = CheckPlot

                    yield return new Conflict
                    {
                        Table = nameof(CuttingUnit),
                        SourctRecID = item.CuttingUnitID,
                        DestRecID = conflictItem.CuttingUnitID,
                        SourceRec = item,
                        DestRec = conflictItem,
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckStrata(DbConnection source, DbConnection destination, string cruiseID)
        {
            var sourceItems = source.From<Stratum>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var item in sourceItems)
            {
                var conflictItem = destination.From<Stratum>()
                    .Where("CruiseID = @CruiseID AND StratumCode = @StratumCode AND StratumID != @StratumID")
                    .Query2(item).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Stratum),
                        SourctRecID = item.StratumID,
                        DestRecID = conflictItem.StratumID,
                        SourceRec = item,
                        DestRec = conflictItem,
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckSampleGroups(DbConnection source, DbConnection destination, string cruiseID)
        {
            var sourceItems = source.Query<SampleGroup, StratumKey>(
                "SELECT sg.*, st.StratumID FROM SampleGroup AS sg " +
                "JOIN Stratum AS st USING (CruiseID, StratumCode) " +
                "WHERE CruiseID = @p1;",
                paramaters: new[] { cruiseID });

            foreach (var (sg, st) in sourceItems)
            {
                var conflictItem = destination.From<SampleGroup>()
                    .Join("Stratum AS st", "USING (CruiseID, StratumCode)")
                    .Where("CruiseID = @p1 AND SampleGroupCode = @p2 AND st.StratumID = @p3 AND SampleGroupID != @p4")
                    .Query(cruiseID, sg.SampleGroupCode, st.StratumID, sg.SampleGroupID).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(SampleGroup),
                        SourctRecID = sg.SampleGroupID,
                        DestRecID = conflictItem.SampleGroupID,
                        SourceRec = sg,
                        DestRec = conflictItem,
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckPlots(DbConnection source, DbConnection destination, string cruiseID)
        {
            //var sourceItems = source.From<Plot>().Where("CruiseID = @p1").Query(cruiseID);

            var sourceItems = source.Query<Plot, CuttingUnitKey>(
                "SELECT p.*, cu.CuttingUnitID FROM Plot AS p " +
                "JOIN CuttingUnit AS cu USING (CruiseID, CuttingUnitCode) " +
                "WHERE CruiseID = @p1;",
                paramaters: new[] { cruiseID });

            foreach (var (plot, unit) in sourceItems)
            {
                var conflictItem = destination.From<Plot>()
                    .Join("CuttingUnit AS cu", "USING (CruiseID, CuttingUnitCode)")
                    .Where("CruiseID = @p1  AND PlotNumber = @p2 AND CuttingUnitID = @p3  AND PlotID != @p4")
                    .Query(cruiseID, plot.PlotNumber, unit.CuttingUnitID, plot.PlotID).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Plot),
                        SourctRecID = plot.PlotID,
                        DestRecID = conflictItem.PlotID,
                        SourceRec = plot,
                        DestRec = conflictItem,
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckPlotsByUnitCode(DbConnection source, DbConnection destination, string cruiseID, string unitCode)
        {
            //var sourceItems = source.From<Plot>().Where("CruiseID = @p1").Query(cruiseID);

            var sourceItems = source.Query<Plot>(
                "SELECT p.* FROM Plot AS p " +
                "WHERE CruiseID = @p1 AND CuttingUnitCode = @p2;",
                paramaters: new[] { cruiseID, unitCode });

            foreach (var plot in sourceItems)
            {
                var conflictItem = destination.From<Plot>()
                    .Where("CruiseID = @p1  AND PlotNumber = @p2 AND CuttingUnitCode = @p3  AND PlotID != @p4")
                    .Query(cruiseID, plot.PlotNumber, plot.CuttingUnitCode, plot.PlotID).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Plot),
                        SourctRecID = plot.PlotID,
                        DestRecID = conflictItem.PlotID,
                        SourceRec = plot,
                        DestRec = conflictItem,
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckTrees(DbConnection source, DbConnection destination, string cruiseID)
        {
            //var sourceItems = source.From<Tree>().Where("CruiseID = @p1").Query(cruiseID);
            var sourceItems = source.Query<Tree, CuttingUnitKey>(
                "SELECT t.*, cu.CuttingUnitID FROM Tree AS t " +
                "JOIN CuttingUnit AS cu USING (CruiseID, CuttingUnitCode) " +
                "WHERE CruiseID = @p1" +
                "   AND PlotNumber IS NULL;", paramaters: new[] { cruiseID }).ToArray();

            foreach (var (tree, unit) in sourceItems)
            {
                var conflictItem = destination.From<Tree>()
                    .Join("CuttingUnit AS cu", "USING (CruiseID, CuttingUnitCode)")
                    .Where("CruiseID = @p1 AND TreeNumber = @p2 AND cu.CuttingUnitID = @p3 AND TreeID != @p4 AND PlotNumber IS NULL")
                    .Query(cruiseID, tree.TreeNumber, unit.CuttingUnitID, tree.TreeID).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Tree),
                        SourctRecID = tree.TreeID,
                        DestRecID = conflictItem.TreeID,
                        SourceRec = tree,
                        DestRec = conflictItem,
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckTreesByUnitCode(DbConnection source, DbConnection destination, string cruiseID, string cuttingUnitCode)
        {
            var sourceItems = source.Query<Tree>(
                "SELECT t.* FROM Tree AS t " +
                "WHERE CruiseID = @p1 AND CuttingUnitCode = @p2" +
                "   AND PlotNumber IS NULL;", paramaters: new[] { cruiseID, cuttingUnitCode }).ToArray();

            foreach (var tree in sourceItems)
            {
                var conflictItem = destination.From<Tree>()
                    .Where("CruiseID = @p1 AND TreeNumber = @p2 AND CuttingUnitCode = @p3 AND TreeID != @p4 AND PlotNumber IS NULL")
                    .Query(cruiseID, tree.TreeNumber, cuttingUnitCode, tree.TreeID).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Tree),
                        SourctRecID = tree.TreeID,
                        DestRecID = conflictItem.TreeID,
                        SourceRec = tree,
                        DestRec = conflictItem,
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckPlotTreesByUnitCode(DbConnection source, DbConnection destination, string cruiseID, string cuttingUnitCode)
        {
            var sourceItems = source.Query<Tree>(
                "SELECT t.* FROM Tree AS t " +
                "WHERE CruiseID = @p1 AND CuttingUnitCode = @p2" +
                "   AND PlotNumber IS NOT NULL;", paramaters: new[] { cruiseID, cuttingUnitCode }).ToArray();

            foreach (var tree in sourceItems)
            {
                var conflictItem = destination.From<Tree>()
                    .Where("CruiseID = @p1 AND TreeNumber = @p2 AND CuttingUnitCode = @p3 AND TreeID != @p4 AND PlotNumber = @p5")
                    .Query(cruiseID, tree.TreeNumber, cuttingUnitCode, tree.TreeID, tree.PlotNumber).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Tree),
                        SourctRecID = tree.TreeID,
                        DestRecID = conflictItem.TreeID,
                        SourceRec = tree,
                        DestRec = conflictItem,
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckPlotTreesByUnitCodePlotNumber(DbConnection source, DbConnection destination, string cruiseID, string cuttingUnitCode, int plotNumber)
        {
            var sourceItems = source.Query<Tree>(
                "SELECT t.* FROM Tree AS t " +
                "WHERE CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber = @p3" +
                "   AND PlotNumber IS NOT NULL;", paramaters: new object[] { cruiseID, cuttingUnitCode, plotNumber }).ToArray();

            foreach (var tree in sourceItems)
            {
                var conflictItem = destination.From<Tree>()
                    .Where("CruiseID = @p1 AND TreeNumber = @p2 AND CuttingUnitCode = @p3 AND TreeID != @p4 AND PlotNumber = @p5")
                    .Query(cruiseID, tree.TreeNumber, cuttingUnitCode, tree.TreeID, tree.PlotNumber).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Tree),
                        SourctRecID = tree.TreeID,
                        DestRecID = conflictItem.TreeID,
                        SourceRec = tree,
                        DestRec = conflictItem,
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckPlotTrees(DbConnection source, DbConnection destination, string cruiseID)
        {
            //var sourceItems = source.From<Tree>().Where("CruiseID = @p1").Query(cruiseID);
            var sourceItems = source.Query<Tree, UnitPlotKey>(
                "SELECT t.*, cu.CuttingUnitID FROM Tree AS t " +
                "JOIN CuttingUnit AS cu USING (CruiseID, CuttingUnitCode) " +
                "JOIN Plot AS p USING (CruiseID, PlotNumber) " +
                "WHERE CruiseID = @p1" +
                "   AND PlotNumber IS NOT NULL;", paramaters: new[] { cruiseID }).ToArray();

            foreach (var (tree, unitPlot) in sourceItems)
            {
                var conflictItem = destination.From<Tree>()
                    .Join("Plot AS p", "USING (CruiseID, PlotNumber)")
                    .Where("CruiseID = @p1 AND TreeNumber = @p2 AND p.PlotID = @p3 AND TreeID != @p4")
                    .Query(cruiseID, tree.TreeNumber, unitPlot.PlotID, tree.TreeID).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Tree),
                        SourctRecID = tree.TreeID,
                        DestRecID = conflictItem.TreeID,
                        SourceRec = tree,
                        DestRec = conflictItem,
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckLogs(DbConnection source, DbConnection destination, string cruiseID)
        {
            var sourceItems = source.From<Log>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var item in sourceItems)
            {
                var conflictItem = destination.From<Log>()
                    .Where("CruiseID = @CruiseID AND LogNumber = @LogNumber AND TreeID = @TreeID AND LogID != @LogID")
                    .Query2(item).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Log),
                        SourctRecID = item.LogID,
                        DestRecID = conflictItem.LogID,
                    };
                }
            }
        }

        private class CuttingUnitKey
        {
            public string CuttingUnitCode { get; set; }
            public string CuttingUnitID { get; set; }
        }

        private class StratumKey
        {
            public string StratumCode { get; set; }
            public string StratumID { get; set; }
        }

        private class SampleGroupKey
        {
            public string SampleGroupCode { get; set; }
            public string SampleGroupID { get; set; }
        }

        private class PlotKey
        {
            public int PlotNumber { get; set; }
            public string PlotID { get; set; }
        }

        private class UnitPlotKey
        {
            public string CuttingUnitCode { get; set; }
            public string CuttingUnitID { get; set; }
            public int PlotNumber { get; set; }
            public string PlotID { get; set; }
        }
    }
}