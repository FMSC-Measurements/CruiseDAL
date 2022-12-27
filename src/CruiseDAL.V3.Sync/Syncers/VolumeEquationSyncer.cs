using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class VolumeEquationSyncer : TableSyncerBase
    {
        public VolumeEquationSyncer() : base(nameof(VolumeEquation))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(VolumeEquation));

            var flags = options.Processing;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND Species = @Species AND PrimaryProduct =  @PrimaryProduct AND VolumeEquationNumber = @VolumeEquationNumber";
            var sourceItems = source.From<VolumeEquation>()
                .Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<VolumeEquation>().Where(where).Query2(i).FirstOrDefault();

                if (match == null)
                {
                    if (flags.HasFlag(SyncOption.Insert))
                    {
                        destination.Insert(i, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                        syncResult.IncrementInserts();
                    }
                }
                else if (ShouldUpdate(i.Modified_TS, match.Modified_TS, flags))
                {
                    destination.Update(i, whereExpression: where, exceptionProcessor: exceptionProcessor);
                    syncResult.IncrementUpdates();
                }
            }
            return syncResult;
        }
    }
}