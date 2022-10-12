using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using FMSC.ORM.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
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

    public class CruiseSyncer
    {
        private ILogger _logger;
        private ILogger Logger => _logger ??= LoggerProvider.Get();

        protected const string MODIFY_CUTTINGUNIT_COMMAND = "UPDATE CuttingUnit SET CuttingUnitCode = @CuttingUnitCode WHERE CuttingUnitID = @CuttingUnitID;";
        protected const string MODIFY_STRATUM_COMMAND = "UPDATE Stratum SET StratumCode = @StratumCode WHERE StratumID = @StratumID;";
        protected const string MODIFY_SAMPLEGROUP_COMMAND = "UPDATE SampleGroup SET SampleGroupCode = @SampleGroupCode WHERE SampleGroupID = @SampleGroupID;";
        protected const string MODIFY_PLOT_COMMAND = "UPDATE Plot SET PlotNumber = @PlotNumber WHERE PlotID = @PlotID;";
        protected const string MODIFY_TREE_COMMAND = "UPDATE Tree SET TreeNumber = @TreeNumber WHERE TreeID = @TreeID;";
        protected const string MODIFY_LOG_COMMAND = "UPDATE Log SET LogNumber = @LogNumber WHERE LogID = @LogID;";

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

        public Task<SyncResult> SyncAsync(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IProgress<double> progress = null)
        {
            return Task.Run(() => Sync(cruiseID, source, destination, options, progress));
        }

        public SyncResult Sync(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options, IProgress<double> progress = null)
        {
            var syncResults = new SyncResult();

            var steps = 36;
            double p = 0.0;
            var transaction = destination.BeginTransaction();
            try
            {
                // core
                syncResults.Add(SyncSale(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncCruise(cruiseID, source, destination, options));
                progress?.Report(++p / steps);

                syncResults.Add(SyncDevice(cruiseID, source, destination, options));
                progress?.Report(++p / steps);

                // design
                syncResults.Add(SyncCuttingUnits(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncStratum(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncCuttingUnit_Stratum(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncSampleGroup(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncSamplerState(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncSpecies(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncSubpopulation(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncFixCNTTallyPopulation(cruiseID, source, destination, options));
                progress?.Report(++p / steps);

                // field setup
                syncResults.Add(SyncLogFieldSetup(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncTreeFieldSetup(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncTreeFieldHeading(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncLogFieldHeading(cruiseID, source, destination, options));
                progress?.Report(++p / steps);

                // validation
                syncResults.Add(SyncTreeAuditRule(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncTreeAuditRuleSelector(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncTreeAuditResolution(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                //SyncLogGradeAuditRules(cruiseID, source, destination, options);
                //progress?.Report(++p / steps);

                // field data
                syncResults.Add(SyncPlot(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncPlotLocation(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncPlot_Strata(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncTree(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncTreeMeasurment(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncTreeLocation(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncTreeFieldValue(cruiseID, source, destination, options));
                progress?.Report(++p / steps);

                syncResults.Add(SyncTallyLedger(cruiseID, source, destination, options));
                progress?.Report(++p / steps);

                syncResults.Add(SyncLog(cruiseID, source, destination, options));
                progress?.Report(++p / steps);
                syncResults.Add(SyncStem(cruiseID, source, destination, options));
                progress?.Report(++p / steps);

                //processing
                syncResults.Add(SyncBiomassEquation(cruiseID, source, destination, options));
                progress?.Report(++p / steps);

                syncResults.Add(SyncReport(cruiseID, source, destination, options));
                progress?.Report(++p / steps);

                syncResults.Add(SyncValueEquation(cruiseID, source, destination, options));
                progress?.Report(++p / steps);

                syncResults.Add(SyncVolumeEquation(cruiseID, source, destination, options));
                progress?.Report(++p / steps);

                // TreeDefaultValue
                syncResults.Add(SyncTreeDefaultValues(cruiseID, source, destination, options));
                progress?.Report(++p / steps);

                // template
                syncResults.Add(SyncStratumTemplates(cruiseID, source, destination, options));
                progress?.Report(++p / steps);

                syncResults.Add(SyncStratumTemplateTreeFieldSetups(cruiseID, source, destination, options));
                progress?.Report(++p / steps);

                syncResults.Add(SyncStratumTemplateLogFieldSetups(cruiseID, source, destination, options));
                progress?.Report(++p / steps);

                transaction.Commit();

                return syncResults;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
        }

        private bool ShouldUpdate(DateTime? srcMod, DateTime? desMod, SyncOption syncFlags)
        {
            if (srcMod.HasValue == false) { return false; }
            else if ((desMod.HasValue == false)
                || syncFlags.HasFlag(SyncOption.ForceUpdate)
                || (DateTime.Compare(srcMod.Value, desMod.Value) > 0) && syncFlags.HasFlag(SyncOption.Update))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private TableSyncResult SyncSale(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(Sale));
            var flags = options.Sale;
            if (flags == SyncOption.Lock) { return syncResult; }

            var sourceSale = source.From<Sale>()
                .Join("Cruise AS c", "USING (SaleNumber)")
                .Where("CruiseID = @p1")
                .Query(cruiseID).FirstOrDefault();

            var match = destination.From<Sale>()
                .Where("SaleNumber = @p1")
                .Query(sourceSale.SaleNumber).FirstOrDefault();

            var saleIDMatch = destination.From<Sale>()
                .Where("SaleID = @p1")
                .Query(sourceSale.SaleID).FirstOrDefault();

            if (match == null && saleIDMatch == null)
            {
                if (flags.HasFlag(SyncOption.Insert))
                {
                    destination.Insert(sourceSale, persistKeyvalue: false);
                    syncResult.IncrementInserts();
                }
            }
            else if (saleIDMatch == null)
            {
                //we have a saleNumber match but no SaleID match, e.g. merging a cruise into a database containing a cruise with the same sale number
                var srcMod = sourceSale.Modified_TS;
                var destMod = match.Modified_TS;

                if (ShouldUpdate(srcMod, destMod, flags))
                {
                    destination.Update(sourceSale, whereExpression: "SaleNumber = @SaleNumber");
                    syncResult.IncrementUpdates();
                }
            }
            else
            {
                // we have a saleID match, but maybe not a sale number match
                var srcMod = sourceSale.Modified_TS;
                var destMod = saleIDMatch.Modified_TS;

                if (ShouldUpdate(srcMod, destMod, flags))
                {
                    destination.Update(sourceSale, whereExpression: "SaleID = @SaleID");
                    syncResult.IncrementUpdates();
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncCruise(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(Cruise));
            var flags = options.Cruise;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID";

            var sCruise = source.From<Cruise>()
                .Where(where)
                .Query2(new { CruiseID = cruiseID })
                .FirstOrDefault();

            var dCruise = destination.From<Cruise>()
                .Where(where)
                .Query2(new { CruiseID = cruiseID })
                .FirstOrDefault();

            if (dCruise == null && flags.HasFlag(SyncOption.Insert))
            {
                destination.Insert(sCruise, persistKeyvalue: false);
                syncResult.IncrementInserts();
            }
            else if (dCruise != null)
            {
                var sMod = sCruise.Modified_TS;
                var dMod = dCruise.Modified_TS;

                if (ShouldUpdate(sMod, dMod, flags))
                {
                    destination.Update(sCruise, whereExpression: where);
                    syncResult.IncrementUpdates();
                }
            }

            return syncResult;
        }

        private TableSyncResult SyncDevice(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(Device));

            var flags = options.Device;
            if (flags == SyncOption.Lock) { return syncResult; }

            var sourceItems = source.From<Device>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var hasMatch = destination.ExecuteScalar<long>("SELECT count(*) FROM Device WHERE DeviceID = @p1 AND CruiseID = @p2;", new object[] { i.DeviceID, cruiseID }) > 0;

                if (hasMatch == false && flags.HasFlag(SyncOption.Insert))
                {
                    destination.Insert(i, persistKeyvalue: false);
                }
            }

            return syncResult;
        }

        private TableSyncResult SyncCuttingUnits(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(CuttingUnit));

            var flags = options.CuttingUnit;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND CuttingUnitID =  @CuttingUnitID";
            var sourceUnits = source.From<CuttingUnit>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceUnits)
            {
                var match = destination.From<CuttingUnit>()
                    .Where(where)
                    .Query2(i)
                    .FirstOrDefault();

                if (match == null)
                {
                    var hasTombstone = destination.From<CuttingUnit_Tombstone>()
                        .Where(where)
                        .Count2(i) > 0;

                    if (flags.HasFlag(SyncOption.ForceInsert)
                        || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                        syncResult.IncrementInserts();
                    }
                }
                else
                {
                    var sMod = i.Modified_TS;
                    var matchMod = match.Modified_TS;

                    if (ShouldUpdate(sMod, matchMod, flags))
                    {
                        destination.Update(i, whereExpression: where);
                        syncResult.IncrementUpdates();
                    }
                }
            }

            return syncResult;
        }

        private TableSyncResult SyncCuttingUnit_Stratum(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(CuttingUnit_Stratum));

            var flags = options.CuttingUnitStratum;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CuttingUnitCode = @CuttingUnitCode AND StratumCode = @StratumCode  AND CruiseID = @CruiseID";
            var sourceItems = source.From<CuttingUnit_Stratum>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {

                var match = destination.From<CuttingUnit_Stratum>().Where(where).Count2(i) > 0;

                if (match == false)
                {
                    var hasTombstone = destination.From<CuttingUnit_Stratum>().Where(where).Count2(i) > 0;

                    if (flags.HasFlag(SyncOption.ForceInsert)
                        || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                        syncResult.IncrementInserts();
                    }
                }
            }

            return syncResult;
        }

        private TableSyncResult SyncStratum(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(Stratum));

            var flags = options.Stratum;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND StratumID = @StratumID";

            var sourceItems = source.From<Stratum>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {

                var match = destination.From<Stratum>()
                    .Where(where)
                    .Query2(i)
                    .FirstOrDefault();

                if (match == null)
                {
                    var hasTombstone = destination.From<Stratum_Tombstone>()
                        .Where(where)
                        .Count2(i) > 0;

                    if (flags.HasFlag(SyncOption.ForceInsert)
                        || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                        syncResult.IncrementInserts();
                    }
                }
                else
                {
                    var sMod = i.Modified_TS;
                    var dMod = match.Modified_TS;

                    if (ShouldUpdate(sMod, dMod, flags))
                    {
                        destination.Update(i, whereExpression: where);
                        syncResult.IncrementUpdates();
                    }
                }
            }

            return syncResult;
        }

        private TableSyncResult SyncSampleGroup(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
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
                            destination.Insert(sg, persistKeyvalue: false);
                            syncResult.IncrementInserts();
                        }
                    }
                    else
                    {
                        var sMod = sg.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, flags))
                        {
                            destination.Update(sg, whereExpression: where);
                            syncResult.IncrementUpdates();
                        }
                    }
                }
            }

            return syncResult;
        }

        private TableSyncResult SyncSamplerState(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(SamplerState));

            var flags = options.SamplerState;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND StratumCode = @StratumCode AND SampleGroupCode = @SampleGroupCode AND DeviceID = @DeviceID";

            var sampleGroups = destination.From<SampleGroup>()
                .Where("CruiseID = @p1")
                .Query(cruiseID).ToArray();
            foreach (var sg in sampleGroups)
            {
                var sStates = source.From<SamplerState>()
                    .Where("CruiseID = @p1 AND SampleGroupCode = @p2")
                    .Query(cruiseID, sg.SampleGroupCode);

                foreach (var i in sStates)
                {
                    var match = destination.From<SamplerState>()
                        .Where(where)
                        .Query2(i)
                        .FirstOrDefault();

                    if (match == null)
                    {
                        if (flags.HasFlag(SyncOption.Insert))
                        {
                            destination.Insert(i, persistKeyvalue: false);
                            syncResult.IncrementInserts();
                        }
                    }
                    else
                    {
                        var sMod = i.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, flags))
                        {
                            destination.Update(i, whereExpression: where);
                            syncResult.IncrementUpdates();
                        }
                    }
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncSpecies(string cruiseID, DbConnection source, DbConnection desination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(Species));

            var flags = options.Species;
            if (flags == SyncOption.Lock) { return syncResult; }

            var sourceItems = source.From<Species>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var i in sourceItems)
            {
                var hasMatch = desination.From<Species>().Where("CruiseID = @CruiseID AND SpeciesCode = @SpeciesCode").Count2(i) > 0;

                if (hasMatch == false && flags.HasFlag(SyncOption.Insert))
                {
                    desination.Insert(i, persistKeyvalue: false);
                    syncResult.IncrementInserts();
                }
            }

            return syncResult;
        }

        private TableSyncResult SyncSubpopulation(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
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
                            destination.Insert(i, persistKeyvalue: false);
                            syncResult.IncrementInserts();
                        }
                    }
                }
            }

            return syncResult;
        }

        private TableSyncResult SyncFixCNTTallyPopulation(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
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
                                destination.Insert(i, persistKeyvalue: false);
                                syncResult.IncrementInserts();
                            }
                        }
                        else
                        {
                            var sMod = i.Modified_TS;
                            var dMod = match.Modified_TS;

                            if (ShouldUpdate(sMod, dMod, flags))
                            {
                                destination.Update(i, whereExpression: where);
                                syncResult.IncrementUpdates();
                            }
                        }
                    }
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncPlot(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(Plot));

            var flags = options.Plot;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND PlotID = @PlotID";

            var cuttingUnits = destination.From<CuttingUnit>()
                .Where("CruiseID = @p1").Query(cruiseID).ToArray();
            foreach (var cu in cuttingUnits)
            {
                var plots = source.From<Plot>()
                    .Where("CuttingUnitCode = @p1 AND CruiseID = @p2")
                    .Query(cu.CuttingUnitCode, cruiseID).ToArray();
                foreach (var plot in plots)
                {
                    var match = destination.From<Plot>()
                        .Where(where)
                        .Query2(plot)
                        .FirstOrDefault();

                    if (match == null)
                    {
                        var hasTombstone = destination.From<Plot>()
                            .Where(where)
                            .Count2(plot) > 0;

                        if (flags.HasFlag(SyncOption.ForceInsert)
                            || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(plot, persistKeyvalue: false);
                            syncResult.IncrementInserts();
                        }
                    }
                    else
                    {
                        var sMod = plot.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, flags))
                        {
                            destination.Update(plot, whereExpression: where);
                            syncResult.IncrementUpdates();
                        }
                    }
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncPlotLocation(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
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
                                destination.Insert(item, persistKeyvalue: false);
                                syncResult.IncrementInserts();
                            }
                        }
                        else
                        {
                            var sMod = item.Modified_TS;
                            var dMod = match.Modified_TS;

                            if (ShouldUpdate(sMod, dMod, flags))
                            {
                                destination.Update(item, whereExpression: where);
                                syncResult.IncrementUpdates();
                            }
                        }
                    }
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncPlot_Strata(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(Plot_Stratum));

            var flags = options.PlotStratum;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND CuttingUnitCode = @CuttingUnitCode AND PlotNumber = @PlotNumber AND StratumCode = @StratumCode";

            var cuttingUnits = destination.From<CuttingUnit>()
                .Where("CruiseID = @p1")
                .Query(cruiseID).ToArray();
            foreach (var cu in cuttingUnits)
            {
                var plots = destination.From<Plot>()
                    .Where("CruiseID = @p1 AND CuttingUnitCode = @p2")
                    .Query(cruiseID, cu.CuttingUnitCode).ToArray();

                foreach (var p in plots)
                {
                    var sourceItems = source.From<Plot_Stratum>()
                        .Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber = @p3")
                        .Query(cruiseID, cu.CuttingUnitCode, p.PlotNumber).ToArray();
                    foreach (var i in sourceItems)
                    {
                        var match = destination.From<Plot_Stratum>()
                            .Where(where)
                            .Query2(i)
                            .FirstOrDefault();

                        if (match == null)
                        {
                            var hasTombstone = destination.From<Plot_Stratum>()
                                .Where(where)
                                .Count2(i) > 0;

                            if (flags.HasFlag(SyncOption.ForceInsert)
                                || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                            {
                                destination.Insert(i, persistKeyvalue: false);
                                syncResult.IncrementInserts();
                            }
                        }
                        else
                        {
                            var sMod = i.Modified_TS;
                            var dMod = match.Modified_TS;

                            if (ShouldUpdate(sMod, dMod, flags))
                            {
                                destination.Update(i, whereExpression: where);
                                syncResult.IncrementUpdates();
                            }
                        }
                    }
                }
            }
            return syncResult;
        }

        public static DateTime GetPlotTreeModified(DbConnection db, string cruiseID, string unitCode, int plotNumber)
        {
            return db.ExecuteScalar<DateTime>("SELECT  max( max(Created_TS), ifnull(max(Modified_TS), datetime(0))) FROM Tree WHERE CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber = @p3",
                        new object[] { cruiseID, unitCode, plotNumber });
        }

        private TableSyncResult SyncTree(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(Tree));

            var flags = options.Tree;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND TreeID = @TreeID";

            var cuttingUnits = destination.From<CuttingUnit>()
                .Where("CruiseID = @p1")
                .Query(cruiseID).ToArray();
            foreach (var cu in cuttingUnits)
            {
                var sourceTrees = source.From<Tree>().Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber IS NULL")
                    .Query(cruiseID, cu.CuttingUnitCode);
                foreach (var i in sourceTrees)
                {
                    var match = destination.From<Tree>()
                        .Where(where)
                        .Query2(i)
                        .FirstOrDefault();

                    if (match == null)
                    {
                        var hasTombstone = destination.From<Tree_Tombstone>()
                            .Where(where)
                            .Count2(i) > 0;

                        if (flags.HasFlag(SyncOption.ForceInsert)
                            || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(i, persistKeyvalue: false);
                            syncResult.IncrementInserts();
                        }
                    }
                    else
                    {
                        var sMod = i.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, flags))
                        {
                            destination.Update(i, whereExpression: where);
                            syncResult.IncrementUpdates();
                        }
                    }
                }
            }

            var plots = destination.From<Plot>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var plot in plots)
            {
                var sourceTrees = source.From<Tree>()
                    .Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber = @p3")
                    .Query(cruiseID, plot.CuttingUnitCode, plot.PlotNumber).ToArray();

                foreach (var tree in sourceTrees)
                {
                    var match = destination.From<Tree>()
                        .Where(where)
                        .Query2(tree)
                        .FirstOrDefault();

                    if (match == null)
                    {
                        // we're only checking for a tombstone if a match doesn't exist
                        // it could be possible for a tombstone to exist event if a match
                        // exists. However, I think we can ignore tombstones in such cases.
                        // perhaps we decided to reinsert the record from another file, but
                        // keep the original tombstone around to retain the records history.

                        var hasTombstone = destination.From<Tree_Tombstone>()
                            .Where(where)
                            .Count2(tree) > 0;

                        if (flags.HasFlag(SyncOption.ForceInsert)
                                || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(tree, persistKeyvalue: false);
                            syncResult.IncrementInserts();
                        }
                    }
                    else
                    {
                        var sMod = tree.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, flags))
                        {
                            destination.Update(match, whereExpression: where);
                            syncResult.IncrementUpdates();
                        }
                    }
                }
            }


            return syncResult;
        }

        private TableSyncResult SyncTreeMeasurment(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            // not checking the tombstone tables for TreeMeasurment because at this
            // point we should already know that the tree has not be deleted.
            // I assuming that TreeMeasurment records wont be deleted unless the tree has been deleted

            var syncResult = new TableSyncResult(nameof(TreeMeasurment));

            var flags = options.TreeMeasurment;
            if (flags == SyncOption.Lock) { return syncResult; }

            var trees = destination.From<Tree>().Where("CruiseID = @p1")
                .Query(cruiseID);
            foreach (var tree in trees)
            {
                var treeID = tree.TreeID;

                var sourceMeasurmentsRecord = source.From<TreeMeasurment>().Where("TreeID = @p1").Query(treeID).FirstOrDefault();
                if (sourceMeasurmentsRecord != null)
                {
                    var hasMeasurmentsRecord = destination.ExecuteScalar<long>("SELECT count(*) FROM TreeMeasurment WHERE TreeID =  @p1;", parameters: new[] { treeID }) > 0;

                    if (hasMeasurmentsRecord)
                    {
                        if (flags.HasFlag(SyncOption.Update))
                        {
                            destination.Update(sourceMeasurmentsRecord, whereExpression: "TreeID =  @TreeID");
                            syncResult.IncrementUpdates();
                        }
                    }
                    else
                    {
                        if (flags.HasFlag(SyncOption.Insert))
                        {
                            destination.Insert(sourceMeasurmentsRecord, persistKeyvalue: false);
                            syncResult.IncrementInserts();
                        }
                    }
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncTreeLocation(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(TreeLocation));

            var flags = options.TreeLocation;
            if (flags == SyncOption.Lock) { return syncResult; }


            var trees = destination.From<Tree>().Where("CruiseID = @p1")
                .Query(cruiseID);
            foreach (var tree in trees)
            {
                var treeID = tree.TreeID;
                var treeLocationRecord = source.From<TreeLocation>()
    .Where("TreeID = @p1")
    .Query(treeID)
    .FirstOrDefault();

                if (treeLocationRecord != null)
                {
                    var match = destination.From<TreeLocation>()
                        .Where("TreeID = @p1")
                        .Query(treeID)
                        .FirstOrDefault();

                    if (match == null)
                    {
                        var hasTombstone = destination.From<TreeLocation_Tombstone>()
                            .Where("TreeID = @p1")
                            .Count(treeID) > 0;

                        if (flags.HasFlag(SyncOption.ForceInsert)
                                || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(treeLocationRecord, persistKeyvalue: false);
                            syncResult.IncrementInserts();
                        }
                    }
                    else
                    {
                        var sMod = treeLocationRecord.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, flags))
                        {
                            destination.Update(treeLocationRecord, whereExpression: "TreeID = @TreeID");
                            syncResult.IncrementUpdates();
                        }
                    }
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncTreeFieldValue(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            // not checking the tombstone tables for TreeFieldValue because at this
            // point we should already know that the tree has not be deleted.
            // we are assuming that TreeFieldValue records wont be deleted unless the tree has been deleted
            // however that might change for TreeFieldValue records

            var syncResult = new TableSyncResult(nameof(TreeFieldValue));

            var flags = options.TreeFieldValue;
            if (flags == SyncOption.Lock) { return syncResult; }

            var trees = destination.From<Tree>().Where("CruiseID = @p1")
                .Query(cruiseID);
            foreach (var tree in trees)
            {
                var treeID = tree.TreeID;

                var treeFieldValues = source.From<TreeFieldValue>().Where("TreeID =  @p1").Query(treeID);
                foreach (var tfv in treeFieldValues)
                {
                    var hasTFVRecord = destination.From<TreeFieldValue>().Where("TreeID = @TreeID AND Field = @Field").Count2(tfv) > 0;
                    if (hasTFVRecord)
                    {
                        if (flags.HasFlag(SyncOption.Update))
                        {
                            destination.Update(tfv, whereExpression: "TreeID = TreeID AND Field = @Field");
                            syncResult.IncrementUpdates();
                        }
                    }
                    else
                    {
                        if (flags.HasFlag(SyncOption.Insert))
                        {
                            destination.Insert(tfv, persistKeyvalue: false);
                            syncResult.IncrementInserts();
                        }
                    }
                }
            }

            return syncResult;
        }

        public static DateTime? GetTreeDataModified(DbConnection db, string treeID)
        {
            // get the latest time stamp on the tree by aggregating the Created Timestamp and Modified Timestamps
            // from both the TreeMeasurment record and the TreeFieldValues records.
            return db.ExecuteScalar<DateTime>(
@" SELECT max(mod) FROM (
        SELECT max( tm.Created_TS, ifnull(tm.Modified_TS, datetime(0))) AS mod FROM TreeMeasurment AS tm WHERE tm.TreeID = @p1
        UNION ALL
        SELECT max( tfv.Created_TS, ifnull(tfv.Modified_TS, datetime(0))) AS mod FROM TreeFieldValue AS tfv WHERE tfv.TreeID = @p1
        UNION ALL
        SELECT max( tl.Created_TS, ifnull(tl.Modified_TS, datetime(0))) AS mod FROM TreeLocation AS tl WHERE tl.TreeID = @p1
);", parameters: new[] { treeID });
        }

        private TableSyncResult SyncLog(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(Log));

            var flags = options.Log;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "LogID = @LogID";

            // only sync for trees that are already in the destination
            // just in case we decide earlier not to sync some trees
            // DONT try to make this query more efficient by only selecting for tree IDs in the log table
            //      we may have trees in the dest file that don't have logs YET that we want to add
            var treeIDs = destination.QueryScalar<string>(
                "SELECT TreeID FROM Tree WHERE CruiseID = @p1;", new[] { cruiseID }).ToArray();
            foreach (var treeID in treeIDs)
            {
                var logs = source.From<Log>()
                    .Where("TreeID = @p1")
                    .Query(treeID).ToArray();
                foreach (var log in logs)
                {
                    var match = destination.From<Log>()
                        .Where(where)
                        .Query2(log)
                        .FirstOrDefault();

                    if (match == null)
                    {
                        var hasTombstone = destination.From<Log_Tombstone>().Where(where).Count2(log) > 0;

                        if (flags.HasFlag(SyncOption.ForceInsert)
                            || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(log, persistKeyvalue: false);
                            syncResult.IncrementInserts();
                        }
                    }
                    else
                    {
                        var sMod = log.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, flags))
                        {
                            destination.Update(log, whereExpression: where);
                            syncResult.IncrementUpdates();
                        }
                    }
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncStem(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(Stem));

            var flags = options.Stem;
            if (flags == SyncOption.Lock) { return syncResult; }

            var allOrNone = options.StemAllOrNone;
            var where = "CruiseID = @CruiseID AND StemID = @StemID";

            var stemTrees = source.Query<TreeID_Modified_TS>("SELECT s.TreeID, max(s.Modified_TS) FROM Stem AS s JOIN Tree AS t USING (TreeID) WHERE t.CruiseID = @p1 GROUP BY t.TreeID;",
                paramaters: new[] { cruiseID });

            foreach (var tree in stemTrees)
            {
                var destMod = destination.Query<TreeID_Modified_TS>("SELECT TreeID, max(Modified_TS) FROM Stem WHERE TreeID = @p1 GROUP BY TreeID;",
                    paramaters: new[] { tree.TreeID }).FirstOrDefault();

                if (allOrNone == false // if not doing all-or-none ignore timestamp comparison and carry on
                    || destMod == null || tree.Modified_TS.CompareTo(destMod.Modified_TS) > 0) // destination has no stems OR source is newer than destination
                {
                    var sourceItems = source.From<Stem>().Where("CruiseID = @p1").Query(cruiseID);

                    foreach (var i in sourceItems)
                    {
                        var match = destination.From<Stem>()
                            .Where(where)
                            .Query2(i)
                            .FirstOrDefault();

                        if (match == null)
                        {
                            var hasTombstone = destination.From<Stem_Tombstone>().Where(where).Count2(i) > 0;
                            if (flags.HasFlag(SyncOption.ForceInsert)
                            || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                            {
                                destination.Insert(i, persistKeyvalue: false);
                                syncResult.IncrementInserts();
                            }
                        }
                        else
                        {
                            var sMod = i.Modified_TS;
                            var dMod = match.Modified_TS;

                            if (ShouldUpdate(sMod, dMod, flags))
                            {
                                destination.Update(i, whereExpression: where);
                                syncResult.IncrementUpdates();
                            }
                        }
                    }
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncTallyLedger(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(TallyLedger));

            var flags = options.TallyLedger;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND TallyLedgerID = @TallyLedgerID";

            var units = destination.From<CuttingUnit>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var unit in units)
            {
                var sourceItems = source.From<TallyLedger>().Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber IS NULL").Query(cruiseID, unit.CuttingUnitCode);
                foreach (var i in sourceItems)
                {
                    var match = destination.From<TallyLedger>()
                        .Where(where)
                        .Query2(i)
                        .FirstOrDefault();

                    if (match == null)
                    {
                        var hasTombstone = destination.From<TallyLedger_Tombstone>().Where(where).Count2(i) > 0;

                        if (flags.HasFlag(SyncOption.ForceInsert)
                                || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(i, persistKeyvalue: false);
                            syncResult.IncrementInserts();
                        }
                    }
                }
            }

            var plots = destination.From<Plot>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var plot in plots)
            {
                var sourceTallyLedgers = source.From<TallyLedger>()
                    .Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber = @p3")
                    .Query(cruiseID, plot.CuttingUnitCode, plot.PlotNumber);

                foreach (var tl in sourceTallyLedgers)
                {
                    var hasMatch = destination.ExecuteScalar<long>("SELECT count(*) FROM TallyLedger WHERE TallyLedgerID = @p1;", parameters: new[] { tl.TallyLedgerID }) > 0;
                    if (!hasMatch)
                    {
                        var hasTombstone = destination.ExecuteScalar<long>("SELECT count(*) FROM TallyLedger_Tombstone WHERE TallyLedgerID = @p1;", parameters: new[] { tl.TallyLedgerID }) > 0;

                        if (flags.HasFlag(SyncOption.ForceInsert)
                                        || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(tl, persistKeyvalue: false);
                            syncResult.IncrementInserts();
                        }
                    }
                }
            }


            return syncResult;
        }

        private TableSyncResult SyncLogFieldSetup(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(LogFieldSetup));

            var flags = options.LogFieldSetup;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND StratumCode = @StratumCode AND Field = @Field";

            var strata = destination.From<Stratum>()
                .Where("CruiseID = @p1")
                .Query(cruiseID).ToArray();

            foreach (var st in strata)
            {
                var sourceItems = source.From<LogFieldSetup>().Where("CruiseID = @p1 AND StratumCode = @p2").Query(cruiseID, st.StratumCode);

                foreach (var i in sourceItems)
                {
                    var match = destination.From<LogFieldSetup>()
                        .Where(where)
                        .Query2(i)
                        .FirstOrDefault();

                    if (match == null)
                    {
                        var hasTombstone = destination.From<LogFieldSetup_Tombstone>().Where(where).Count2(i) > 0;

                        if (flags.HasFlag(SyncOption.ForceInsert)
                                || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                        {
                            destination.Insert(i, persistKeyvalue: false);
                            syncResult.IncrementInserts();
                        }
                    }
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncTreeFieldSetup(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(TreeFieldSetup));

            var flags = options.TreeFieldSetup;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND StratumCode = @StratumCode AND coalesce(SampleGroupCode, '') = coalesce(@SampleGroupCode, '') AND Field = @Field";

            var sourceItems = source.From<TreeFieldSetup>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<TreeFieldSetup>()
                    .Where(where)
                    .Query2(i)
                    .FirstOrDefault();

                if (match == null)
                {
                    var hasTombstone = destination.From<TreeFieldSetup_Tombstone>().Where(where).Count2(i) > 0;

                    if (flags.HasFlag(SyncOption.ForceInsert)
                            || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                        syncResult.IncrementInserts();
                    }
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncTreeAuditRule(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(TreeAuditRule));

            var flags = options.TreeAuditRule;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "TreeAuditRuleID = @TreeAuditRuleID";

            var sourceItems = source.From<TreeAuditRule>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<TreeAuditRule>()
                    .Where(where)
                    .Query2(i)
                    .FirstOrDefault();

                if (match == null)
                {
                    var hasTombstone = destination.From<TreeAuditRule_Tombstone>().Where(where).Count2(i) > 0;

                    if (flags.HasFlag(SyncOption.ForceInsert)
                            || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                        syncResult.IncrementInserts();
                    }
                }
                // update not supported
            }
            return syncResult;
        }

        private TableSyncResult SyncTreeAuditRuleSelector(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(TreeAuditRuleSelector));

            var flags = options.TreeAuditRuleSelector;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "TreeAuditRuleID = @TreeAuditRuleID AND ifnull(SpeciesCode, '') = ifnull(@SpeciesCode, '') AND ifnull(LiveDead, '') = ifnull(@LiveDead, '') AND ifnull(PrimaryProduct, '') = ifnull(@PrimaryProduct, '')";

            var sourceItems = source.From<TreeAuditRuleSelector>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var i in sourceItems)
            {
                var match = destination.From<TreeAuditRuleSelector>()
                    .Where(where)
                    .Query2(i)
                    .FirstOrDefault();
                if (match == null)
                {
                    var hasTombstone = destination.From<TreeAuditRuleSelector_Tombstone>()
                        .Where(where).Count2(i) > 0;

                    if (flags.HasFlag(SyncOption.ForceInsert)
                            || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                        syncResult.IncrementInserts();
                    }
                }
                // update not supported
            }
            return syncResult;
        }

        private TableSyncResult SyncTreeAuditResolution(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(TreeAuditResolution));

            var flags = options.TreeAuditResolution;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "TreeAuditRuleID = @TreeAuditRuleID AND TreeID = @TreeID";

            var sourceItems = source.From<TreeAuditResolution>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var i in sourceItems)
            {
                var match = destination.From<TreeAuditResolution>()
                    .Where(where).Query2(i);

                // it is possible that the match doesn't have the same initials, or resolution values
                // but I think it is safe to ignore conflicts in this situation just as long as there is a resolution

                if (match == null)
                {
                    var hasTombstone = destination.From<TreeAuditResolution_Tombstone>()
                        .Where(where).Count2(i) > 0;

                    if (flags.HasFlag(SyncOption.ForceInsert)
                                || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                        syncResult.IncrementInserts();
                    }
                }
                // update not supported
            }
            return syncResult;
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

        private TableSyncResult SyncTreeFieldHeading(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(TreeFieldHeading));

            var flags = options.TreeFieldHeading;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND Field = @Field";
            var sourceItems = source.From<TreeFieldHeading>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<TreeFieldHeading>().Where(where).Query2(i).FirstOrDefault();

                if (match == null)
                {
                    if (flags.HasFlag(SyncOption.Insert))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                        syncResult.IncrementInserts();
                    }
                }
                else if (ShouldUpdate(i.Modified_TS, match.Modified_TS, flags))
                {
                    destination.Update(i, whereExpression: where);
                    syncResult.IncrementUpdates();
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncLogFieldHeading(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(LogFieldHeading));

            var flags = options.LogFieldHeading;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND Field = @Field";
            var sourceItems = source.From<LogFieldHeading>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<LogFieldHeading>()
                    .Where(where).Query2(i).FirstOrDefault();

                if (match == null)
                {
                    if (flags.HasFlag(SyncOption.Insert))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                        syncResult.IncrementInserts();
                    }
                }
                else if (ShouldUpdate(i.Modified_TS, match.Modified_TS, flags))
                {
                    destination.Update(i, whereExpression: where);
                    syncResult.IncrementUpdates();
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncBiomassEquation(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
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
                        destination.Insert(i, persistKeyvalue: false);
                        syncResult.IncrementInserts();
                    }
                }
                else
                {
                    if (ShouldUpdate(i.Modified_TS, match.Modified_TS, flags))
                    {
                        destination.Update(i, whereExpression: where);
                        syncResult.IncrementUpdates();
                    }
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncReport(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(Reports));

            var flags = options.Processing;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND ReportID = @ReportID";
            var sourceItems = source.From<Reports>()
                .Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<Reports>()
                    .Where(where).Query2(i).FirstOrDefault();

                if (match == null)
                {
                    if (flags.HasFlag(SyncOption.Insert))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                        syncResult.IncrementInserts();
                    }
                }
                else if (ShouldUpdate(i.Modified_TS, match.Modified_TS, flags))
                {
                    destination.Update(i, whereExpression: where);
                    syncResult.IncrementUpdates();
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncValueEquation(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(ValueEquation));

            var flags = options.Processing;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND Species = @Species AND PrimaryProduct = @PrimaryProduct AND ValueEquationNumber = @ValueEquationNumber";
            var sourceItems = source.From<ValueEquation>()
                .Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<ValueEquation>()
                    .Where(where).Query2(i).FirstOrDefault();

                if (match == null)
                {
                    if (flags.HasFlag(SyncOption.Insert))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                        syncResult.IncrementInserts();
                    }
                }
                else if (ShouldUpdate(i.Modified_TS, match.Modified_TS, flags))
                {
                    destination.Update(i, whereExpression: where);
                    syncResult.IncrementUpdates();
                }
            }
            return syncResult;

        }

        private TableSyncResult SyncVolumeEquation(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
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
                        destination.Insert(i, persistKeyvalue: false);
                        syncResult.IncrementInserts();
                    }
                }
                else if (ShouldUpdate(i.Modified_TS, match.Modified_TS, flags))
                {
                    destination.Update(i, whereExpression: where);
                    syncResult.IncrementUpdates();
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncTreeDefaultValues(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(TreeDefaultValue));

            var flags = options.TreeDefaultValue;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND coalesce(SpeciesCode, '') = coalesce(@SpeciesCode, '') AND coalesce(PrimaryProduct, '') = coalesce(@PrimaryProduct, '')";
            var sourceItems = source.From<TreeDefaultValue>()
                .Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<TreeDefaultValue>().Where(where).Query2(i).FirstOrDefault();

                if (match == null)
                {
                    var hasTombstone = destination.From<TreeDefaultValue_Tombstone>()
                                                    .Where(where).Count2(i) > 0;

                    if (flags.HasFlag(SyncOption.ForceInsert)
                                || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                        syncResult.IncrementInserts();
                    }
                }
                else
                {
                    if (ShouldUpdate(i.Modified_TS, match.Modified_TS, flags))
                    {
                        destination.Update(i, whereExpression: where);
                        syncResult.IncrementUpdates();
                    }
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncStratumTemplates(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(StratumTemplate));

            var flags = options.Template;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND StratumTemplateName = @StratumTemplateName";
            var sourceItems = source.From<StratumTemplate>()
                .Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<StratumTemplate>().Where(where).Query2(i).FirstOrDefault();

                if (match == null)
                {
                    var hasTombstone = destination.From<StratumTemplate_Tombstone>()
                                                    .Where(where).Count2(i) > 0;

                    if (flags.HasFlag(SyncOption.ForceInsert)
                                || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                        syncResult.IncrementInserts();
                    }
                }
                else if (ShouldUpdate(i.Modified_TS, match.Modified_TS, flags))
                {
                    destination.Update(i, whereExpression: where);
                    syncResult.IncrementUpdates();
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncStratumTemplateTreeFieldSetups(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(StratumTemplateTreeFieldSetup));

            var flags = options.Template;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND StratumTemplateName = @StratumTemplateName AND Field = @Field";
            var sourceItems = source.From<StratumTemplateTreeFieldSetup>()
                .Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<StratumTemplateTreeFieldSetup>().Where(where).Query2(i).FirstOrDefault();

                if (match == null)
                {
                    var hasTombstone = destination.From<StratumTemplateTreeFieldSetup_Tombstone>()
                                .Where(where).Count2(i) > 0;

                    if (flags.HasFlag(SyncOption.ForceInsert)
                                || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                        syncResult.IncrementInserts();
                    }
                }
                else if (ShouldUpdate(i.Modified_TS, match.Modified_TS, flags))
                {
                    destination.Update(i, whereExpression: where);
                    syncResult.IncrementUpdates();
                }
            }
            return syncResult;
        }

        private TableSyncResult SyncStratumTemplateLogFieldSetups(string cruiseID, DbConnection source, DbConnection destination, TableSyncOptions options)
        {
            var syncResult = new TableSyncResult(nameof(StratumTemplateLogFieldSetup));

            var flags = options.Template;
            if (flags == SyncOption.Lock) { return syncResult; }

            var where = "CruiseID = @CruiseID AND StratumTemplateName = @StratumTemplateName AND Field = @Field";
            var sourceItems = source.From<StratumTemplateLogFieldSetup>()
                .Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<StratumTemplateLogFieldSetup>().Where(where).Query2(i).FirstOrDefault();

                if (match == null)
                {
                    var hasTombstone = destination.From<StratumTemplateLogFieldSetup_Tombstone>()
                                .Where(where).Count2(i) > 0;

                    if (flags.HasFlag(SyncOption.ForceInsert)
                                || (hasTombstone == false && flags.HasFlag(SyncOption.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                        syncResult.IncrementInserts();
                    }
                }
                else if (ShouldUpdate(i.Modified_TS, match.Modified_TS, flags))
                {
                    destination.Update(i, whereExpression: where);
                    syncResult.IncrementUpdates();
                }
            }
            return syncResult;
        }
    }

    public class TableSyncResult
    {
        public string TableName { get; set; }

        int _inserts;
        int _updates;

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
        Dictionary<string, TableSyncResult> _tableResults = new Dictionary<string, TableSyncResult>();

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
    }

    public class TreeID_Modified_TS
    {
        public string TreeID { get; set; }

        public DateTime Modified_TS { get; set; }
    }
}