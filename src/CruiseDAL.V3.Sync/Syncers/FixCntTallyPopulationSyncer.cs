using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class FixCNTTallyPopulationSyncer : TableSyncerBase
    {
        public FixCNTTallyPopulationSyncer() : base(nameof(FixCNTTallyPopulation))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(FixCNTTallyPopulation));

            var flags = options.FixCNTTallyPopulation;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND StratumCode = @StratumCode AND SampleGroupCode = @SampleGroupCode AND SpeciesCode = @SpeciesCode AND LiveDead = @LiveDead";

            var strata = destination.From<Stratum>()
                .Where("CruiseID = @p1")
                .Query(cruiseID).ToArray();
            foreach (var st in strata)
            {
                var sampleGroups = destination.From<SampleGroup>()
                .Where("CruiseID = @p1 AND StratumCode = @p2")
                .Query(cruiseID, st.StratumCode).ToArray();
                foreach (var sg in sampleGroups)
                {
                    var sourceItems = source.From<FixCNTTallyPopulation>()
                        .Where("CruiseID = @p1 AND StratumCode = @p2 AND SampleGroupCode = @p3")
                        .Query(cruiseID, sg.StratumCode, sg.SampleGroupCode);

                    foreach (var i in sourceItems)
                    {
                        var match = destination.From<FixCNTTallyPopulation>()
                            .Where(where)
                            .Query2(i)
                            .FirstOrDefault();

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