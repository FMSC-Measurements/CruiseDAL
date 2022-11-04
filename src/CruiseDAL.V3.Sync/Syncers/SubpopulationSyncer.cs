using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class SubpopulationSyncer : TableSyncerBase
    {
        public SubpopulationSyncer() : base(nameof(SubPopulation))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(SubPopulation));

            var flags = options.Subpopulation;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND StratumCode = @StratumCode AND SampleGroupCode = @SampleGroupCode AND SpeciesCode = @SpeciesCode AND LiveDead = @LiveDead";

            var sampleGroups = destination.From<SampleGroup>()
                .Where("CruiseID = @p1")
                .Query(cruiseID).ToArray();
            foreach (var sg in sampleGroups)
            {
                var sourceItems = source.From<SubPopulation>()
                    .Where("CruiseID = @p1 AND SampleGroupCode = @p2")
                    .Query(cruiseID, sg.SampleGroupCode);

                foreach (var i in sourceItems)
                {
                    var match = destination.From<SubPopulation>()
                        .Where(where)
                        .Query2(i)
                        .FirstOrDefault();

                    if (match == null)
                    {
                        var hasTombstone = destination.From<SubPopulation_Tombstone>().Where(where).Count2(i) > 0;

                        if (flags.HasFlag(SyncOption.ForceInsert)
                            || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(i, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementInserts();
                        }
                    }
                }
            }

            return syncResult;
        }
    }
}