using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class LogSyncer : TableSyncerBase
    {
        public LogSyncer() : base(nameof(Log))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(Log));

            var flags = options.Log;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "LogID = @LogID";

            // only sync for trees that are already in the destination
            // just in case we decide earlier not to sync some trees
            // DONT try to make this query more efficient by only selecting for tree IDs in the log table
            //      we may have trees in the dest file that don't have logs YET that we want to add
            var treeIDs = destination.QueryScalar<string>(
                "SELECT TreeID FROM Tree WHERE CruiseID = @p1;", new[] { cruiseID }).ToArray();
            foreach (var treeID in treeIDs)
            {
                var logs = source.From<Log>()
                    .Where("TreeID = @p1")
                    .Query(treeID).ToArray();
                foreach (var log in logs)
                {
                    var match = destination.From<Log>()
                        .Where(where)
                        .Query2(log)
                        .FirstOrDefault();

                    if (match == null)
                    {
                        var hasTombstone = destination.From<Log_Tombstone>().Where(where).Count2(log) > 0;

                        if (flags.HasFlag(SyncOption.ForceInsert)
                            || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(log, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementInserts();
                        }
                    }
                    else
                    {
                        var sMod = log.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, flags))
                        {
                            destination.Update(log, whereExpression: where, exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementUpdates();
                        }
                    }
                }
            }
            return syncResult;
        }
    }
}