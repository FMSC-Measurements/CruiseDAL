using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class TreeDefaultValueSyncer : TableSyncerBase
    {
        public TreeDefaultValueSyncer() : base(nameof(TreeDefaultValue))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(TreeDefaultValue));

            var flags = options.TreeDefaultValue;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND coalesce(SpeciesCode, '') = coalesce(@SpeciesCode, '') AND coalesce(PrimaryProduct, '') = coalesce(@PrimaryProduct, '')";
            var sourceItems = source.From<TreeDefaultValue>()
                .Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<TreeDefaultValue>().Where(where).Query2(i).FirstOrDefault();

                if (match == null)
                {
                    var hasTombstone = destination.From<TreeDefaultValue_Tombstone>()
                                                    .Where(where).Count2(i) > 0;

                    if (flags.HasFlag(SyncOption.ForceInsert)
                                || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                        syncResult.IncrementInserts();
                    }
                }
                else
                {
                    if (ShouldUpdate(i.Modified_TS, match.Modified_TS, flags))
                    {
                        destination.Update(i, whereExpression: where, exceptionProcessor: exceptionProcessor);
                        syncResult.IncrementUpdates();
                    }
                }
            }
            return syncResult;
        }
    }
}