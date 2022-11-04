using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class TallyLedgerSyncer : TableSyncerBase
    {
        public TallyLedgerSyncer() : base(nameof(TallyLedger))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(TallyLedger));

            var flags = options.TallyLedger;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND TallyLedgerID = @TallyLedgerID";

            var units = destination.From<CuttingUnit>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var unit in units)
            {
                var sourceItems = source.From<TallyLedger>().Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber IS NULL").Query(cruiseID, unit.CuttingUnitCode);
                foreach (var i in sourceItems)
                {
                    var match = destination.From<TallyLedger>()
                        .Where(where)
                        .Query2(i)
                        .FirstOrDefault();

                    if (match == null)
                    {
                        var hasTombstone = destination.From<TallyLedger_Tombstone>().Where(where).Count2(i) > 0;

                        if (flags.HasFlag(SyncOption.ForceInsert)
                                || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(i, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementInserts();
                        }
                    }
                }
            }

            var plots = destination.From<Plot>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var plot in plots)
            {
                var sourceTallyLedgers = source.From<TallyLedger>()
                    .Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber = @p3")
                    .Query(cruiseID, plot.CuttingUnitCode, plot.PlotNumber);

                foreach (var tl in sourceTallyLedgers)
                {
                    var hasMatch = destination.ExecuteScalar<long>("SELECT count(*) FROM TallyLedger WHERE TallyLedgerID = @p1;", parameters: new[] { tl.TallyLedgerID }) > 0;
                    if (!hasMatch)
                    {
                        var hasTombstone = destination.ExecuteScalar<long>("SELECT count(*) FROM TallyLedger_Tombstone WHERE TallyLedgerID = @p1;", parameters: new[] { tl.TallyLedgerID }) > 0;

                        if (flags.HasFlag(SyncOption.ForceInsert)
                                        || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(tl, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementInserts();
                        }
                    }
                }
            }

            return syncResult;
        }
    }
}