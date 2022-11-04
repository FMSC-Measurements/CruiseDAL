using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class CruiseLogSyncer : TableSyncerBase
    {
        public CruiseLogSyncer() : base(nameof(CruiseLog))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(CruiseLog));

            var flags = options.CruiseLog;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseLogID = @CruiseLogID";
            var sourceItems = source.From<CruiseLog>()
                .Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<CruiseLog>().Where(where).Query2(i).FirstOrDefault();

                if (match == null)
                {
                    if (flags.HasFlag(SyncOption.Insert))
                    {
                        destination.Insert(i, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                        syncResult.IncrementInserts();
                    }
                }
                // no update, because cruise log records are immutable
            }
            return syncResult;
        }
    }
}