﻿using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using CruiseDAL.V3.Sync.Util;


namespace CruiseDAL.V3.Sync
{
    public class ConflictChecker
    {
        // downstream conflicts:
        // downstream conflicts are conflicts that will need to be resolved if a conflict is resolved using the
        // chose resolution option. The chose resolution option may require additional conflict resolution
        // if child data has conflicting natural keys. 

        // we don't need to worry about more one level down from a parent record with downstream conflicts
        // at least with cruise data when we go more than 
        // e.g. trees with-in plots or logs with-in trees
        // if there is a nested conflict on a plot. we don't need to worry about conflicts on the trees within the plot
        // if the user resolves with a modify resolution, then the trees wont be in conflict
        // if the user resolves with a chose resolution, then we want to take all the trees from the chosen file. I can't think of any situation
        //      where we would want to mix tree from two files on a plot. this is sorta using the 'All or Nothing' resolution option


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
                    var treeConflicts = CheckTreesByUnitCode(source, destination, cruiseID, unitCode).ToArray();
                    var plotConflicts = CheckPlotsByUnitCode(source, destination, cruiseID, unitCode).ToArray();
                    var downstreamConflicts = treeConflicts.Concat(plotConflicts).ToArray();



                    yield return new Conflict
                    {
                        Table = nameof(CuttingUnit),
                        Identity = Identify(item),
                        SourctRecID = item.CuttingUnitID,
                        DestRecID = conflictItem.CuttingUnitID,
                        SourceRec = item,
                        DestRec = conflictItem,
                        DownstreamConflicts = downstreamConflicts,
                        SourceMod = DateMath.Max(item.Created_TS.Value, item.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
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
                        Identity = Identify(item),
                        SourctRecID = item.StratumID,
                        DestRecID = conflictItem.StratumID,
                        SourceRec = item,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(item.Created_TS.Value, item.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
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
                        Identity = Identify(sg),
                        SourctRecID = sg.SampleGroupID,
                        DestRecID = conflictItem.SampleGroupID,
                        SourceRec = sg,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(sg.Created_TS.Value, sg.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
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
                    var treeConflicts = CheckPlotTreesByUnitCodePlotNumber(source, destination, cruiseID, plot.CuttingUnitCode, plot.PlotNumber).ToArray();

                    yield return new Conflict
                    {
                        Table = nameof(Plot),
                        Identity = Identify(plot),
                        SourctRecID = plot.PlotID,
                        DestRecID = conflictItem.PlotID,
                        SourceRec = plot,
                        DestRec = conflictItem,
                        DownstreamConflicts = treeConflicts,
                        SourceMod = DateMath.Max(plot.Created_TS.Value, plot.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                    };
                }
            }
        }

        protected IEnumerable<Conflict> CheckPlotsByUnitCode(DbConnection source, DbConnection destination, string cruiseID, string unitCode)
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
                        Identity = Identify(plot),
                        SourctRecID = plot.PlotID,
                        DestRecID = conflictItem.PlotID,
                        SourceRec = plot,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(plot.Created_TS.Value, plot.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
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
                        Identity = Identify(tree),
                        SourctRecID = tree.TreeID,
                        DestRecID = conflictItem.TreeID,
                        SourceRec = tree,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(tree.Created_TS.Value, tree.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                    };
                }
            }
        }

        protected IEnumerable<Conflict> CheckTreesByUnitCode(DbConnection source, DbConnection destination, string cruiseID, string cuttingUnitCode)
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
                        Identity = Identify(tree),
                        SourctRecID = tree.TreeID,
                        DestRecID = conflictItem.TreeID,
                        SourceRec = tree,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(tree.Created_TS.Value, tree.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                    };
                }
            }
        }

        protected IEnumerable<Conflict> CheckPlotTreesByUnitCode(DbConnection source, DbConnection destination, string cruiseID, string cuttingUnitCode)
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
                        Identity = Identify(tree),
                        SourctRecID = tree.TreeID,
                        DestRecID = conflictItem.TreeID,
                        SourceRec = tree,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(tree.Created_TS.Value, tree.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                    };
                }
            }
        }

        protected IEnumerable<Conflict> CheckPlotTreesByUnitCodePlotNumber(DbConnection source, DbConnection destination, string cruiseID, string cuttingUnitCode, int plotNumber)
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
                        Identity = Identify(tree),
                        SourctRecID = tree.TreeID,
                        DestRecID = conflictItem.TreeID,
                        SourceRec = tree,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(tree.Created_TS.Value, tree.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
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
                        Identity = Identify(tree),
                        SourctRecID = tree.TreeID,
                        DestRecID = conflictItem.TreeID,
                        SourceRec = tree,
                        DestRec = conflictItem,
                        SourceMod = tree.Created_TS.Value.Max(tree.Modified_TS),
                        DestMod = conflictItem.Created_TS.Value.Max(conflictItem.Modified_TS),
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
                        Identity = Identify(item),
                        SourctRecID = item.LogID,
                        DestRecID = conflictItem.LogID,
                        SourceRec = item,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(item.Created_TS.Value, item.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckLogsByUnitCodeTreeNumber(DbConnection source, DbConnection destination, string cruiseID, string cuttingUnitCode, int treeNumber)
        {
            
            var sourceItems = source.From<Log>()
                .Join("Tree", "USING (TreeID)")
                .Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND TreeNumber = @p3 AND PlotNumber IS NULL")
                .Query(cruiseID, cuttingUnitCode, treeNumber);

            foreach (var item in sourceItems)
            {
                var conflictItem = destination.From<Log>()
                    .Join("Tree", "USING (TreeID)")
                    .Where("CruiseID = @p1 AND LogNumber = @p2 AND CuttingUnitCode = @p3 AND TreeNumber = @p4  AND LogID != @LogID")
                    .Query(cruiseID, item.LogNumber, cuttingUnitCode, treeNumber).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Log),
                        Identity = Identify(item),
                        SourctRecID = item.LogID,
                        SourceRec = item,
                        DestRec = conflictItem,
                        DestRecID = conflictItem.LogID,
                        SourceMod = DateMath.Max(item.Created_TS.Value, item.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckLogsByUnitCodePlotTreeNumber(DbConnection source, DbConnection destination, string cruiseID, string cuttingUnitCode, int treeNumber, int plotNumber)
        {

            var sourceItems = source.From<Log>()
                .Join("Tree", "USING (TreeID)")
                .Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND TreeNumber = @p3 AND PlotNumber = @p4")
                .Query(cruiseID, cuttingUnitCode, treeNumber, plotNumber);

            foreach (var item in sourceItems)
            {
                var conflictItem = destination.From<Log>()
                    .Join("Tree", "USING (TreeID)")
                    .Where("CruiseID = @p1 AND LogNumber = @p2 AND CuttingUnitCode = @p3 AND TreeNumber = @p4 AND PlotNumber = @p5  AND LogID != @p6")
                    .Query(cruiseID, item.LogNumber, cuttingUnitCode, treeNumber, plotNumber, item.LogID).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Log),
                        Identity = Identify(item),
                        SourctRecID = item.LogID,
                        DestRecID = conflictItem.LogID,
                        SourceRec = item,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(item.Created_TS.Value, item.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                    };
                }
            }
        }

        protected string Identify(CuttingUnit cu)
        {
            return $"Unit: {cu.CuttingUnitCode}";
        }

        protected string Identify(Stratum st)
        {
            return $"Stratum: {st.StratumCode}";
        }

        protected string Identify(SampleGroup sg)
        {
            return $"Sample Group: {sg.SampleGroupCode}, Stratum:{sg.StratumCode}";
        }

        protected string Identify(Plot p)
        {
            return $"Plot: {p.PlotNumber}, Unit: {p.CuttingUnitCode}";
        }

        protected string Identify(Tree t)
        {
            if (t.PlotNumber.HasValue)
            {
                return $"Tree: {t.TreeNumber}, Plot: {t.PlotNumber.Value}, Unit: {t.CuttingUnitCode}";
            }
            else
            {
                return $"Tree: {t.TreeNumber}, Unit: {t.CuttingUnitCode}";
            }
        }

        protected string Identify(Log l)
        {
            return $"Log: {l.LogNumber}";
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