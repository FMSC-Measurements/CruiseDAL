using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class Plot_StratumSyncer : TableSyncerBase
    {
        public Plot_StratumSyncer() : base(nameof(Plot_Stratum))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(Plot_Stratum));

            var flags = options.PlotStratum;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND CuttingUnitCode = @CuttingUnitCode AND PlotNumber = @PlotNumber AND StratumCode = @StratumCode";

            var cuttingUnits = destination.From<CuttingUnit>()
                .Where("CruiseID = @p1")
                .Query(cruiseID).ToArray();
            foreach (var cu in cuttingUnits)
            {
                var plots = destination.From<Plot>()
                    .Where("CruiseID = @p1 AND CuttingUnitCode = @p2")
                    .Query(cruiseID, cu.CuttingUnitCode).ToArray();

                foreach (var p in plots)
                {
                    var sourceItems = source.From<Plot_Stratum>()
                        .Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber = @p3")
                        .Query(cruiseID, cu.CuttingUnitCode, p.PlotNumber).ToArray();
                    foreach (var i in sourceItems)
                    {
                        var match = destination.From<Plot_Stratum>()
                            .Where(where)
                            .Query2(i)
                            .FirstOrDefault();

                        if (match == null)
                        {
                            var hasTombstone = destination.From<Plot_Stratum>()
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
                            var dMod = match.Modified_TS;

                            if (ShouldUpdate(sMod, dMod, flags))
                            {
                                destination.Update(i, whereExpression: where, exceptionProcessor: exceptionProcessor);
                                syncResult.IncrementUpdates();
                            }
                        }
                    }
                }
            }
            return syncResult;
        }
    }
}