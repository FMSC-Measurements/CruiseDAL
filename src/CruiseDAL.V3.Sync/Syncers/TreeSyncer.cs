﻿using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class TreeSyncer : TableSyncerBase
    {
        public TreeSyncer() : base(nameof(Tree))
        {
        }

        private static string WHERE = "CruiseID = @CruiseID AND TreeID = @TreeID";

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(Tree));

            var flags = options.Tree;
            if (flags == SyncOption.Lock) { return syncResult; }

            SyncUnitTrees(cruiseID, source, destination, exceptionProcessor, syncResult, flags);

            SyncPlotTrees(cruiseID, source, destination, exceptionProcessor, syncResult, flags);

            return syncResult;
        }

        private static void SyncUnitTrees(string cruiseID, DbConnection source, DbConnection destination, IExceptionProcessor exceptionProcessor, TableSyncResult syncResult, SyncOption flags)
        {
            var cuttingUnits = destination.From<CuttingUnit>()
                            .Where("CruiseID = @p1")
                            .Query(cruiseID).ToArray();
            foreach (var cu in cuttingUnits)
            {
                var sourceTrees = source.From<Tree>().Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber IS NULL")
                    .Query(cruiseID, cu.CuttingUnitCode);
                foreach (var i in sourceTrees)
                {
                    var match = destination.From<Tree>()
                        .Where(WHERE)
                        .Query2(i)
                        .FirstOrDefault();

                    if (match == null)
                    {
                        var hasTombstone = destination.From<Tree_Tombstone>()
                            .Where(WHERE)
                            .Count2(i) > 0;

                        if (flags.HasFlag(SyncOption.ForceInsert)
                            || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(i, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementInserts();
                        }
                    }
                    else
                    {
                        var sMod = i.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, flags))
                        {
                            destination.Update(i, whereExpression: WHERE, exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementUpdates();
                        }
                    }
                }
            }
        }

        private static void SyncPlotTrees(string cruiseID, DbConnection source, DbConnection destination, IExceptionProcessor exceptionProcessor, TableSyncResult syncResult, SyncOption flags)
        {
            var plots = destination.From<Plot>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var plot in plots)
            {
                var sourceTrees = source.From<Tree>()
                    .Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber = @p3")
                    .Query(cruiseID, plot.CuttingUnitCode, plot.PlotNumber).ToArray();

                foreach (var tree in sourceTrees)
                {
                    var match = destination.From<Tree>()
                        .Where(WHERE)
                        .Query2(tree)
                        .FirstOrDefault();

                    if (match == null)
                    {
                        // we're only checking for a tombstone if a match doesn't exist
                        // it could be possible for a tombstone to exist event if a match
                        // exists. However, I think we can ignore tombstones in such cases.
                        // perhaps we decided to reinsert the record from another file, but
                        // keep the original tombstone around to retain the records history.

                        var hasTombstone = destination.From<Tree_Tombstone>()
                            .Where(WHERE)
                            .Count2(tree) > 0;

                        if (flags.HasFlag(SyncOption.ForceInsert)
                                || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(tree, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementInserts();
                        }
                    }
                    else
                    {
                        var sMod = tree.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, flags))
                        {
                            destination.Update(match, whereExpression: WHERE, exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementUpdates();
                        }
                    }
                }
            }
        }
    }
}