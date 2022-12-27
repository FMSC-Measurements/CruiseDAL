using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class LogFieldSetupSyncer : TableSyncerBase
    {
        public LogFieldSetupSyncer() : base(nameof(LogFieldSetup))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(LogFieldSetup));

            var flags = options.LogFieldSetup;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND StratumCode = @StratumCode AND Field = @Field";

            var strata = destination.From<Stratum>()
                .Where("CruiseID = @p1")
                .Query(cruiseID).ToArray();

            foreach (var st in strata)
            {
                var sourceItems = source.From<LogFieldSetup>().Where("CruiseID = @p1 AND StratumCode = @p2").Query(cruiseID, st.StratumCode);

                foreach (var i in sourceItems)
                {
                    var match = destination.From<LogFieldSetup>()
                        .Where(where)
                        .Query2(i)
                        .FirstOrDefault();

                    if (match == null)
                    {
                        var hasTombstone = destination.From<LogFieldSetup_Tombstone>().Where(where).Count2(i) > 0;

                        if (flags.HasFlag(SyncOption.ForceInsert)
                                || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(i, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementInserts();
                        }
                    }
                }
            }
            return syncResult;
        }
    }
}