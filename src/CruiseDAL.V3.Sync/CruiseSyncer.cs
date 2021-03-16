using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using FMSC.ORM.Logging;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace CruiseDAL.V3.Sync
{
    public class CruiseSyncer : IDbSyncer
    {
        private ILogger _logger;
        private ILogger Logger => _logger ?? (_logger = LoggerProvider.Get());

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
            var steps = 23;
            float p = 0.0f;
            var transaction = destination.BeginTransaction();
            try
            {
                
                SyncSale(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncCruise(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                SyncDevice(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

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

                SyncLogFieldSetup(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncTreeFieldSetup(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                SyncTreeAuditRule(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

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

                SyncTreeFieldHeading(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                transaction.Commit();
            }
            catch(Exception e)
            {
                transaction.Rollback();
                throw;
            }
        }

        

        private void SyncSale(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var sourceSale = source.From<Sale>()
                .Join("Cruise AS c", "USING (SaleID)")
                .Where("CruiseID = @p1")
                .Query(cruiseID).FirstOrDefault();

            var hasMatch = destination.ExecuteScalar<bool>("SELECT EXISTS (SELECT * FROM Sale WHERE SaleID = @p1);", new[] { sourceSale.SaleID });
            if(!hasMatch)
            {
                destination.Insert(sourceSale, persistKeyvalue: false);
            }
            else
            {

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

                if (sMod.HasValue == false) { return; }
                if ((sMod.HasValue == true && dMod.HasValue == false)
                    || DateTime.Compare(sMod.Value, dMod.Value) > 0)
                {
                    if (options.Design.HasFlag(SyncFlags.Update))
                    {
                        destination.Update(sCruise, whereExpression: where);
                    }
                }
            }
        }

        void SyncDevice(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var sourceItems = source.From<Device>().Where("CruiseID = @p1").Query(cruiseID);

            foreach(var i in sourceItems)
            {
                var hasMatch = destination.ExecuteScalar<long>("SELECT count(*) FROM Device WHERE DeviceID = @p1 AND CruiseID = @p2;", new object[] { i.DeviceID, cruiseID }) > 0;

                if(hasMatch == false)
                {
                    destination.Insert(i, persistKeyvalue: false);
                }
            }
        }

        private void SyncCuttingUnits(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND CuttingUnitCode =  @CuttingUnitCode";
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

                    if (hasTombstone == false)
                    {
                        if (options.Design.HasFlag(SyncFlags.Insert))
                        {
                            destination.Insert(i, persistKeyvalue: false);
                        }
                    }
                }
                else
                {
                    var sMod = i.Modified_TS;
                    var matchMod = match.Modified_TS;

                    if (sMod.HasValue == false) { continue; }
                    if ((sMod.HasValue && matchMod.HasValue == false)
                        || DateTime.Compare(sMod.Value, matchMod.Value) > 0)
                    {
                        if (options.Design.HasFlag(SyncFlags.Update))
                        {
                            destination.Update(i, whereExpression: where);
                        }
                    }
                }
            }
        }

        void SyncCuttingUnit_Stratum(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CuttingUnitCode = @CuttingUnitCode AND StratumCode = @StratumCode";
            var sourceItems = source.From<CuttingUnit_Stratum>().Where("CruiseID = @p1").Query(cruiseID);
            foreach(var i in sourceItems)
            {
                var hasItem = destination.From<CuttingUnit_Stratum>().Where(where).Count2(i) > 0;
                var hasTombstone = destination.From<CuttingUnit_Stratum>().Where(where).Count2(i) > 0;

                if(hasItem == false)
                {
                    if(hasTombstone == false)
                    {
                        destination.Insert(i, persistKeyvalue: false);
                    }
                }
            }
        }

        private void SyncStrata(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND StratumCode = @StratumCode";

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

                    if (hasTombstone == false)
                    {
                        if (options.Design.HasFlag(SyncFlags.Insert))
                        {
                            destination.Insert(i, persistKeyvalue: false);
                        }
                    }
                }
                else
                {
                    var sMod = i.Modified_TS;
                    var dMod = match.Modified_TS;

                    if (sMod.HasValue == false) { continue; }
                    if ((sMod.HasValue && dMod.HasValue == false)
                        || DateTime.Compare(sMod.Value, dMod.Value) > 0)
                    {
                        if (options.Design.HasFlag(SyncFlags.Update))
                        {
                            destination.Update(i, whereExpression: where);
                        }
                    }
                }
            }
        }

        private void SyncSampleGroup(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND StratumCode = @StratumCode AND SampleGroupCode = @SampleGroupCode";

            var sourceItems = source.From<SampleGroup>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<SampleGroup>()
                    .Where(where)
                    .Query2(i)
                    .FirstOrDefault();

                if (match == null)
                {
                    var hasTombstone = destination.From<SampleGroup_Tombstone>()
                        .Where(where)
                        .Count2(i) > 0;

                    if (hasTombstone == false)
                    {
                        if (options.Design.HasFlag(SyncFlags.Insert))
                        {
                            destination.Insert(i, persistKeyvalue: false);
                        }
                    }
                }
                else
                {
                    var sMod = i.Modified_TS;
                    var dMod = match.Modified_TS;

                    if (sMod.HasValue == false) { continue; }
                    if ((sMod.HasValue && dMod.HasValue == false)
                        || DateTime.Compare(sMod.Value, dMod.Value) > 0)
                    {
                        if (options.Design.HasFlag(SyncFlags.Update))
                        {
                            destination.Update(i, whereExpression: where);
                        }
                    }
                }
            }
        }

        private void SyncSamplerState(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND StratumCode = @StratumCode AND SampleGroupCode = @SampleGroupCode AND DeviceID = @DeviceID";

            var sourceItems = source.From<SamplerState>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
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

                    if (sMod.HasValue == false) { continue; }
                    if ((sMod.HasValue && dMod.HasValue == false)
                        || DateTime.Compare(sMod.Value, dMod.Value) > 0)
                    {
                        if (options.SamplerState.HasFlag(SyncFlags.Update))
                        {
                            destination.Update(i, whereExpression: where);
                        }
                    }
                }
            }
        }

        void SyncSpecies(string cruiseID, DbConnection source, DbConnection desination, CruiseSyncOptions options)
        {
            var sourceItems = source.From<Species>().Where("CruiseID = @p1").Query(cruiseID);
            foreach(var i in sourceItems)
            {
                var hasMatch = desination.From<Species>().Where("CruiseID = @CruiseID AND SpeciesCode = @SpeciesCode").Count2(i) > 0;

                if(hasMatch == false)
                {
                    desination.Insert(i, persistKeyvalue: false);
                }
            }
        }

        void SyncSubPopulation(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND StratumCode = @StratumCode AND SampleGroupCode = @SampleGroupCode AND SpeciesCode = @SpeciesCode AND LiveDead = @LiveDead";

            var sourceItems = source.From<SubPopulation>().Where("CruiseID = @p1").Query(cruiseID);

            foreach(var i in sourceItems)
            {
                var match = destination.From<SubPopulation>()
                    .Where(where)
                    .Query2(i)
                    .FirstOrDefault();

                if(match == null)
                {
                    var hasTombstone = destination.From<SubPopulation_Tombstone>().Where(where).Count2(i) > 0;

                    if (hasTombstone == false && options.Design.HasFlag(SyncFlags.Insert))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                    }
                }
            }
        }

        void SyncFixCNTTallyPopulation(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND StratumCode = @StratumCode AND SampleGroupCode = @SampleGroupCode AND Species = @Species AND LiveDead = @LiveDead";

            var sourceItems = source.From<FixCNTTallyPopulation>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<FixCNTTallyPopulation>()
                    .Where(where)
                    .Query2(i)
                    .FirstOrDefault();

                if (match == null)
                {
                    destination.Insert(i, persistKeyvalue: false);
                }
                else
                {
                    var sMod = i.Modified_TS;
                    var dMod = match.Modified_TS;

                    if (sMod.HasValue == false) { continue; }
                    if ((sMod.HasValue && dMod.HasValue == false)
                        || DateTime.Compare(sMod.Value, dMod.Value) > 0)
                    {
                        destination.Update(i, whereExpression: where);
                    }
                }
            }
        }

        private void SyncPlots(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND PlotID = @PlotID";

            var sourceItems = source.From<Plot>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<Plot>()
                    .Where(where)
                    .Query2(i)
                    .FirstOrDefault();

                if (match == null)
                {
                    var hasTombstone = destination.From<Plot>()
                        .Where(where)
                        .Count2(i) > 0;

                    if (hasTombstone == false)
                    {
                        if (options.FieldData.HasFlag(SyncFlags.Insert))
                        {
                            destination.Insert(i, persistKeyvalue: false);
                        }
                    }
                }
                else
                {
                    var sMod = i.Modified_TS;
                    var dMod = match.Modified_TS;

                    if (sMod.HasValue == false) { continue; }
                    if ((sMod.HasValue && dMod.HasValue == false)
                        || DateTime.Compare(sMod.Value, dMod.Value) > 0)
                    {
                        if (options.FieldData.HasFlag(SyncFlags.Update))
                        {
                            destination.Update(i, whereExpression: where);
                        }
                    }
                }
            }
        }

        private void SyncPlotLocation(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND PlotID = @PlotID";

            var plotIDs = destination.QueryScalar<string>("SELECT PlotID FROM Plot WHERE CruiseID = @p1;", paramaters: new[] { cruiseID });
            foreach (var plot in plotIDs)
            {
                var item = source.From<PlotLocation>()
                    .Where("CruiseID = @p1 AND PlotID = @p2")
                    .Query(cruiseID, plot)
                    .FirstOrDefault();

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

                    if (sMod.HasValue == false) { continue; }
                    if ((sMod.HasValue && dMod.HasValue == false)
                        || DateTime.Compare(sMod.Value, dMod.Value) > 0)
                    {
                        if (options.FieldData.HasFlag(SyncFlags.Update))
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
                var sourceItems = source.From<Plot_Stratum>().Where("CruiseID = @p1 AND PlotID = @p2").Query(cruiseID, plot);

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

                        if (hasTombstone == false)
                        {
                            if (options.FieldData.HasFlag(SyncFlags.Insert))
                            {
                                destination.Insert(i, persistKeyvalue: false);
                            }
                        }
                    }
                    else
                    {
                        var sMod = i.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (sMod.HasValue == false) { continue; }
                        if ((sMod.HasValue && dMod.HasValue == false)
                            || DateTime.Compare(sMod.Value, dMod.Value) > 0)
                        {
                            if (options.FieldData.HasFlag(SyncFlags.Update))
                            {
                                destination.Update(i, whereExpression: where);
                            }
                        }
                    }
                }
            }
        }

        private void SyncPlotTree(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var syncFlags = options.TreeFlags;
            var allOrNone = options.PlotTreeAllOrNone;

            var cuttingUnitCodes = destination.QueryScalar<string>("SELECT CuttingUnitCode FROM main.CuttingUnit WHERE CruiseID = @p1;", new[] { cruiseID });

            foreach (var unitCode in cuttingUnitCodes)
            {
                var plotNumbers = destination.QueryScalar<int>("SELECT PlotNumber FROM main.Plot WHERE CuttingUnitCode = @p1 AND CruiseID = @p2;", new[] { cruiseID, unitCode });

                foreach (var plotNumber in plotNumbers)
                {
                    var destPlotMod = CruiseAnalizer.GetPlotTreeModified(destination, cruiseID, unitCode, plotNumber);

                    var srcPlotMod = CruiseAnalizer.GetPlotTreeModified(source, cruiseID, unitCode, plotNumber);

                    if (allOrNone == false
    || (srcPlotMod.CompareTo(destPlotMod) > 0))
                    {
                        SyncPlotTreeMeasurmentData(source, destination, cruiseID, unitCode, plotNumber, options);
                        SyncPlotTallyLedger(source, destination, cruiseID, unitCode, plotNumber, options);
                    }
                }
            }
        }

        private void SyncPlotTreeMeasurmentData(DbConnection source, DbConnection destination, string cruiseID, string unitCode, int plotNumber, CruiseSyncOptions options)
        {
            const string where = "CruiseID = @CruiseID AND TreeID = @TreeID";

            var syncFlags = options.TreeFlags;

            var sourceTrees = source.From<Tree>().Where("CruiseID = @p1 AND CuttingUnitCode = @p2 AND PlotNumber = @p3").Query(cruiseID, unitCode, plotNumber);

            foreach (var tree in sourceTrees)
            {
                var match = destination.From<Tree>()
                    .Where(where)
                    .Query2(tree)
                    .FirstOrDefault();

                if (match == null)
                {
                    var hasTombstone = destination.From<Tree_Tombstone>()
                        .Where(where)
                        .Count2(tree) > 0;

                    if (hasTombstone == false)
                    {
                        if (syncFlags.HasFlag(SyncFlags.Insert))
                        {
                            destination.Insert(tree, persistKeyvalue: false);
                        }
                    }
                }
                else
                {
                    var sMod = tree.Modified_TS;
                    var dMod = match.Modified_TS;

                    if (sMod.HasValue == false) { continue; }
                    if ((sMod.HasValue && dMod.HasValue == false)
                        || DateTime.Compare(sMod.Value, dMod.Value) > 0)
                    {
                        if (syncFlags.HasFlag(SyncFlags.Update))
                        {
                            destination.Update(match, whereExpression: where);
                        }
                    }
                }

                SyncTreeData(source, destination, tree.TreeID, options);
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

                        if (hasTombstone == false)
                        {
                            if (options.FieldData.HasFlag(SyncFlags.Insert))
                            {
                                destination.Insert(i, persistKeyvalue: false);
                            }
                        }
                    }
                    else
                    {
                        var sMod = i.Modified_TS;
                        var dMod = match.Modified_TS;

                        if (sMod.HasValue == false) { continue; }
                        if ((sMod.HasValue && dMod.HasValue == false)
                            || DateTime.Compare(sMod.Value, dMod.Value) > 0)
                        {
                            if (options.FieldData.HasFlag(SyncFlags.Update))
                            {
                                destination.Update(i, whereExpression: where);
                            }
                        }
                    }
                }
            }
        }

        private void SyncTreeData(DbConnection source, DbConnection destination, string treeID, CruiseSyncOptions options)
        {
            // we are not checking the tombstone tables for TreeMeasurments or TreeFieldValue because at this
            // point we should already know that the tree has not be deleted.
            // we are assuming that TreeMeasurment or TreeFieldValue records wont be deleted unless the tree has been deleted
            // however that might change for TreeFieldValue records

            var srcLatestTimeStamp = CruiseAnalizer.GetTreeDataModified(source, treeID);
            var destLatestTimeStamp = CruiseAnalizer.GetTreeDataModified(source, treeID);

            if (srcLatestTimeStamp != null
                && (destLatestTimeStamp == null || srcLatestTimeStamp.Value.CompareTo(destLatestTimeStamp) > 0))
            {
                var sourceMeasurmentsRecord = source.From<TreeMeasurment>().Where("TreeID = @p1").Query(treeID).FirstOrDefault();
                if (sourceMeasurmentsRecord != null)
                {
                    var hasMeasurmentsRecord = destination.ExecuteScalar<long>("SELECT count(*) FROM TreeMeasurments WHERE TreeID =  @p1;", parameters: new[] { treeID }) > 0;

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

                    if (hasTombstone == false)
                    {
                        if (options.FieldData.HasFlag(SyncFlags.Insert))
                        {
                            destination.Insert(treeLocationRecord, persistKeyvalue: false);
                        }
                    }
                }
                else
                {
                    var sMod = treeLocationRecord.Modified_TS;
                    var dMod = match.Modified_TS;

                    if (sMod.HasValue == false) { return; }
                    if ((sMod.HasValue && dMod.HasValue == false)
                        || DateTime.Compare(sMod.Value, dMod.Value) > 0)
                    {
                        if (options.FieldData.HasFlag(SyncFlags.Update))
                        {
                            destination.Update(treeLocationRecord, whereExpression: "TreeID = @TreeID");
                        }
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

                    if (hasTombstone == false)
                    {
                        if (options.FieldData.HasFlag(SyncFlags.Insert))
                        {
                            destination.Insert(i, persistKeyvalue: false);
                        }
                    }
                }
                else
                {
                    var sMod = i.Modified_TS;
                    var dMod = match.Modified_TS;

                    if (sMod.HasValue == false) { continue; }
                    if ((sMod.HasValue && dMod.HasValue == false)
                        || DateTime.Compare(sMod.Value, dMod.Value) > 0)
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
            var where = "CruiseID = @CruiseID AND TreeID = @TreeID AND LogNumber = @LogNumber";

            var sourceItems = source.From<Log>().Join("Tree", "USING (TreeID)").Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<Log>()
                    .Where(where)
                    .Query2(i)
                    .FirstOrDefault();

                if (match == null)
                {
                    var hasTombstone = destination.From<Log_Tombstone>().Where(where).Count2(i) > 0;

                    if (hasTombstone == false)
                    {
                        if (options.FieldData.HasFlag(SyncFlags.Insert))
                        {
                            destination.Insert(i, persistKeyvalue: false);
                        }
                    }
                }
                else
                {
                    var sMod = i.Modified_TS;
                    var dMod = match.Modified_TS;

                    if (sMod.HasValue == false) { continue; }
                    if ((sMod.HasValue && dMod.HasValue == false)
                        || DateTime.Compare(sMod.Value, dMod.Value) > 0)
                    {
                        if (options.FieldData.HasFlag(SyncFlags.Update))
                        {
                            destination.Update(i, whereExpression: where);
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
                            if (hasTombstone == false)
                            {
                                if (options.FieldData.HasFlag(SyncFlags.Insert))
                                {
                                    destination.Insert(i, persistKeyvalue: false);
                                }
                            }
                        }
                        else
                        {
                            var sMod = i.Modified_TS;
                            var dMod = match.Modified_TS;

                            if (allOrNone == false && sMod.HasValue == false) { continue; }
                            if ((sMod.HasValue && dMod.HasValue == false)
                                || DateTime.Compare(sMod.Value, dMod.Value) > 0)
                            {
                                if (options.FieldData.HasFlag(SyncFlags.Update))
                                {
                                    destination.Update(i, whereExpression: where);
                                }
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

                var hasTombstone = destination.From<TallyLedger_Tombstone>().Where(where).Count2(i) > 0;

                if (match == null && hasTombstone == false)
                {
                    if (options.FieldData.HasFlag(SyncFlags.Insert))
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

                var hasTombstone = destination.From<LogFieldSetup_Tombstone>().Where(where).Count2(i) > 0;
                if (match == null && hasTombstone == false)
                {
                    if (options.Design.HasFlag(SyncFlags.Insert))
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

                var hasTombstone = destination.From<TreeFieldSetup_Tombstone>().Where(where).Count2(i) > 0;
                if (match == null && hasTombstone == false)
                {
                    if (options.Design.HasFlag(SyncFlags.Insert))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                    }
                }
            }
        }

        private void SyncTreeAuditRule(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var where = "CruiseID = @CruiseID AND StratumCode = @StratumCode AND coalesce(SampleGroupCode, '') = coalesce(@SampleGroupCode, '') AND Field = @Field";

            var sourceItems = source.From<TreeFieldSetup>().Where("CruiseID = @p1").Query(cruiseID);

            foreach (var i in sourceItems)
            {
                var match = destination.From<TreeFieldSetup>()
                    .Where(where)
                    .Query2(i)
                    .FirstOrDefault();

                var hasTombstone = destination.From<TreeFieldSetup_Tombstone>().Where(where).Count2(i) > 0;
                if (match == null && hasTombstone == false)
                {
                    if (options.Design.HasFlag(SyncFlags.Insert))
                    {
                        destination.Insert(i, persistKeyvalue: false);
                    }
                }
            }
        }

        private void SyncTreeFieldHeading(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var sourceItems = source.From<TreeFieldHeading>().Where("CruiseID = @p1").Query(cruiseID);

            foreach(var i in sourceItems)
            {

            }
        }
    }

    public class TreeID_Modified_TS
    {
        public string TreeID { get; set; }

        public DateTime Modified_TS { get; set; }
    }
}