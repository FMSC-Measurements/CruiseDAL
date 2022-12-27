using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class SpeciesSyncer : TableSyncerBase
    {
        public SpeciesSyncer() : base(nameof(Species))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(Species));

            var flags = options.Species;
            if (flags == SyncOption.Lock) { return syncResult; }

            var sourceItems = source.From<Species>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var i in sourceItems)
            {
                var hasMatch = destination.From<Species>().Where("CruiseID = @CruiseID AND SpeciesCode = @SpeciesCode").Count2(i) > 0;

                if (hasMatch == false && flags.HasFlag(SyncOption.Insert))
                {
                    destination.Insert(i, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                    syncResult.IncrementInserts();
                }
            }

            return syncResult;
        }
    }
}