using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class SamplerStateSyncer : TableSyncerBase
    {
        public SamplerStateSyncer() : base(nameof(SamplerState))
        {
        }

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(SampleGroup));

            var flags = options.SampleGroup;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND StratumCode = @StratumCode AND SampleGroupID = @SampleGroupID";

            var strata = destination.From<Stratum>()
                .Where("CruiseID = @p1").Query(cruiseID).ToArray();

            foreach (var st in strata)
            {
                var sampleGroups = source.From<SampleGroup>()
                    .Where("CruiseID = @p1 AND StratumCode = @p2")
                    .Query(cruiseID, st.StratumCode);
                foreach (var sg in sampleGroups)
                {
                    var match = destination.From<SampleGroup>()
                        .Where(where)
                        .Query2(sg)
                        .FirstOrDefault();

                    if (match == null)
                    {
                        var hasTombstone = destination.From<SampleGroup_Tombstone>()
                            .Where(where)
                            .Count2(sg) > 0;

                        if (flags.HasFlag(SyncOption.ForceInsert)
                            || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(sg, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementInserts();
                        }
                    }
                    else
                    {
                        var sMod = sg.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, flags))
                        {
                            destination.Update(sg, whereExpression: where, exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementUpdates();
                        }
                    }
                }
            }

            return syncResult;
        }
    }
}