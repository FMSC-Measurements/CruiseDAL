using CruiseDAL.V3.Models;
using CruiseDAL.V3.Sync.Util;
using FMSC.ORM.Core;
using FMSC.ORM.SQLite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

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

        protected Dictionary<string, string> DeviceNameLookup { get; set; }

        public ConflictResolutionOptions CheckConflicts(SQLiteDatastore sourceDb, SQLiteDatastore destDb, string cruiseID, ConflictCheckOptions options)
        {
            var source = sourceDb.OpenConnection();
            var dest = destDb.OpenConnection();
            try
            {
                return CheckConflicts(source, dest, cruiseID, options);
            }
            finally
            {
                sourceDb.ReleaseConnection();
                destDb.ReleaseConnection();
            }
        }

        public ConflictResolutionOptions CheckConflicts(DbConnection source, DbConnection destination, string cruiseID, ConflictCheckOptions options)
        {
            InitDeviceLookup(source, destination, cruiseID);

            return new ConflictResolutionOptions(
                CheckCuttingUnits(source, destination, cruiseID),
                CheckStrata(source, destination, cruiseID),
                CheckSampleGroups(source, destination, cruiseID),
                CheckPlots(source, destination, cruiseID),
                CheckTrees(source, destination, cruiseID),
                CheckPlotTrees(source, destination, cruiseID, options),
                CheckLogs(source, destination, cruiseID));
        }

        // create look up that can be used to translate device ids to device name
        // This makes it so we don't need to join with device table when running queries
        // TODO: need to look at how FScruiser behaves when user changes device name
        // how do we handle changing device name... do we need a modified_TS on the device table?
        private void InitDeviceLookup(DbConnection source, DbConnection destination, string cruiseID)
        {
            var deviceNameLookup = new Dictionary<string, string>();
            var srcDevices = source.From<CruiseDAL.V3.Models.Device>().Where("CruiseID = @p1").Query(cruiseID);
            var destDevices = destination.From<CruiseDAL.V3.Models.Device>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var d in srcDevices.Concat(destDevices))
            {
                if (deviceNameLookup.ContainsKey(d.DeviceID)) { continue; }
                deviceNameLookup.Add(d.DeviceID, d.Name);
            }

            DeviceNameLookup = deviceNameLookup;
        }

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
                        SourceRecID = item.CuttingUnitID,
                        DestRecID = conflictItem.CuttingUnitID,
                        SourceRec = item,
                        DestRec = conflictItem,
                        DownstreamConflicts = downstreamConflicts,
                        SourceMod = DateMath.Max(item.Created_TS.Value, item.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                        SrcDeviceName = DeviceNameLookup.GetValueOrDefault(item.CreatedBy),
                        DestDeviceName = DeviceNameLookup.GetValueOrDefault(conflictItem.CreatedBy),
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
                    var sgConflicts = CheckSampleGroupsByStratumCode(source, destination, item.StratumCode, cruiseID).ToArray();

                    yield return new Conflict
                    {
                        Table = nameof(Stratum),
                        Identity = Identify(item),
                        SourceRecID = item.StratumID,
                        DestRecID = conflictItem.StratumID,
                        SourceRec = item,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(item.Created_TS.Value, item.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                        DownstreamConflicts = sgConflicts,
                        SrcDeviceName = DeviceNameLookup.GetValueOrDefault(item.CreatedBy),
                        DestDeviceName = DeviceNameLookup.GetValueOrDefault(conflictItem.CreatedBy),
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
                        SourceRecID = sg.SampleGroupID,
                        DestRecID = conflictItem.SampleGroupID,
                        SourceRec = sg,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(sg.Created_TS.Value, sg.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                        SrcDeviceName = DeviceNameLookup.GetValueOrDefault(sg.CreatedBy),
                        DestDeviceName = DeviceNameLookup.GetValueOrDefault(conflictItem.CreatedBy),
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckSampleGroupsByStratumCode(DbConnection source, DbConnection destination, string stratumCode, string cruiseID)
        {
            var sourceItems = source.Query<SampleGroup>(
                "SELECT sg.* FROM SampleGroup AS sg " +
                "WHERE CruiseID = @p1 AND StratumCode = @p2;",
                paramaters: new[] { cruiseID, stratumCode });

            foreach (var sg in sourceItems)
            {
                var conflictItem = destination.From<SampleGroup>()
                    .Where("CruiseID = @p1 AND SampleGroupCode = @p2 AND SampleGroup.StratumCode = @p3 AND SampleGroupID != @p4")
                    .Query(cruiseID, sg.SampleGroupCode, stratumCode, sg.SampleGroupID).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(SampleGroup),
                        Identity = Identify(sg),
                        SourceRecID = sg.SampleGroupID,
                        DestRecID = conflictItem.SampleGroupID,
                        SourceRec = sg,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(sg.Created_TS.Value, sg.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                        SrcDeviceName = DeviceNameLookup.GetValueOrDefault(sg.CreatedBy),
                        DestDeviceName = DeviceNameLookup.GetValueOrDefault(conflictItem.CreatedBy),
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
                        SourceRecID = plot.PlotID,
                        DestRecID = conflictItem.PlotID,
                        SourceRec = plot,
                        DestRec = conflictItem,
                        DownstreamConflicts = treeConflicts,
                        SourceMod = DateMath.Max(plot.Created_TS.Value, plot.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                        SrcDeviceName = DeviceNameLookup.GetValueOrDefault(plot.CreatedBy),
                        DestDeviceName = DeviceNameLookup.GetValueOrDefault(conflictItem.CreatedBy),
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
                        SourceRecID = plot.PlotID,
                        DestRecID = conflictItem.PlotID,
                        SourceRec = plot,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(plot.Created_TS.Value, plot.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                        SrcDeviceName = DeviceNameLookup.GetValueOrDefault(plot.CreatedBy),
                        DestDeviceName = DeviceNameLookup.GetValueOrDefault(conflictItem.CreatedBy),
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
                    // We aren't checking trees for downstream log, because
                    //      logs are linked to trees by TreeID

                    yield return new Conflict
                    {
                        Table = nameof(Tree),
                        Identity = Identify(tree),
                        SourceRecID = tree.TreeID,
                        DestRecID = conflictItem.TreeID,
                        SourceRec = tree,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(tree.Created_TS.Value, tree.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                        SrcDeviceName = DeviceNameLookup.GetValueOrDefault(tree.CreatedBy),
                        DestDeviceName = DeviceNameLookup.GetValueOrDefault(conflictItem.CreatedBy),
                    };
                }
            }
        }

        // only check non-plot trees
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
                        SourceRecID = tree.TreeID,
                        DestRecID = conflictItem.TreeID,
                        SourceRec = tree,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(tree.Created_TS.Value, tree.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                        SrcDeviceName = DeviceNameLookup.GetValueOrDefault(tree.CreatedBy),
                        DestDeviceName = DeviceNameLookup.GetValueOrDefault(conflictItem.CreatedBy),
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
                        SourceRecID = tree.TreeID,
                        DestRecID = conflictItem.TreeID,
                        SourceRec = tree,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(tree.Created_TS.Value, tree.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                        SrcDeviceName = DeviceNameLookup.GetValueOrDefault(tree.CreatedBy),
                        DestDeviceName = DeviceNameLookup.GetValueOrDefault(conflictItem.CreatedBy),
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
                        SourceRecID = tree.TreeID,
                        DestRecID = conflictItem.TreeID,
                        SourceRec = tree,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(tree.Created_TS.Value, tree.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                        SrcDeviceName = DeviceNameLookup.GetValueOrDefault(tree.CreatedBy),
                        DestDeviceName = DeviceNameLookup.GetValueOrDefault(conflictItem.CreatedBy),
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckPlotTrees(DbConnection source, DbConnection destination, string cruiseID, ConflictCheckOptions options)
        {
            //var sourceItems = source.From<Tree>().Where("CruiseID = @p1").Query(cruiseID);
            var sourceItems = source.Query<Tree, UnitPlotKey>(
                "SELECT t.*, cu.CuttingUnitID, p.PlotID FROM Tree AS t " +
                "JOIN Plot AS p USING (CruiseID, PlotNumber, CuttingUnitCode) " +
                "JOIN CuttingUnit AS cu USING (CruiseID, CuttingUnitCode) " +
                "WHERE CruiseID = @p1" +
                "   AND PlotNumber IS NOT NULL;", paramaters: new[] { cruiseID }).ToArray();

            // when users are doing nested plots where each stratum numbers tree independently
            // we need to allow for multiple trees with the same tree number in separate strata
            //
            var conflictWhereOption = (options.AllowDuplicateTreeNumberForNestedStrata) ?
                "CruiseID = @CruiseID AND TreeNumber = @TreeNumber AND StratumCode = @StratumCode AND p.PlotID = @PlotID AND TreeID != @TreeID" :
                "CruiseID = @CruiseID AND TreeNumber = @TreeNumber AND p.PlotID = @PlotID AND TreeID != @TreeID";

            foreach (var (tree, unitPlot) in sourceItems)
            {
                var conflictItem = destination.From<Tree>()
                    .Join("Plot AS p", "USING (CruiseID, CuttingUnitCode, PlotNumber)")
                    .Where(conflictWhereOption)
                    .Query2(new
                    {
                        CruiseID = cruiseID,
                        tree.TreeNumber,
                        tree.TreeID,
                        tree.StratumCode,
                        unitPlot.PlotID,
                    }).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Tree),
                        Identity = Identify(tree),
                        SourceRecID = tree.TreeID,
                        DestRecID = conflictItem.TreeID,
                        SourceRec = tree,
                        DestRec = conflictItem,
                        SourceMod = tree.Created_TS.Value.Max(tree.Modified_TS),
                        DestMod = conflictItem.Created_TS.Value.Max(conflictItem.Modified_TS),
                        SrcDeviceName = DeviceNameLookup.GetValueOrDefault(tree.CreatedBy),
                        DestDeviceName = DeviceNameLookup.GetValueOrDefault(conflictItem.CreatedBy),
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckLogs(DbConnection source, DbConnection destination, string cruiseID)
        {
            // using custom model type LogEx because we want to be able to display unit, plot number and tree number to the user
            var sourceItems = source.From<LogEx>().Join("Tree", "USING (TreeID)").Where("Log.CruiseID = @p1").Query(cruiseID);

            foreach (var item in sourceItems)
            {
                var conflictItem = destination.From<LogEx>()
                    .Join("Tree", "USING (TreeID)")
                    .Where("Log.CruiseID = @CruiseID AND LogNumber = @LogNumber AND Log.TreeID = @TreeID AND LogID != @LogID")
                    .Query2(item).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Log),
                        Identity = Identify(item),
                        SourceRecID = item.LogID,
                        DestRecID = conflictItem.LogID,
                        SourceRec = item,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(item.Created_TS.Value, item.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                        SrcDeviceName = DeviceNameLookup.GetValueOrDefault(item.CreatedBy),
                        DestDeviceName = DeviceNameLookup.GetValueOrDefault(conflictItem.CreatedBy),
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckLogsByUnitCodeTreeNumber(DbConnection source, DbConnection destination, string cruiseID, string cuttingUnitCode, int treeNumber)
        {
            var sourceItems = source.From<LogEx>()
                .Join("Tree", "USING (TreeID)")
                .Where("Log.CruiseID = @p1 AND Tree.CuttingUnitCode = @p2 AND Tree.TreeNumber = @p3 AND Tree.PlotNumber IS NULL")
                .Query(cruiseID, cuttingUnitCode, treeNumber);

            foreach (var item in sourceItems)
            {
                var conflictItem = destination.From<LogEx>()
                    .Join("Tree", "USING (TreeID)")
                    .Where("Log.CruiseID = @p1 AND Log.LogNumber = @p2 AND Tree.CuttingUnitCode = @p3 AND Tree.TreeNumber = @p4  AND Log.LogID != @p5")
                    .Query(cruiseID, item.LogNumber, cuttingUnitCode, treeNumber, item.LogID).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Log),
                        Identity = Identify(item),
                        SourceRecID = item.LogID,
                        SourceRec = item,
                        DestRec = conflictItem,
                        DestRecID = conflictItem.LogID,
                        SourceMod = DateMath.Max(item.Created_TS.Value, item.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                        SrcDeviceName = DeviceNameLookup.GetValueOrDefault(item.CreatedBy),
                        DestDeviceName = DeviceNameLookup.GetValueOrDefault(conflictItem.CreatedBy),
                    };
                }
            }
        }

        public IEnumerable<Conflict> CheckLogsByUnitCodePlotTreeNumber(DbConnection source, DbConnection destination, string cruiseID, string cuttingUnitCode, int treeNumber, int plotNumber)
        {
            var sourceItems = source.From<LogEx>()
                .Join("Tree", "USING (TreeID)")
                .Where("Log.CruiseID = @p1 AND Tree.CuttingUnitCode = @p2 AND Tree.TreeNumber = @p3 AND Tree.PlotNumber = @p4")
                .Query(cruiseID, cuttingUnitCode, treeNumber, plotNumber);

            foreach (var item in sourceItems)
            {
                var conflictItem = destination.From<LogEx>()
                    .Join("Tree", "USING (TreeID)")
                    .Where("Log.CruiseID = @p1 AND Log.LogNumber = @p2 AND Tree.CuttingUnitCode = @p3 AND Tree.TreeNumber = @p4 AND Tree.PlotNumber = @p5  AND Log.LogID != @p6")
                    .Query(cruiseID, item.LogNumber, cuttingUnitCode, treeNumber, plotNumber, item.LogID).FirstOrDefault();

                if (conflictItem != null)
                {
                    yield return new Conflict
                    {
                        Table = nameof(Log),
                        Identity = Identify(item),
                        SourceRecID = item.LogID,
                        DestRecID = conflictItem.LogID,
                        SourceRec = item,
                        DestRec = conflictItem,
                        SourceMod = DateMath.Max(item.Created_TS.Value, item.Modified_TS),
                        DestMod = DateMath.Max(conflictItem.Created_TS.Value, conflictItem.Modified_TS),
                        SrcDeviceName = DeviceNameLookup.GetValueOrDefault(item.CreatedBy),
                        DestDeviceName = DeviceNameLookup.GetValueOrDefault(conflictItem.CreatedBy),
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

        protected string Identify(LogEx l)
        {
            var plotNumber = (l.PlotNumber.HasValue) ? ", Plot #:" + l.PlotNumber.Value : "";
            return $"Log: {l.LogNumber}, Unit:{l.CuttingUnitCode}{plotNumber}, Tree #:{l.TreeNumber}";
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