using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using FMSC.ORM.Logging;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace CruiseDAL.V3.Sync
{
    public class CruiseSyncer : IDbSyncer
    {
        private ILogger _logger;
        private ILogger Logger => _logger ??= LoggerProvider.Get();

        public bool CheckContiansCruise(DbConnection db, string cruiseID)
        {
            var hasCruise = db.ExecuteScalar<int>("SELECT count(*) FROM Cruise WHERE CruiseID = @p1;", new[] { cruiseID }) > 0;
            //var hasUnits = db.ExecuteScalar<int>("SELECT count(*) FROM CuttingUnit WHERE CruiseID = @p1;", new[] { cruiseID }) > 0;
            //var hasStratum = db.ExecuteScalar<int>("SELECT count(*) FROM Stratum WHERE CruiseID = @p1;", new[] { cruiseID }) > 0;

            return hasCruise;
        }

        public void Sync(string cruiseID, CruiseDatastore source, CruiseDatastore destination, CruiseSyncOptions options)
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

        public Task SyncAsync(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options, IProgress<float> progress = null)
        {
            return Task.Run(() => Sync(cruiseID, source, destination, options, progress));
        }

        public void Sync(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options, IProgress<float> progress = null)
        {
            var steps = 34;
            float p = 0.0f;
            var transaction = destination.BeginTransaction();
            try
            {
                // core
                SyncSale(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncCruise(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                SyncDevice(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                // design
                SyncCuttingUnits(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncStrata(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncCuttingUnit_Stratum(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncSampleGroup(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncSamplerState(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncSpecies(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncSubPopulation(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncFixCNTTallyPopulation(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                // field setup
                SyncLogFieldSetup(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncTreeFieldSetup(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncTreeFieldHeading(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncLogFieldHeading(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                // validation
                SyncTreeAuditRule(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncTreeAuditRuleSelector(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncTreeAuditResolution(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                //SyncLogGradeAuditRules(cruiseID, source, destination, options);
                //progress?.Report(p++ / steps);

                // field data
                SyncPlots(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncPlotLocation(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncPlot_Strata(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncPlotTree(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncNonPlotTrees(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                SyncTallyLedger(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                SyncLog(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncStem(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                //processing
                SyncBiomassEquations(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                SyncReports(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                SyncValueEquations(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                SyncVolumeEquations(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                // TreeDefaultValue
                SyncTreeDefaultValues(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                // template
                SyncStratumTemplates(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                SyncStratumTemplateTreeFieldSetups(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                SyncStratumTemplateLogFieldSetups(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
        }

        private bool ShouldUpdate(DateTime? srcMod, DateTime? desMod, SyncFlags syncFlags)
        {
            if (srcMod.HasValue == false) { return false; }
            else if ((desMod.HasValue == false)
                || syncFlags.HasFlag(SyncFlags.ForceUpdate)
                || (DateTime.Compare(srcMod.Value, desMod.Value) > 0) && syncFlags.HasFlag(SyncFlags.Update))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SyncSale(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var sourceSale = source.From<Sale>()
                .Join("Cruise AS c", "USING (SaleNumber)")
                .Where("CruiseID = @p1")
                .Query(cruiseID).FirstOrDefault();

            var match = destination.From<Sale>()
                .Where("SaleNumber = @p1")
                .Query(sourceSale.SaleNumber).FirstOrDefault();

            if (match == null)
            {
                destination.Insert(sourceSale, persistKeyvalue: false);
            }
            else
            {
                var srcMod = sourceSale.Modified_TS;
                var destMod = match.Modified_TS;

                if (sourceSale.SaleID == match.SaleID && ShouldUpdate(srcMod, destMod, options.Design))
                {
                    destination.Update(sourceSale, whereExpression: "SaleID = @SaleID");
                }
            }
        }

        private void SyncCruise(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID";

            var sCruise = source.From<Cruise>()
                .Where(where)
                .Query2(new { CruiseID = cruiseID })
                .FirstOrDefault();

            var dCruise = destination.From<Cruise>()
                .Where(where)
                .Query2(new { CruiseID = cruiseID })
                .FirstOrDefault();

            if (dCruise == null)
            {
                if (options.Design.HasFlag(SyncFlags.Insert))
                {
                    destination.Insert(sCruise, persistKeyvalue: false);
                }
            }
            else
            {
                var sMod = sCruise.Modified_TS;
                var dMod = dCruise.Modified_TS;

                if (ShouldUpdate(sMod, dMod, options.Design))
                {
                    destination.Update(sCruise, whereExpression: where);
                }
            }
        }

        private void SyncDevice(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var sourceItems = source.From<Device>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var hasMatch = destination.ExecuteScalar<long>("SELECT count(*) FROM Device WHERE DeviceID = @p1 AND CruiseID = @p2;", new object[] { i.DeviceID, cruiseID }) > 0;

                if (hasMatch == false)
                {
                    destination.Insert(i, persistKeyvalue: false);
                }
            }
        }

        private void SyncCuttingUnits(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
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

                    if (options.Design.HasFlag(SyncFlags.ForceInsert)
                        || (hasTombstone == false && options.Design.HasFlag(SyncFlags.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                    }
                }
                else
                {
                    var sMod = i.Modified_TS;
                    var matchMod = match.Modified_TS;

                    if (ShouldUpdate(sMod, matchMod, options.Design))
                    {
                        destination.Update(i, whereExpression: where);
                    }
                }
            }
        }

        private void SyncCuttingUnit_Stratum(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CuttingUnitCode = @CuttingUnitCode AND StratumCode = @StratumCode  AND CruiseID = @CruiseID";
            var sourceItems = source.From<CuttingUnit_Stratum>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var i in sourceItems)
            {
                var hasItem = destination.From<CuttingUnit_Stratum>().Where(where).Count2(i) > 0;
                var hasTombstone = destination.From<CuttingUnit_Stratum>().Where(where).Count2(i) > 0;

                if (hasItem == false && options.Design.HasFlag(SyncFlags.Insert))
                {
                    if (hasTombstone == false || options.Design.HasFlag(SyncFlags.ForceInsert))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                    }
                }
            }
        }

        private void SyncStrata(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
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

                    if (options.Design.HasFlag(SyncFlags.ForceInsert)
                        || (hasTombstone == false && options.Design.HasFlag(SyncFlags.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                    }
                }
                else
                {
                    var sMod = i.Modified_TS;
                    var dMod = match.Modified_TS;

                    if (ShouldUpdate(sMod, dMod, options.Design))
                    {
                        destination.Update(i, whereExpression: where);
                    }
                }
            }
        }

        private void SyncSampleGroup(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND StratumCode = @StratumCode AND SampleGroupID = @SampleGroupID";

            var stratumCodes = destination.QueryScalar<string>("SELECT StratumCode FROM Stratum WHERE CruiseID = @p1", new[] { cruiseID });
            foreach (var stCode in stratumCodes)
            {
                var sampleGroups = source.From<SampleGroup>().Where("CruiseID = @p1").Query(cruiseID);
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

                        if (options.Design.HasFlag(SyncFlags.ForceInsert)
                            || (hasTombstone == false && options.Design.HasFlag(SyncFlags.Insert)))
                        {
                            if (options.Design.HasFlag(SyncFlags.Insert))
                            {
                                destination.Insert(sg, persistKeyvalue: false);
                            }
                        }
                    }
                    else
                    {
                        var sMod = sg.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, options.Design))
                        {
                            destination.Update(sg, whereExpression: where);
                        }
                    }
                }
            }
        }

        private void SyncSamplerState(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND StratumCode = @StratumCode AND SampleGroupCode = @SampleGroupCode AND DeviceID = @DeviceID";

            var sampleGroups = destination.From<SampleGroup>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);
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
                        if (options.SamplerState.HasFlag(SyncFlags.Insert))
                        {
                            destination.Insert(i, persistKeyvalue: false);
                        }
                    }
                    else
                    {
                        var sMod = i.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, options.SamplerState))
                        {
                            destination.Update(i, whereExpression: where);
                        }
                    }
                }
            }
        }

        private void SyncSpecies(string cruiseID, DbConnection source, DbConnection desination, CruiseSyncOptions options)
        {
            var sourceItems = source.From<Species>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var i in sourceItems)
            {
                var hasMatch = desination.From<Species>().Where("CruiseID = @CruiseID AND SpeciesCode = @SpeciesCode").Count2(i) > 0;

                if (hasMatch == false)
                {
                    desination.Insert(i, persistKeyvalue: false);
                }
            }
        }

        private void SyncSubPopulation(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND StratumCode = @StratumCode AND SampleGroupCode = @SampleGroupCode AND SpeciesCode = @SpeciesCode AND LiveDead = @LiveDead";

            var sampleGroups = destination.From<SampleGroup>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

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

                        if (options.Design.HasFlag(SyncFlags.ForceInsert)
                            || (hasTombstone == false && options.Design.HasFlag(SyncFlags.Insert)))
                        {
                            destination.Insert(i, persistKeyvalue: false);
                        }
                    }
                }
            }
        }

        private void SyncFixCNTTallyPopulation(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND StratumCode = @StratumCode AND SampleGroupCode = @SampleGroupCode AND SpeciesCode = @SpeciesCode AND LiveDead = @LiveDead";

            var sampleGroups = destination.From<SampleGroup>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

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
                        if (options.Design.HasFlag(SyncFlags.Insert))
                        {
                            destination.Insert(i, persistKeyvalue: false);
                        }
                    }
                    else
                    {
                        var sMod = i.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, options.Design))
                        {
                            destination.Update(i, whereExpression: where);
                        }
                    }
                }
            }
        }

        private void SyncPlots(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND PlotID = @PlotID";

            var unitCodes = destination.QueryScalar<string>("SELECT CuttingUnitCode FROM CuttingUnit WHERE CruiseID = @p1;", new[] { cruiseID });
            foreach (var unitCode in unitCodes)
            {
                var plots = source.From<Plot>()
                    .Where("CuttingUnitCode = @p1 AND CruiseID = @p2")
                    .Query(unitCode, cruiseID);

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

                        if (options.FieldData.HasFlag(SyncFlags.ForceInsert)
                            || (hasTombstone == false && options.FieldData.HasFlag(SyncFlags.Insert)))
                        {
                            destination.Insert(plot, persistKeyvalue: false);
                        }
                    }
                    else
                    {
                        var sMod = plot.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, options.FieldData))
                        {
                            destination.Update(plot, whereExpression: where);
                        }
                    }
                }
            }
        }

        private void SyncPlotLocation(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "PlotID = @PlotID";

            var plotIDs = destination.QueryScalar<string>("SELECT PlotID FROM Plot WHERE CruiseID = @p1", new[] { cruiseID });
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

                        if (hasTombstone == false)
                        {
                            if (options.FieldData.HasFlag(SyncFlags.Insert))
                            {
                                destination.Insert(item, persistKeyvalue: false);
                            }
                        }
                    }
                    else
                    {
                        var sMod = item.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, options.FieldData))
                        {
                            destination.Update(item, whereExpression: where);
                        }
                    }
                }
            }
        }

        private void SyncPlot_Strata(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND CuttingUnitCode = @CuttingUnitCode AND PlotNumber = @PlotNumber AND StratumCode = @StratumCode";

            var plotIDs = destination.QueryScalar<string>("SELECT PlotID FROM Plot WHERE CruiseID = @p1;", paramaters: new[] { cruiseID });
            foreach (var plot in plotIDs)
            {
                var sourceItems = source.From<Plot_Stratum>()
                    .Join("main.Plot", "USING (CruiseID, CuttingUnitCode, PlotNumber)")
                    .Where("PlotID = @p1").Query(plot);

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

                        if (options.FieldData.HasFlag(SyncFlags.ForceInsert)
                            || (hasTombstone == false && options.FieldData.HasFlag(SyncFlags.Insert)))
                        {
                            destination.Insert(i, persistKeyvalue: false);
                        }
                    }
                    else
                    {
                        var sMod = i.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, options.FieldData))
                        {
                            destination.Update(i, whereExpression: where);
                        }
                    }
                }
            }
        }

        private void SyncPlotTree(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var syncFlags = options.TreeFlags;
            var allOrNone = options.PlotTreeAllOrNone;

            // read unit codes from the destination because we only should care about syncing units that have already been merged
            // incase we decided not to merge a unit earlier in the sync process
            var cuttingUnitCodes = destination.QueryScalar<string>("SELECT CuttingUnitCode FROM main.CuttingUnit WHERE CruiseID = @p1;", new[] { cruiseID });

            //var plots = destination.From<Plot>().Query().ToArray();

            foreach (var unitCode in cuttingUnitCodes)
            {
                var plotNumbers = destination.QueryScalar<int>("SELECT PlotNumber FROM Plot WHERE CuttingUnitCode = @p1 AND CruiseID = @p2;", new[] { unitCode, cruiseID }).ToArray();

                foreach (var plotNumber in plotNumbers)
                {
                    var destPlotMod = GetPlotTreeModified(destination, cruiseID, unitCode, plotNumber);

                    var srcPlotMod = GetPlotTreeModified(source, cruiseID, unitCode, plotNumber);

                    if (allOrNone == false
    || (srcPlotMod.CompareTo(destPlotMod) > 0))
                    {
                        SyncPlotTreeMeasurmentData(source, destination, cruiseID, unitCode, plotNumber, options);
                        SyncPlotTallyLedger(source, destination, cruiseID, unitCode, plotNumber, options);
                    }
                }
            }
        }

        public static DateTime GetPlotTreeModified(DbConnection db, string cruiseID, string unitCode, int plotNumber)
        {
            return db.ExecuteScalar<DateTime>("SELECT  max( max(Created_TS), max(Modified_TS)) FROM Tree WHERE CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber = @p3",
                        new object[] { cruiseID, unitCode, plotNumber });
        }

        private void SyncPlotTreeMeasurmentData(DbConnection source, DbConnection destination, string cruiseID, string unitCode, int plotNumber, CruiseSyncOptions options)
        {
            const string where = "CruiseID = @CruiseID AND TreeID = @TreeID";

            var syncFlags = options.TreeFlags;

            var sourceTrees = source.From<Tree>().Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber = @p3").Query(cruiseID, unitCode, plotNumber).ToArray();

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

                    if (syncFlags.HasFlag(SyncFlags.ForceInsert)
                            || (hasTombstone == false && syncFlags.HasFlag(SyncFlags.Insert)))
                    {
                        destination.Insert(tree, persistKeyvalue: false);

                        // only sync tree data if we have synced the new tree record
                        SyncTreeData(source, destination, tree.TreeID, options);
                    }
                }
                else
                {
                    var sMod = tree.Modified_TS;
                    var dMod = match.Modified_TS;

                    if (ShouldUpdate(sMod, dMod, syncFlags))
                    {
                        destination.Update(match, whereExpression: where);
                    }

                    SyncTreeData(source, destination, tree.TreeID, options);
                }
            }
        }

        private void SyncNonPlotTrees(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND TreeID = @TreeID";

            var cuttingUnitCodes = destination.QueryScalar<string>("SELECT CuttingUnitCode FROM main.CuttingUnit WHERE CruiseID = @p1;", new[] { cruiseID });

            foreach (var unit in cuttingUnitCodes)
            {
                var sourceTrees = source.From<Tree>().Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber IS NULL").Query(cruiseID, unit);

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

                        if (options.FieldData.HasFlag(SyncFlags.ForceInsert)
                            || (hasTombstone == false && options.FieldData.HasFlag(SyncFlags.Insert)))
                        {
                            destination.Insert(i, persistKeyvalue: false);

                            SyncTreeData(source, destination, i.TreeID, options);
                        }
                    }
                    else
                    {
                        var sMod = i.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, options.FieldData))
                        {
                            destination.Update(i, whereExpression: where);
                        }

                        SyncTreeData(source, destination, i.TreeID, options);
                    }
                }
            }
        }

        private void SyncTreeData(DbConnection source, DbConnection destination, string treeID, CruiseSyncOptions options)
        {
            // we are not checking the tombstone tables for TreeMeasurment or TreeFieldValue because at this
            // point we should already know that the tree has not be deleted.
            // we are assuming that TreeMeasurment or TreeFieldValue records wont be deleted unless the tree has been deleted
            // however that might change for TreeFieldValue records

            var srcLatestTimeStamp = GetTreeDataModified(source, treeID);
            var destLatestTimeStamp = GetTreeDataModified(destination, treeID);

            // we are comparing the agregate time stamps, per tree, across the files.
            // This means changes to tree data in any of the tables that contain tree data
            // can cause the given tree record to be updated in the sync.

            if (srcLatestTimeStamp != null
                && (destLatestTimeStamp == null || srcLatestTimeStamp.Value.CompareTo(destLatestTimeStamp) > 0))
            {
                var sourceMeasurmentsRecord = source.From<TreeMeasurment>().Where("TreeID = @p1").Query(treeID).FirstOrDefault();
                if (sourceMeasurmentsRecord != null)
                {
                    var hasMeasurmentsRecord = destination.ExecuteScalar<long>("SELECT count(*) FROM TreeMeasurment WHERE TreeID =  @p1;", parameters: new[] { treeID }) > 0;

                    if (hasMeasurmentsRecord)
                    {
                        destination.Update(sourceMeasurmentsRecord, whereExpression: "WHERE TreeID =  @TreeID");
                    }
                    else
                    {
                        destination.Insert(sourceMeasurmentsRecord, persistKeyvalue: false);
                    }
                }

                SyncTreeLocation(source, destination, treeID, options);

                SyncTreeFieldValue(source, destination, treeID, options);
            }
        }

        public static DateTime? GetTreeDataModified(DbConnection db, string treeID)
        {
            // get the latest time stamp on the tree by agregating the Created Timestamp and Modified Timestamps
            // from both the TreeMeasurment record and the TreeFieldValues records.
            return db.ExecuteScalar<DateTime>(
@" SELECT max(mod) FROM (
        SELECT max( tm.Created_TS, tm.Modified_TS) AS mod FROM TreeMeasurment AS tm WHERE tm.TreeID = @p1
        UNION ALL
        SELECT max( tfv.Created_TS, tfv.Modified_TS) AS mod FROM TreeFieldValue AS tfv WHERE tfv.TreeID = @p1
        UNION ALL
        SELECT max( tl.Created_TS, tl.Modified_TS) AS mod FROM TreeLocation AS tl WHERE tl.TreeID = @p1
);", parameters: new[] { treeID });
        }

        private void SyncTreeLocation(DbConnection source, DbConnection destination, string treeID, CruiseSyncOptions options)
        {
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

                    if (options.FieldData.HasFlag(SyncFlags.ForceInsert)
                            || (hasTombstone == false && options.FieldData.HasFlag(SyncFlags.Insert)))
                    {
                        destination.Insert(treeLocationRecord, persistKeyvalue: false);
                    }
                }
                else
                {
                    var sMod = treeLocationRecord.Modified_TS;
                    var dMod = match.Modified_TS;

                    if (ShouldUpdate(sMod, dMod, options.FieldData))
                    {
                        destination.Update(treeLocationRecord, whereExpression: "TreeID = @TreeID");
                    }
                }
            }
        }

        private void SyncTreeFieldValue(DbConnection source, DbConnection destination, string treeID, CruiseSyncOptions options)
        {
            var treeFieldValues = source.From<TreeFieldValue>().Where("TreeID =  @p1").Query(treeID);
            foreach (var tfv in treeFieldValues)
            {
                var hasTFVRecord = destination.From<TreeFieldValue>().Where("TreeID = @TreeID AND Field = @Field").Count2(tfv) > 0;
                if (hasTFVRecord)
                {
                    destination.Update(tfv, whereExpression: "TreeID = TreeID AND Field = @Field");
                }
                else
                {
                    destination.Insert(tfv, persistKeyvalue: false);
                }
            }
        }

        private void SyncPlotTallyLedger(DbConnection source, DbConnection destination, string cruiseID, string unitCode, int plotNumber, CruiseSyncOptions options)
        {
            var syncFlags = options.TreeFlags;

            var sourceTallyLedgers = source.From<TallyLedger>()
                .Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber = @p3")
                .Query(cruiseID, unitCode, plotNumber);

            foreach (var tl in sourceTallyLedgers)
            {
                var hasRecord = destination.ExecuteScalar<long>("SELECT count(*) FROM TallyLedger WHERE TallyLedgerID = @p1;", parameters: new[] { tl.TallyLedgerID }) > 0;
                var hasTombstone = destination.ExecuteScalar<long>("SELECT count(*) FROM TallyLedger_Tombstone WHERE TallyLedgerID = @p1;", parameters: new[] { tl.TallyLedgerID }) > 0;

                if (hasRecord == false && hasTombstone == false)
                {
                    destination.Insert(tl, persistKeyvalue: false);
                }
            }
        }

        private void SyncTreeMeasurment(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND TreeID = @TreeID";

            var sourceItems = source.From<TreeMeasurment>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<TreeMeasurment>()
                    .Where(where)
                    .Query2(i)
                    .FirstOrDefault();

                if (match == null)
                {
                    var hasTombstone = destination.From<TreeMeasurment_Tombstone>()
                        .Where(where)
                        .Count2(i) > 0;

                    if (options.FieldData.HasFlag(SyncFlags.ForceInsert)
                            || (hasTombstone == false && options.FieldData.HasFlag(SyncFlags.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                    }
                }
                else
                {
                    var sMod = i.Modified_TS;
                    var dMod = match.Modified_TS;

                    if (ShouldUpdate(sMod, dMod, options.FieldData))
                    {
                        if (options.FieldData.HasFlag(SyncFlags.Update))
                        {
                            destination.Update(i, whereExpression: where);
                        }
                    }
                }
            }
        }

        private void SyncLog(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "TreeID = @TreeID AND LogNumber = @LogNumber";

            // only sync for trees that are already in the destination
            // just incase we decide earlier not to sync some trees
            var treeIDs = destination.QueryScalar<string>("SELECT TreeID FROM Tree WHERE CruiseID = @p1", new[] { cruiseID });
            foreach (var treeID in treeIDs)
            {
                var logs = source.From<Log>()
                    .Where("TreeID = @p1")
                    .Query(treeID);
                foreach (var log in logs)
                {
                    var match = destination.From<Log>()
                        .Where(where)
                        .Query2(log)
                        .FirstOrDefault();

                    if (match == null)
                    {
                        var hasTombstone = destination.From<Log_Tombstone>().Where(where).Count2(log) > 0;

                        if (options.FieldData.HasFlag(SyncFlags.ForceInsert)
                            || (hasTombstone == false && options.FieldData.HasFlag(SyncFlags.Insert)))
                        {
                            destination.Insert(log, persistKeyvalue: false);
                        }
                    }
                    else
                    {
                        var sMod = log.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (ShouldUpdate(sMod, dMod, options.FieldData))
                        {
                            if (options.FieldData.HasFlag(SyncFlags.Update))
                            {
                                destination.Update(log, whereExpression: where);
                            }
                        }
                    }
                }
            }
        }

        private void SyncStem(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
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
                            if (options.FieldData.HasFlag(SyncFlags.ForceInsert)
                            || (hasTombstone == false && options.FieldData.HasFlag(SyncFlags.Insert)))
                            {
                                destination.Insert(i, persistKeyvalue: false);
                            }
                        }
                        else
                        {
                            var sMod = i.Modified_TS;
                            var dMod = match.Modified_TS;

                            if (ShouldUpdate(sMod, dMod, options.FieldData))
                            {
                                destination.Update(i, whereExpression: where);
                            }
                        }
                    }
                }
            }
        }

        private void SyncTallyLedger(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND TallyLedgerID = @TallyLedgerID";

            var sourceItems = source.From<TallyLedger>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<TallyLedger>()
                    .Where(where)
                    .Query2(i)
                    .FirstOrDefault();

                if (match == null)
                {
                    var hasTombstone = destination.From<TallyLedger_Tombstone>().Where(where).Count2(i) > 0;

                    if (options.FieldData.HasFlag(SyncFlags.ForceInsert)
                            || (hasTombstone == false && options.FieldData.HasFlag(SyncFlags.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                    }
                }
            }
        }

        private void SyncLogFieldSetup(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND StratumCode = @StratumCode AND Field = @Field";

            var sourceItems = source.From<LogFieldSetup>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<LogFieldSetup>()
                    .Where(where)
                    .Query2(i)
                    .FirstOrDefault();

                if (match == null)
                {
                    var hasTombstone = destination.From<LogFieldSetup_Tombstone>().Where(where).Count2(i) > 0;

                    if (options.Design.HasFlag(SyncFlags.ForceInsert)
                            || (hasTombstone == false && options.Design.HasFlag(SyncFlags.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                    }
                }
            }
        }

        private void SyncTreeFieldSetup(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
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

                    if (options.Design.HasFlag(SyncFlags.ForceInsert)
                            || (hasTombstone == false && options.Design.HasFlag(SyncFlags.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                    }
                }
            }
        }

        private void SyncTreeAuditRule(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
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

                    if (options.Validation.HasFlag(SyncFlags.ForceInsert)
                            || (hasTombstone == false && options.Validation.HasFlag(SyncFlags.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                    }
                }
                // update not supported
            }
        }

        private void SyncTreeAuditRuleSelector(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
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

                    if (options.Validation.HasFlag(SyncFlags.ForceInsert)
                            || (hasTombstone == false && options.Validation.HasFlag(SyncFlags.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                    }
                }
                // update not supported
            }
        }

        private void SyncTreeAuditResolution(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "TreeAuditRuleID = @TreeAuditRuleID AND TreeID = @TreeID";

            var sourceItems = source.From<TreeAuditResolution>().Where("CruiseID = @p1").Query(cruiseID);
            foreach (var i in sourceItems)
            {
                var match = destination.From<TreeAuditResolution>()
                    .Where(where).Query2(i);

                // it is possible that the match doesn't have the same initials, or resolution values
                // but I think it is save to ignore conflicts in this situation just as long as there is a resolution

                if (match == null)
                {
                    var hasTombstone = destination.From<TreeAuditResolution_Tombstone>()
                        .Where(where).Count2(i) > 0;

                    if (options.Validation.HasFlag(SyncFlags.ForceInsert)
                                || (hasTombstone == false && options.Validation.HasFlag(SyncFlags.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                    }
                }
                // update not supported
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

        private void SyncTreeFieldHeading(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND Field = @Field";
            var sourceItems = source.From<TreeFieldHeading>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<TreeFieldHeading>().Where(where).Query2(i).FirstOrDefault();

                if (match == null)
                {
                    destination.Insert(i, persistKeyvalue: false);
                }
                else
                {
                    if (ShouldUpdate(i.Modified_TS, match.Modified_TS, options.Design))
                    {
                        destination.Update(i, whereExpression: where);
                    }
                }
            }
        }

        private void SyncLogFieldHeading(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND Field = @Field";
            var sourceItems = source.From<LogFieldHeading>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<LogFieldHeading>()
                    .Where(where).Query2(i).FirstOrDefault();

                if (match == null)
                {
                    destination.Insert(i, persistKeyvalue: false);
                }
                else
                {
                    if (ShouldUpdate(i.Modified_TS, match.Modified_TS, options.Design))
                    {
                        destination.Update(i, whereExpression: where);
                    }
                }
            }
        }

        private void SyncBiomassEquations(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND Species = @Species AND Product = @Product AND Component = @Component AND LiveDead = @LiveDead";
            var sourceItems = source.From<BiomassEquation>()
                .Where("CruiseID = @p1").Query(cruiseID);

            foreach(var i in sourceItems)
            {
                var match = destination.From<BiomassEquation>()
                    .Where(where).Query2(i).FirstOrDefault();

                if(match == null)
                {
                    destination.Insert(i, persistKeyvalue: false);
                }
                else
                {
                    if (ShouldUpdate(i.Modified_TS, match.Modified_TS, options.Processing))
                    {
                        destination.Update(i, whereExpression: where);
                    }
                }
            }
        }

        private void SyncReports(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND ReportID = @ReportID";
            var sourceItems = source.From<Reports>()
                .Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<Reports>()
                    .Where(where).Query2(i).FirstOrDefault();

                if (match == null)
                {
                    destination.Insert(i, persistKeyvalue: false);
                }
                else
                {
                    if (ShouldUpdate(i.Modified_TS, match.Modified_TS, options.Processing))
                    {
                        destination.Update(i, whereExpression: where);
                    }
                }
            }
        }

        private void SyncValueEquations(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND Species = @Species AND PrimaryProduct = @PrimaryProduct AND ValueEquationNumber = @ValueEquationNumber";
            var sourceItems = source.From<ValueEquation>()
                .Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<ValueEquation>()
                    .Where(where).Query2(i).FirstOrDefault();

                if (match == null)
                {
                    destination.Insert(i, persistKeyvalue: false);
                }
                else
                {
                    if (ShouldUpdate(i.Modified_TS, match.Modified_TS, options.Processing))
                    {
                        destination.Update(i, whereExpression: where);
                    }
                }
            }
        }

        private void SyncVolumeEquations(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND Species = @Species AND PrimaryProduct =  @PrimaryProduct AND VolumeEquationNumber = @VolumeEquationNumber";
            var sourceItems = source.From<VolumeEquation>()
                .Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<VolumeEquation>().Where(where).Query2(i).FirstOrDefault();

                if (match == null)
                {
                    destination.Insert(i, persistKeyvalue: false);
                }
                else
                {
                    if (ShouldUpdate(i.Modified_TS, match.Modified_TS, options.Processing))
                    {
                        destination.Update(i, whereExpression: where);
                    }
                }
            }
        }

        private void SyncTreeDefaultValues(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND coalesce(SpeciesCode, '') = coalesce(@SpeciesCode, '') AND coalesce(PrimaryProduct, '') = coalesce(@PrimaryProduct, '')";
            var sourceItems = source.From<TreeDefaultValue>()
                .Where("CruiseID = @p1").Query(cruiseID);

            foreach(var i in sourceItems)
            {
                var match = destination.From<TreeDefaultValue>().Where(where).Query2(i).FirstOrDefault();

                if(match == null)
                {
                    var hasTombstone = destination.From<TreeDefaultValue_Tombstone>()
                                                    .Where(where).Count2(i) > 0;

                    if (options.TreeDefaultValue.HasFlag(SyncFlags.ForceInsert)
                                || (hasTombstone == false && options.TreeDefaultValue.HasFlag(SyncFlags.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                    }
                }
                else
                {
                    if (ShouldUpdate(i.Modified_TS, match.Modified_TS, options.TreeDefaultValue))
                    {
                        destination.Update(i, whereExpression: where);
                    }
                }
            }
        }

        private void SyncStratumTemplates(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND StratumTemplateName = @StratumTemplateName";
            var sourceItems = source.From<StratumTemplate>()
                .Where("CruiseID = @p1").Query(cruiseID);

            foreach(var i in sourceItems)
            {
                var match = destination.From<StratumTemplate>().Where(where).Query2(i).FirstOrDefault();

                if(match == null)
                {
                    var hasTombstone = destination.From<StratumTemplate_Tombstone>()
                                                    .Where(where).Count2(i) > 0;

                    if (options.Template.HasFlag(SyncFlags.ForceInsert)
                                || (hasTombstone == false && options.Template.HasFlag(SyncFlags.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                    }
                }
                else
                {
                    if (ShouldUpdate(i.Modified_TS, match.Modified_TS, options.Template))
                    {
                        destination.Update(i, whereExpression: where);
                    }
                }
            }
        }

        private void SyncStratumTemplateTreeFieldSetups(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
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

                    if (options.Template.HasFlag(SyncFlags.ForceInsert)
                                || (hasTombstone == false && options.Template.HasFlag(SyncFlags.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                    }
                }
                else
                {
                    if (ShouldUpdate(i.Modified_TS, match.Modified_TS, options.Template))
                    {
                        destination.Update(i, whereExpression: where);
                    }
                }
            }
        }

        private void SyncStratumTemplateLogFieldSetups(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
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

                    if (options.Template.HasFlag(SyncFlags.ForceInsert)
                                || (hasTombstone == false && options.Template.HasFlag(SyncFlags.Insert)))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                    }
                }
                else
                {
                    if (ShouldUpdate(i.Modified_TS, match.Modified_TS, options.Template))
                    {
                        destination.Update(i, whereExpression: where);
                    }
                }
            }
        }
    }

    public class TreeID_Modified_TS
    {
        public string TreeID { get; set; }

        public DateTime Modified_TS { get; set; }
    }
}