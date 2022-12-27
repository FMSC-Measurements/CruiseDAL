using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class StemSyncer : TableSyncerBase
    {
        public StemSyncer() : base(nameof(Stem))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(Stem));

            var flags = options.Stem;
            if (flags == SyncOption.Lock) { return syncResult; }

            var allOrNone = options.StemAllOrNone;
            var where = "CruiseID = @CruiseID AND StemID = @StemID";

            var stemTrees = source.Query<TreeID_Modified_TS>("SELECT s.TreeID, max(s.Modified_TS) FROM Stem AS s JOIN Tree AS t USING (TreeID) WHERE t.CruiseID = @p1 GROUP BY t.TreeID;",
                paramaters: new[] { cruiseID });

            foreach (var tree in stemTrees)
            {
                var destMod = destination.Query<TreeID_Modified_TS>("SELECT TreeID, max(Modified_TS) FROM Stem WHERE TreeID = @p1 GROUP BY TreeID;",
                    paramaters: new[] { tree.TreeID }).FirstOrDefault();

                if (allOrNone == false // if not doing all-or-none ignore timestamp comparison and carry on
                    || destMod == null || tree.Modified_TS.CompareTo(destMod.Modified_TS) > 0) // destination has no stems OR source is newer than destination
                {
                    var sourceItems = source.From<Stem>().Where("CruiseID = @p1").Query(cruiseID);

                    foreach (var i in sourceItems)
                    {
                        var match = destination.From<Stem>()
                            .Where(where)
                            .Query2(i)
                            .FirstOrDefault();

                        if (match == null)
                        {
                            var hasTombstone = destination.From<Stem_Tombstone>().Where(where).Count2(i) > 0;
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
                                destination.Update(i, whereExpression: where, exceptionProcessor: exceptionProcessor);
                                syncResult.IncrementUpdates();
                            }
                        }
                    }
                }
            }
            return syncResult;
        }

        private class TreeID_Modified_TS
        {
            public string TreeID { get; set; }

            public DateTime Modified_TS { get; set; }
        }
    }
}