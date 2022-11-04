using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

namespace CruiseDAL.V3.Sync.Syncers
{
    public class StratumSyncer : TableSyncerBase
    {
        public StratumSyncer() : base(nameof(Stratum))
        {
        }

        protected const string STRATUM_WHERE_STATMENT = "CruiseID = @CruiseID AND StratumID = @StratumID";

        public override TableSyncResult SyncRecords(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor)
        {
            var syncResult = new TableSyncResult(nameof(Stratum));

            var flags = options.Stratum;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND StratumID = @StratumID";

            var sourceItems = source.From<Stratum>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<Stratum>()
                    .Where(STRATUM_WHERE_STATMENT)
                    .Query2(i)
                    .FirstOrDefault();

                if (match == null)
                {
                    var hasTombstone = destination.From<Stratum_Tombstone>()
                        .Where(STRATUM_WHERE_STATMENT)
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
                    if(HasStratumCruiseMethodMatch(destination, options, i, match, out var errorMessage) && !options.AllowCruiseMethodChanges)
                    {
                        throw new StratumSettingMismatchException(errorMessage)
                        {
                            CruiseID = cruiseID,
                            StratumCode = i.StratumCode,
                        };
                    }

                    if(HasStratumSamplingPrametersMatch(destination, options, i, match, out var samplingParamsErrorMessage) && !options.AllowStratumSamplingChanges)
                    {
                        throw new StratumSettingMismatchException(errorMessage)
                        {
                            CruiseID = cruiseID,
                            StratumCode = i.StratumCode,
                        };
                    }

                    var sMod = i.Modified_TS;
                    var dMod = match.Modified_TS;

                    if (ShouldUpdate(sMod, dMod, flags))
                    {
                        destination.Update(i, whereExpression: where, exceptionProcessor: exceptionProcessor);
                        syncResult.IncrementUpdates();
                    }
                }
            }

            return syncResult;
        }

        public static bool HasStratumCruiseMethodMatch(DbConnection destination, TableSyncOptions options, Stratum srcStratum, Stratum destStratum, out string message)
        {
            message = null;
            if (srcStratum.Method != destStratum.Method && HasCruiseData(destination, destStratum))
            {
                message = $"Cruise Method Mismatch: Stratum Code:{srcStratum.StratumCode}";
                return true;
            }
            return false;
        }

        public static bool HasStratumSamplingPrametersMatch(DbConnection destination, TableSyncOptions options, Stratum srcStratum, Stratum destStratum, out string message)
        {
            // todo breadk out HasCruiseData into method and fix cruise data check to use StratumID

            message = null;
            var hasCruiseData = HasCruiseData(destination, destStratum);

            if (srcStratum.KZ3PPNT != destStratum.KZ3PPNT && hasCruiseData)
            {
                message = $"Kz3Ppnt Mismatch: Stratum Code:{srcStratum.StratumCode}";
                return true;
            }

            if (srcStratum.SamplingFrequency != destStratum.SamplingFrequency && hasCruiseData)
            {
                message = $"Sampling Frequency Mismatch: Stratum Code:{srcStratum.StratumCode}";
                return true;
            }

            if (srcStratum.BasalAreaFactor != destStratum.BasalAreaFactor && hasCruiseData)
            {
                message = $"BAF Mismatch: Stratum Code:{srcStratum.StratumCode}";
                return true;
            }

            if (srcStratum.FixedPlotSize != destStratum.FixedPlotSize && hasCruiseData)
            {
                message = $"FPS Mismatch: Stratum Code:{srcStratum.StratumCode}";
                return true;
            }

            if (srcStratum.FixCNTField != destStratum.FixCNTField && hasCruiseData)
            {
                message = $"FixCNT Field Mismatch: Stratum Code:{srcStratum.StratumCode}";
                return true;
            }

            return false;
        }

        public static bool HasCruiseData(DbConnection db, Stratum st)
        {
            return db.From<TallyLedger>().Join("Stratum", "USING (CruiseID, StratumCode)")
                        .Where("StratumID = @StratumID").Count2(st) > 0
                    || db.From<Tree>().Join("Stratum", "USING (CruiseID, StratumCode)")
                        .Where("StratumID = @StratumID").Count2(st) > 0;
        }
    }
}