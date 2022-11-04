using CruiseDAL.V3.Models;
using CruiseDAL.V3.Sync.Syncers;
using FMSC.ORM.Core;
using FMSC.ORM.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.V3.Sync
{
    // TODO are we good with filtering what logs get synced by only syncing log for treeIDs that have already been synced
    // or do we have to check unit, stratum, samplegroup, plot ID exclude lists. we probably should but thats a lot of
    // extra work. I'm good with just checking the excludeTreeID and the excludeLogID list for now

    // TODO once stems have actually been implemented we need to recheck all that

    // TODO for records like plotLocation and treeLocation, if we only sync records by enumerating the records in
    //      the destination database, do we need to check the exclude list

    // TODO sync Species_Product

    // TODO need to noodle some more on how conflicts with tree is going to work with tally ledger records.
    // when resolving with Chose Dest or Chose Source we probably shouldn't sync TallyLedgers associated with not not-picked record
    // one possible solution for now would be to only allow resolution with Modify Dest or Modify Source

    // TODO there are two ways to handle the ChoseSrourc/ChoseDest resolution. We can leave or merge any child records. I think we need both options. But Chose and Merge should be the default option
    // situations where we need merge child records:
    //      - user added units to files separately, went to cruise and how has tree data in both files. That tree data needs to make it to the final file
    // situations where we need chose but no merge(overwrite child data):
    //      - user added paper data to files separately and now has full duplicate data one two files.

    // when resolving with a 'chose' resolution, can we simplify the choice the user makes by just allowing them to chose the newest version of the record
    // if when checking all downstream conflicts, if all records are the same, can we auto resolve the conflict but going with latest modified

    public class CruiseDatabaseSyncer
    {
        protected delegate TableSyncResult SyncAction(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IExceptionProcessor exceptionProcessor);

        private ILogger _logger;
        private ILogger Logger => _logger ??= LoggerProvider.Get();

        protected static TableSyncerBase[] SYNCERS = new TableSyncerBase[]
        {
            // core
            new SaleSyncer(),
            new CruiseSyncer(),
            new DeviceSyncer(),

            // design
            new CuttingUnitSyncer(),
            new StratumSyncer(),
            new CuttingUnitStratumSyncer(),
            new SampleGroupSyncer(),
            new SamplerStateSyncer(),
            new SpeciesSyncer(),
            new SubpopulationSyncer(),
            new FixCNTTallyPopulationSyncer(),

            // field setup
            new LogFieldSetupSyncer(),
            new TreeFieldSetupSyncer(),
            new TreeFieldHeadingSyncer(),
            new LogFieldHeadingSyncer(),

            // validation
            new TreeAuditRuleSyncer(),
            new TreeAuditRuleSelectorSyncer(),
            new TreeAuditResolutionSyncer(),
            // new LogGradeAuditRuleSyncer(),

            // field data
            new PlotSyncer(),
            new PlotLocationSyncer(),
            new Plot_StratumSyncer(),
            new TreeSyncer(),
            new TreeMeasurmentSyncer(),
            new TreeLocationSyncer(),
            new TreeFieldValueSyncer(),

            new TallyLedgerSyncer(),

            new LogSyncer(),
            new StemSyncer(),

            // processing
            new BiomassEquationSyncer(),
            new ReportsSyncer(),
            new ValueEquationSyncer(),
            new VolumeEquationSyncer(),

            new TreeDefaultValueSyncer(),

            // template
            new StratumTemplateSyncer(),
            new StratumTemplateTreeFieldSetupSyncer(),
            new StratumTemplateLogFieldSetupSyncer(),

            // util
            new CruiseLogSyncer(),
        };

        public bool CheckContiansCruise(DbConnection db, string cruiseID)
        {
            var hasCruise = db.ExecuteScalar<int>("SELECT count(*) FROM Cruise WHERE CruiseID = @p1;", new[] { cruiseID }) > 0;
            //var hasUnits = db.ExecuteScalar<int>("SELECT count(*) FROM CuttingUnit WHERE CruiseID = @p1;", new[] { cruiseID }) > 0;
            //var hasStratum = db.ExecuteScalar<int>("SELECT count(*) FROM Stratum WHERE CruiseID = @p1;", new[] { cruiseID }) > 0;

            return hasCruise;
        }

        public void Sync(string cruiseID, CruiseDatastore source, CruiseDatastore destination, TableSyncOptions options)
        {
            var sourceConn = source.OpenConnection();
            try
            {
                var destConn = destination.OpenConnection();
                try
                {
                    Sync(cruiseID, sourceConn, destConn, options);
                }
                finally
                {
                    destination.ReleaseConnection();
                }
            }
            finally
            {
                source.ReleaseConnection();
            }
        }

        public Task<SyncResult> SyncAsync(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IProgress<double> progress = null, IExceptionProcessor exceptionProcessor = null)
        {
            return Task.Run(() => Sync(cruiseID, source, destination, options, progress));
        }

        public SyncResult Sync(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IProgress<double> progress = null, IExceptionProcessor exceptionProcessor = null)
        {
            var syncResults = new SyncResult();

            var syncers = SYNCERS;
            var steps = syncers.Length;
            double p = 0.0;
            var transaction = destination.BeginTransaction();
            try
            {
                foreach (var syncer in syncers)
                {
                    try
                    {
                        var syncResult = syncer.SyncRecords(cruiseID, source, destination, options, exceptionProcessor);
                        syncResults.Add(syncResult);
                        Logger.Log(syncResult.ToString(), LogCategory.None, LogLevel.Info);
                        progress?.Report(++p / steps);
                    }
                    catch (Exception e)
                    {
                        var tableName = syncer.TableName;
                        Logger.LogException(e);
                        throw new SyncException($"SyncException: {tableName}", e);
                    }
                }

                destination.Insert(new CruiseLog
                {
                    CruiseID = cruiseID,
                    Message = $"Cruise Synced:::: " + syncResults.ToString(),
                    Program = CruiseDatastore.GetCallingProgram(),
                    Level = "I",
                });

                transaction.Commit();

                return syncResults;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        //private void SyncLogGradeAuditRules(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        //{
        //    var where = "CruiseID = @CruiseID AND Grade = @Grade AND ifnull(SpeciesCode, '') = ifnull(@SpeciesCode, '') ";

        //    var sourceItems = source.From<LogGradeAuditRule>().Where("CruiseID = @p1").Query(cruiseID);
        //    foreach (var i in sourceItems)
        //    {
        //        var match = destination.From<LogGradeAuditRule>()
        //            .Where(where).Query2(i);

        //        if (match == null)
        //        {
        //            var hasTombstone = destination.From<LogGradeAuditRule_Tombstone>()
        //                .Where(where).Count2(i) > 0;

        //            if (options.Validation.HasFlag(SyncFlags.ForceInsert)
        //                        || (hasTombstone == false && options.Validation.HasFlag(SyncFlags.Insert)))
        //            {
        //                destination.Insert(i, persistKeyvalue: false);
        //            }
        //        }
        //        // update not supported
        //    }
        //}
    }

    public class TableSyncResult
    {
        public string TableName { get; set; }

        private int _inserts;
        private int _updates;

        public TableSyncResult(string tableName)
        {
            TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        }

        public int Inserts => _inserts;

        public int Updates => _updates;

        public void IncrementInserts()
        { _inserts++; }

        public void IncrementUpdates()
        { _updates++; }

        public override string ToString()
        {
            return $"{TableName} Added:{Inserts} Updated:{Updates}";
        }
    }

    public class SyncResult : IEnumerable<TableSyncResult>
    {
        private readonly Dictionary<string, TableSyncResult> _tableResults = new Dictionary<string, TableSyncResult>();

        public TableSyncResult this[string table]
        {
            get
            {
                if (_tableResults.TryGetValue(table, out TableSyncResult result))
                { return result; }
                else
                { return null; }
            }

            set
            {
                if (_tableResults.ContainsKey(table))
                { _tableResults[table] = value; }
                else
                { _tableResults.Add(table, value); }
            }
        }

        public void Add(TableSyncResult syncResult)
        {
            if (syncResult == null) throw new ArgumentNullException(nameof(syncResult));

            var tableName = syncResult.TableName;
            this[tableName] = syncResult;
        }

        public IEnumerator<TableSyncResult> GetEnumerator()
        {
            return _tableResults.Select(x => x.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _tableResults.Select(x => x.Value).GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var tr in _tableResults)
            {
                sb.AppendLine(tr.Value.ToString());
            }
            return sb.ToString();
        }
    }
}