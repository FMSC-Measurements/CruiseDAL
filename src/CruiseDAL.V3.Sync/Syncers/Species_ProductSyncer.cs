using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class Species_ProductSyncer : TableSyncerBase
    {
        public Species_ProductSyncer() : base(nameof(Species_Product))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(Species_Product));

            var flags = options.Species_Product;
            if (flags == SyncOption.Lock) { return syncResult; }

            var sourceItems = source.From<Species_Product>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var i in sourceItems)
            {
                var hasMatch = destination.From<Species_Product>().Where("CruiseID = @CruiseID AND SpeciesCode = @SpeciesCode AND ifnull(PrimaryProduct, '') = ifnull(@PrimaryProduct, '')").Count2(i) > 0;

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