using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

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
                var match = FindMatch(destination, i);

                if (match == null)
                {
                    bool hasTombstone = FindTombstone(destination, i);

                    if (flags.HasFlag(SyncOption.ForceInsert)
                        || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false, exceptionProcessor: exceptionProcessor);
                        syncResult.IncrementInserts();
                    }
                }
                else
                {
                    if (CheckHasStratumDesignMismatch(destination, i, match, out var errorMessage) && !options.AllowStratumDesignChanges)
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

        public static bool CheckHasDesignMismatchErrors(string cruiseID, DbConnection source, DbConnection destination, out string[] errors)
        {
            var sourceItems = source.From<Stratum>().Where("CruiseID = @p1").Query(cruiseID);
            var errorList = new List<string>();

            foreach (var st in sourceItems)
            {
                var match = FindMatch(destination, st);
                if (match == null) { continue; }

                string error;
                if (CheckHasStratumDesignMismatch(destination, st, match, out error))
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

        protected static bool FindTombstone(DbConnection destination, Stratum st)
        {
            return destination.From<Stratum_Tombstone>()
                                    .Where(STRATUM_WHERE_STATMENT)
                                    .Count2(st) > 0;
        }

        protected static Stratum FindMatch(DbConnection destination, Stratum st)
        {
            return destination.From<Stratum>()
                                .Where(STRATUM_WHERE_STATMENT)
                                .Query2(st)
                                .FirstOrDefault();
        }

        protected static bool CheckHasStratumDesignMismatch(DbConnection destination, Stratum srcStratum, Stratum destStratum, out string message)
        {
            message = null;
            var hasCruiseData = HasCruiseData(destination, destStratum);

            if (srcStratum.Method != destStratum.Method && hasCruiseData)
            {
                message = $"Cruise Method Mismatch: Stratum Code:{srcStratum.StratumCode}";
                return true;
            }

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

        protected static bool HasCruiseData(DbConnection db, Stratum st)
        {
            return db.From<TallyLedger>().Join("Stratum", "USING (CruiseID, StratumCode)")
                        .Where("StratumID = @StratumID").Count2(st) > 0
                    || db.From<Tree>().Join("Stratum", "USING (CruiseID, StratumCode)")
                        .Where("StratumID = @StratumID").Count2(st) > 0;
        }
    }
}