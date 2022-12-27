using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class BiomassEquationSyncer : TableSyncerBase
    {
        public BiomassEquationSyncer() : base(nameof(BiomassEquation))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(BiomassEquation));

            var flags = options.Processing;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND Species = @Species AND Product = @Product AND Component = @Component AND LiveDead = @LiveDead";
            var sourceItems = source.From<BiomassEquation>()
                .Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<BiomassEquation>()
                    .Where(where).Query2(i).FirstOrDefault();

                if (match == null)
                {
                    if (flags.HasFlag(SyncOption.Insert))
                    {
                        destination.Insert(i, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                        syncResult.IncrementInserts();
                    }
                }
                else
                {
                    if (ShouldUpdate(i.Modified_TS, match.Modified_TS, flags))
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