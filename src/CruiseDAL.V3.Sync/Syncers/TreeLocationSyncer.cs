using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class TreeLocationSyncer : TableSyncerBase
    {
        public TreeLocationSyncer() : base(nameof(TreeLocation))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(TreeLocation));

            var flags = options.TreeLocation;
            if (flags == SyncOption.Lock) { return syncResult; }

            var trees = destination.From<Tree>().Where("CruiseID = @p1")
                .Query(cruiseID);
            foreach (var tree in trees)
            {
                var treeID = tree.TreeID;
                var treeLocationRecord = source.From<TreeLocation>()
    .Where("TreeID = @p1")
    .Query(treeID)
    .FirstOrDefault();

                if (treeLocationRecord != null)
                {
                    var match = destination.From<TreeLocation>()
                        .Where("TreeID = @p1")
                        .Query(treeID)
                        .FirstOrDefault();

                    if (match == null)
                    {
                        var hasTombstone = destination.From<TreeLocation_Tombstone>()
                            .Where("TreeID = @p1")
                            .Count(treeID) > 0;

                        if (flags.HasFlag(SyncOption.ForceInsert)
                                || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(treeLocationRecord, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementInserts();
                        }
                    }
                    else
                    {
                        var sMod = treeLocationRecord.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, flags))
                        {
                            destination.Update(treeLocationRecord, whereExpression: "TreeID = @TreeID", exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementUpdates();
                        }
                    }
                }
            }
            return syncResult;
        }
    }
}