using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class CuttingUnitStratumSyncer : TableSyncerBase
    {
        public CuttingUnitStratumSyncer() : base(nameof(CuttingUnit_Stratum))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(CuttingUnit_Stratum));

            var flags = options.CuttingUnitStratum;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CuttingUnitCode = @CuttingUnitCode AND StratumCode = @StratumCode  AND CruiseID = @CruiseID";
            var sourceItems = source.From<CuttingUnit_Stratum>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<CuttingUnit_Stratum>().Where(where).Count2(i) > 0;

                if (match == false)
                {
                    var hasTombstone = destination.From<CuttingUnit_Stratum>().Where(where).Count2(i) > 0;

                    if (flags.HasFlag(SyncOption.ForceInsert)
                        || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                        syncResult.IncrementInserts();
                    }
                }
            }

            return syncResult;
        }
    }
}