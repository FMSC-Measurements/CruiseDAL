using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class PlotSyncer : TableSyncerBase
    {
        public PlotSyncer() : base(nameof(Plot))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(Plot));

            var flags = options.Plot;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND PlotID = @PlotID";

            var cuttingUnits = destination.From<CuttingUnit>()
                .Where("CruiseID = @p1").Query(cruiseID).ToArray();
            foreach (var cu in cuttingUnits)
            {
                var plots = source.From<Plot>()
                    .Where("CuttingUnitCode = @p1 AND CruiseID = @p2")
                    .Query(cu.CuttingUnitCode, cruiseID).ToArray();
                foreach (var plot in plots)
                {
                    var match = destination.From<Plot>()
                        .Where(where)
                        .Query2(plot)
                        .FirstOrDefault();

                    if (match == null)
                    {
                        var hasTombstone = destination.From<Plot>()
                            .Where(where)
                            .Count2(plot) > 0;

                        if (flags.HasFlag(SyncOption.ForceInsert)
                            || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(plot, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementInserts();
                        }
                    }
                    else
                    {
                        var sMod = plot.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, flags))
                        {
                            destination.Update(plot, whereExpression: where, exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementUpdates();
                        }
                    }
                }
            }
            return syncResult;
        }
    }
}