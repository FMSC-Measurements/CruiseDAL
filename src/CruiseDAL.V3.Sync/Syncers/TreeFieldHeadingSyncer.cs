using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class TreeFieldHeadingSyncer : TableSyncerBase
    {
        public TreeFieldHeadingSyncer() : base(nameof(TreeFieldHeading))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(TreeFieldHeading));

            var flags = options.TreeFieldHeading;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND Field = @Field";
            var sourceItems = source.From<TreeFieldHeading>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<TreeFieldHeading>().Where(where).Query2(i).FirstOrDefault();

                if (match == null)
                {
                    if (flags.HasFlag(SyncOption.Insert))
                    {
                        destination.Insert(i, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                        syncResult.IncrementInserts();
                    }
                }
                else if (ShouldUpdate(i.Modified_TS, match.Modified_TS, flags))
                {
                    destination.Update(i, whereExpression: where, exceptionProcessor: exceptionProcessor);
                    syncResult.IncrementUpdates();
                }
            }
            return syncResult;
        }
    }
}