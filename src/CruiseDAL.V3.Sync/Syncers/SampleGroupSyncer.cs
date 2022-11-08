using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class SampleGroupSyncer : TableSyncerBase
    {
        public SampleGroupSyncer() : base(nameof(SampleGroup))
        {
        }

        protected const string SAMPLEGROUP_WHERE_STATMENT = "CruiseID = @CruiseID AND StratumCode = @StratumCode AND SampleGroupID = @SampleGroupID";

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(SampleGroup));

            var flags = options.SampleGroup;
            if (flags == SyncOption.Lock) { return syncResult; }

            var strata = destination.From<Stratum>()
                .Where("CruiseID = @p1").Query(cruiseID).ToArray();

            foreach (var st in strata)
            {
                var sampleGroups = source.From<SampleGroup>()
                    .Where("CruiseID = @p1 AND StratumCode = @p2")
                    .Query(cruiseID, st.StratumCode);
                foreach (var sg in sampleGroups)
                {
                    SampleGroup match = FindMatch(destination, sg);

                    if (match == null)
                    {
                        bool hasTombstone = FindTombstone(destination, sg);

                        if (flags.HasFlag(SyncOption.ForceInsert)
                            || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(sg, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementInserts();
                        }
                    }
                    else
                    {
                        if (CheckHasSampleGroupSampleingParameterChanges(destination, sg, match, out var errorMsg) && !options.AllowSampleGroupSamplingChanges)
                        {
                            throw new SampleGroupSettingMismatchException(errorMsg)
                            {
                                CruiseID = cruiseID,
                                StratumCode = sg.StratumCode,
                                SampleGroupCode = sg.SampleGroupCode
                            };
                        }

                        var sMod = sg.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, flags))
                        {
                            destination.Update(sg, whereExpression: SAMPLEGROUP_WHERE_STATMENT, exceptionProcessor: exceptionProcessor);
                            syncResult.IncrementUpdates();
                        }
                    }
                }
            }

            return syncResult;
        }

        public static bool CheckHasDesignMismatchErrors(string cruiseID, DbConnection source, DbConnection destination, out string[] errors)
        {
            var sourceItems = source.From<SampleGroup>().Where("CruiseID = @p1").Query(cruiseID);
            var errorList = new List<string>();

            foreach (var sg in sourceItems)
            {
                var match = FindMatch(destination, sg);
                if (match == null) { continue; }

                string error;
                if (CheckHasSampleGroupSampleingParameterChanges(destination, sg, match, out error))
                {
                    errorList.Add(error);
                }
            }

            if (errorList.Count > 0)
            {
                errors = errorList.ToArray();
                return true;
            }
            else
            {
                errors = null;
                return false;
            }
        }

        private static bool FindTombstone(DbConnection destination, SampleGroup sg)
        {
            return destination.From<SampleGroup_Tombstone>()
                                        .Where(SAMPLEGROUP_WHERE_STATMENT)
                                        .Count2(sg) > 0;
        }

        private static SampleGroup FindMatch(DbConnection destination, SampleGroup sg)
        {
            return destination.From<SampleGroup>()
                                    .Where(SAMPLEGROUP_WHERE_STATMENT)
                                    .Query2(sg)
                                    .FirstOrDefault();
        }

        public static bool CheckHasSampleGroupSampleingParameterChanges(DbConnection destination, SampleGroup srcSg, SampleGroup destSg, out string message)
        {
            message = null;

            var hasCruiseData = HasCruiseData(destination, destSg);

            if (srcSg.SamplingFrequency != destSg.SamplingFrequency && hasCruiseData)
            {
                message = $"Sample Group Sampling Frequency Mismatch:::: Sg Code:{srcSg.SampleGroupCode} Stratum Code:{srcSg.StratumCode}";
                return true;
            }

            if (srcSg.KZ != destSg.KZ && hasCruiseData)
            {
                message = $"Sample Group KZ Mismatch:::: Sg Code:{srcSg.SampleGroupCode} Stratum Code:{srcSg.StratumCode}";
                return true;
            }

            if (srcSg.InsuranceFrequency != destSg.InsuranceFrequency && hasCruiseData)
            {
                message = $"Sample Group Insurance Frequency Mismatch:::: Sg Code:{srcSg.SampleGroupCode} Stratum Code:{srcSg.StratumCode}";
                return true;
            }

            if (srcSg.BigBAF != destSg.BigBAF && hasCruiseData)
            {
                message = $"Sample Group BigBAF Mismatch:::: Sg Code:{srcSg.SampleGroupCode} Stratum Code:{srcSg.StratumCode}";
                return true;
            }

            if (srcSg.SmallFPS != destSg.SmallFPS && hasCruiseData)
            {
                message = $"Sample Group SmallFPS Mismatch:::: Sg Code:{srcSg.SampleGroupCode} Stratum Code:{srcSg.StratumCode}";
                return true;
            }

            if (srcSg.TallyBySubPop != destSg.TallyBySubPop && hasCruiseData)
            {
                message = $"Sample Group TallyBySubPop Mismatch:::: Sg Code:{srcSg.SampleGroupCode} Stratum Code:{srcSg.StratumCode}";
                return true;
            }

            if (srcSg.SampleSelectorType != destSg.SampleSelectorType && hasCruiseData)
            {
                message = $"Sample Group Sample Selector Type Mismatch:::: Sg Code:{srcSg.SampleGroupCode} Stratum Code:{srcSg.StratumCode}";
                return true;
            }

            return false;
        }

        public static bool HasCruiseData(DbConnection db, SampleGroup sg)
        {
            return db.From<TallyLedger>().Join("SampleGroup", "USING (CruiseID, StratumCode, SampleGroupCode)")
                        .Where("SampleGroupID = @SampleGroupID").Count2(sg) > 0
                    || db.From<Tree>().Join("SampleGroup", "USING (CruiseID, StratumCode, SampleGroupCode)")
                        .Where("SampleGroupID = @SampleGroupID").Count2(sg) > 0;
        }
    }
}