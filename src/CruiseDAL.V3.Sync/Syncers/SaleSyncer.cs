using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class SaleSyncer : TableSyncerBase
    {
        public SaleSyncer() : base(nameof(Sale))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(Sale));
            var flags = options.Sale;
            if (flags == SyncOption.Lock) { return syncResult; }

            var sourceSale = source.From<Sale>()
                .Join("Cruise AS c", "USING (SaleNumber)")
                .Where("CruiseID = @p1")
                .Query(cruiseID).FirstOrDefault();

            var match = destination.From<Sale>()
                .Where("SaleNumber = @p1")
                .Query(sourceSale.SaleNumber).FirstOrDefault();

            var saleIDMatch = destination.From<Sale>()
                .Where("SaleID = @p1")
                .Query(sourceSale.SaleID).FirstOrDefault();

            if (match == null && saleIDMatch == null)
            {
                if (flags.HasFlag(SyncOption.Insert))
                {
                    destination.Insert(sourceSale, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                    syncResult.IncrementInserts();
                }
            }
            else if (saleIDMatch == null)
            {
                //we have a saleNumber match but no SaleID match, e.g. merging a cruise into a database containing a cruise with the same sale number
                var srcMod = sourceSale.Modified_TS;
                var destMod = match.Modified_TS;

                if (ShouldUpdate(srcMod, destMod, flags))
                {
                    destination.Update(sourceSale, whereExpression: "SaleNumber = @SaleNumber", exceptionProcessor: exceptionProcessor);
                    syncResult.IncrementUpdates();
                }
            }
            else
            {
                // we have a saleID match, but maybe not a sale number match
                var srcMod = sourceSale.Modified_TS;
                var destMod = saleIDMatch.Modified_TS;

                if (ShouldUpdate(srcMod, destMod, flags))
                {
                    destination.Update(sourceSale, whereExpression: "SaleID = @SaleID", exceptionProcessor: exceptionProcessor);
                    syncResult.IncrementUpdates();
                }
            }
            return syncResult;
        }
    }
}