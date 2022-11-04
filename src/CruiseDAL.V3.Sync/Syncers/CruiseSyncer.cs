using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class CruiseSyncer : TableSyncerBase
    {
        public CruiseSyncer() : base(nameof(Cruise))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(Cruise));
            var flags = options.Cruise;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID";

            var sCruise = source.From<Cruise>()
                .Where(where)
                .Query2(new { CruiseID = cruiseID })
                .FirstOrDefault();

            var dCruise = destination.From<Cruise>()
                .Where(where)
                .Query2(new { CruiseID = cruiseID })
                .FirstOrDefault();

            if (dCruise == null && flags.HasFlag(SyncOption.Insert))
            {
                destination.Insert(sCruise, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                syncResult.IncrementInserts();
            }
            else if (dCruise != null)
            {
                var sMod = sCruise.Modified_TS;
                var dMod = dCruise.Modified_TS;

                if (ShouldUpdate(sMod, dMod, flags))
                {
                    destination.Update(sCruise, whereExpression: where, exceptionProcessor: exceptionProcessor);
                    syncResult.IncrementUpdates();
                }
            }

            return syncResult;
        }
    }
}