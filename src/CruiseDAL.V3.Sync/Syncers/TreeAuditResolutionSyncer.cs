using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class TreeAuditResolutionSyncer : TableSyncerBase
    {
        public TreeAuditResolutionSyncer() : base(nameof(TreeAuditResolution))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(TreeAuditResolution));

            var flags = options.TreeAuditResolution;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "TreeAuditRuleID = @TreeAuditRuleID AND TreeID = @TreeID";

            var sourceItems = source.From<TreeAuditResolution>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var i in sourceItems)
            {
                var match = destination.From<TreeAuditResolution>()
                    .Where(where).Query2(i);

                // it is possible that the match doesn't have the same initials, or resolution values
                // but I think it is safe to ignore conflicts in this situation just as long as there is a resolution

                if (match == null)
                {
                    var hasTombstone = destination.From<TreeAuditResolution_Tombstone>()
                        .Where(where).Count2(i) > 0;

                    if (flags.HasFlag(SyncOption.ForceInsert)
                                || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                        syncResult.IncrementInserts();
                    }
                }
                // update not supported
            }
            return syncResult;
        }
    }
}