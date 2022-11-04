using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class PlotLocationSyncer : TableSyncerBase
    {
        public PlotLocationSyncer() : base(nameof(PlotLocation))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(PlotLocation));

            var flags = options.PlotLocation;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "PlotID = @PlotID";

            var cuttingUnits = destination.From<CuttingUnit>()
                .Where("CruiseID = @p1")
                .Query(cruiseID).ToArray();
            foreach (var cu in cuttingUnits)
            {
                var plotIDs = destination.QueryScalar<string>("SELECT PlotID FROM Plot WHERE CruiseID = @p1 AND CuttingUnitCode = @p2",
                    new[] { cruiseID, cu.CuttingUnitCode });
                foreach (var plotID in plotIDs)
                {
                    var items = source.From<PlotLocation>()
                    .Where("PlotID = @p1")
                    .Query(plotID);

                    foreach (var item in items)
                    {
                        var match = destination.From<PlotLocation>()
                                .Where(where)
                                .Query2(item)
                                .FirstOrDefault();

                        if (match == null)
                        {
                            var hasTombstone = destination.From<PlotLocation>()
                                .Where(where)
                                .Count2(item) > 0;

                            if (hasTombstone == false && flags.HasFlag(SyncOption.Insert))
                            {
                                destination.Insert(item, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                                syncResult.IncrementInserts();
                            }
                        }
                        else
                        {
                            var sMod = item.Modified_TS;
                            var dMod = match.Modified_TS;

                            if (ShouldUpdate(sMod, dMod, flags))
                            {
                                destination.Update(item, whereExpression: where, exceptionProcessor: exceptionProcessor);
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