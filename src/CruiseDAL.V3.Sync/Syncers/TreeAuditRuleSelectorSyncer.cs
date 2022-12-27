using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class TreeAuditRuleSelectorSyncer : TableSyncerBase
    {
        public TreeAuditRuleSelectorSyncer() : base(nameof(TreeAuditRuleSelector))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(TreeAuditRuleSelector));

            var flags = options.TreeAuditRuleSelector;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "TreeAuditRuleID = @TreeAuditRuleID AND ifnull(SpeciesCode, '') = ifnull(@SpeciesCode, '') AND ifnull(LiveDead, '') = ifnull(@LiveDead, '') AND ifnull(PrimaryProduct, '') = ifnull(@PrimaryProduct, '')";

            var sourceItems = source.From<TreeAuditRuleSelector>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var i in sourceItems)
            {
                var match = destination.From<TreeAuditRuleSelector>()
                    .Where(where)
                    .Query2(i)
                    .FirstOrDefault();
                if (match == null)
                {
                    var hasTombstone = destination.From<TreeAuditRuleSelector_Tombstone>()
                        .Where(where).Count2(i) > 0;

                    if (flags.HasFlag(SyncOption.ForceInsert)
                            || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                        syncResult.IncrementInserts();
                    }
                }
                // update not supported
            }
            return syncResult;
        }
    }
}