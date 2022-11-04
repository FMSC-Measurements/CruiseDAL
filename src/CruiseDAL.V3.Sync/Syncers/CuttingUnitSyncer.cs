using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class CuttingUnitSyncer : TableSyncerBase
    {
        public CuttingUnitSyncer() : base(nameof(CuttingUnit))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(CuttingUnit));

            var flags = options.CuttingUnit;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND CuttingUnitID =  @CuttingUnitID";
            var sourceUnits = source.From<CuttingUnit>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceUnits)
            {
                var match = destination.From<CuttingUnit>()
                    .Where(where)
                    .Query2(i)
                    .FirstOrDefault();

                if (match == null)
                {
                    var hasTombstone = destination.From<CuttingUnit_Tombstone>()
                        .Where(where)
                        .Count2(i) > 0;

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
                    var matchMod = match.Modified_TS;

                    if (ShouldUpdate(sMod, matchMod, flags))
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