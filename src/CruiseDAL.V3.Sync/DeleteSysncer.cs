using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.V3.Sync
{
    public class DeleteSysncer
    {
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
            var steps = 16;
            float p = 0.0f;
            var transaction = destination.BeginTransaction();
            try
            {
                SyncCuttingUnits(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncStrata(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncCuttingUnit_Stratum(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncSampleGroup(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncSubPopulation(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                // field setup
                SyncLogFieldSetup(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncTreeFieldSetup(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                // validation
                SyncTreeAuditRule(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncTreeAuditRuleSelector(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncTreeAuditResolution(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                // field data
                SyncPlots(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncPlotLocation(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);
                SyncPlot_Strata(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                SyncTallyLedger(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                //SyncLog(cruiseID, source, destination, options);
                //progress?.Report(p++ / steps);
                //SyncStem(cruiseID, source, destination, options);
                //progress?.Report(p++ / steps);

                //processing
                SyncVolumeEquations(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                SyncReports(cruiseID, source, destination, options);
                progress?.Report(p++ / steps);

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw;
            }
        }

        

        private void SyncCuttingUnits(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<CuttingUnit_Tombstone>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

            foreach(var i in deletedItems)
            {
                var match = destination.From<CuttingUnit>()
                    .Where("CuttingUnitID = @CuttingUnitID")
                    .Query2(new { i.CuttingUnitID }).FirstOrDefault();

                if(match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM CuttingUnit WHERE CuttingUnitID = @CuttingUnitID;", new { i.CuttingUnitID });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }

        private void SyncStrata(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<Stratum_Tombstone>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

            foreach(var i in deletedItems)
            {
                var match = destination.From<Stratum>()
                    .Where("StratumID = @StratumID")
                    .Query2(new { i.StratumID }).FirstOrDefault();

                if(match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM Stratum WHERE StratumID = @StratumID;", new { i.StratumID });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }

        private void SyncCuttingUnit_Stratum(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<CuttingUnit_Stratum_Tombstone>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

            foreach (var i in deletedItems)
            {
                var match = destination.From<CuttingUnit_Stratum>()
                    .Where("CruiseID = @CruiseID AND CuttingUnitCode = @CuttingUnitCode AND StratumCode = @StratumCode")
                    .Query2(new { i.CruiseID, i.CuttingUnitCode, i.StratumCode }).FirstOrDefault();

                if (match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM CuttingUnitStratum WHERE CruiseID = @CruiseID AND CuttingUnitCode = @CuttingUnitCode AND StratumCode = @StratumCode;", new { i.CruiseID, i.CuttingUnitCode, i.StratumCode });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }

        private void SyncSampleGroup(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<SampleGroup_Tombstone>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

            foreach (var i in deletedItems)
            {
                var match = destination.From<SampleGroup>()
                    .Where("SampleGroupID = @SampleGroupID")
                    .Query2(new { i.SampleGroupID }).FirstOrDefault();

                if (match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM SampleGroup WHERE SampleGroupID = @SampleGroupID;", new { i.SampleGroupID });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }

        private void SyncSubPopulation(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<SubPopulation_Tombstone>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

            foreach (var i in deletedItems)
            {
                var match = destination.From<SubPopulation>()
                    .Where("SubPopulationID = @SubPopulationID")
                    .Query2(new { i.SubPopulationID }).FirstOrDefault();

                if (match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM SubPopulation WHERE SubPopulationID = @SubPopulationID;", new { i.SubPopulationID });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }

        // FixCNT Tally Population doesn't have a tombstone table
        //private void SyncFixCNTTallyPopulation(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        //{
        //    throw new NotImplementedException();
        //}

        private void SyncLogFieldSetup(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<LogFieldSetup_Tombstone>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

            foreach (var i in deletedItems)
            {
                var match = destination.From<LogFieldSetup>()
                    .Where("CruiseID = @CruiseID AND StratumCode = @StratumCode AND Field = @Field")
                    .Query2(new { i.CruiseID, i.StratumCode, i.Field }).FirstOrDefault();

                if (match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM LogFieldSetup WHERE CruiseID = @CruiseID AND StratumCode = @StratumCode AND Field = @Field;", 
                        new { i.CruiseID, i.StratumCode, i.Field });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }

        private void SyncTreeFieldSetup(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<TreeFieldSetup_Tombstone>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

            foreach (var i in deletedItems)
            {
                var match = destination.From<TreeFieldSetup>()
                    .Where("CruiseID = @CruiseID AND StratumCode = @StratumCode AND SampleGroupCode = @SampleGroupCode AND Field = @Field")
                    .Query2(new { i.CruiseID, i.StratumCode, i.SampleGroupCode, i.Field }).FirstOrDefault();

                if (match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM TreeFieldSetup WHERE CruiseID = @CruiseID AND StratumCode = @StratumCode AND SampleGroupCode = @SampleGroupCode AND Field = @Field;",
                        new { i.CruiseID, i.StratumCode, i.SampleGroupCode, i.Field });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }

        private void SyncTreeAuditRule(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<TreeAuditRule_Tombstone>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

            foreach (var i in deletedItems)
            {
                var match = destination.From<TreeAuditRule>()
                    .Where("TreeAuditRuleID = @TreeAuditRuleID")
                    .Query2(new { i.TreeAuditRuleID }).FirstOrDefault();

                if (match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM TreeAuditRule WHERE TreeAuditRuleID = @TreeAuditRuleID;",
                        new { i.TreeAuditRuleID });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }

        private void SyncTreeAuditRuleSelector(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<TreeAuditRuleSelector_Tombstone>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

            foreach (var i in deletedItems)
            {
                var match = destination.From<TreeAuditRuleSelector>()
                    .Where("TreeAuditRuleID = @TreeAuditRuleID AND ifnull(SpeciesCode, '') = ifnull(@SpeciesCode, '') AND ifnull(PrimaryProduct, '') = ifnull(@PrimaryProduct, '') AND ifnull(LiveDead, '') = ifnull(@LiveDead, '')")
                    .Query2(new { i.TreeAuditRuleID, i.SpeciesCode, i.PrimaryProduct, i.LiveDead }).FirstOrDefault();

                if (match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM TreeAuditRuleSelector WHERE TreeAuditRuleID = @TreeAuditRuleID AND ifnull(SpeciesCode, '') = ifnull(@SpeciesCode, '') AND ifnull(PrimaryProduct, '') = ifnull(@PrimaryProduct, '') AND ifnull(LiveDead, '') = ifnull(@LiveDead, '');",
                        new { i.TreeAuditRuleID, i.SpeciesCode, i.PrimaryProduct, i.LiveDead });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }

        private void SyncTreeAuditResolution(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<TreeAuditResolution_Tombstone>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

            foreach (var i in deletedItems)
            {
                var match = destination.From<TreeAuditResolution>()
                    .Where("TreeAuditRuleID = @TreeAuditRuleID AND TreeID = @TreeID")
                    .Query2(new { i.TreeAuditRuleID, i.TreeID }).FirstOrDefault();

                if (match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM TreeAuditResolution WHERE TreeAuditRuleID = @TreeAuditRuleID AND ifnull(SpeciesCode, '') = ifnull(@SpeciesCode, '') AND ifnull(PrimaryProduct, '') = ifnull(@PrimaryProduct, '') AND ifnull(LiveDead, '') = ifnull(@LiveDead, '');",
                        new { i.TreeAuditRuleID, i.TreeID });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }

        private void SyncPlots(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<Plot_Tombstone>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

            foreach (var i in deletedItems)
            {
                var match = destination.From<Plot>()
                    .Where("PlotID = @PlotID")
                    .Query2(new { i.PlotID }).FirstOrDefault();

                if (match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM Plot WHERE PlotID = @PlotID;",
                        new { i.PlotID });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }

        private void SyncPlotLocation(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<PlotLocation_Tombstone>()
                .Join("Plot", "USING (PlotID)")
                .Where("CruiseID = @p1")
                .Query(cruiseID);

            foreach (var i in deletedItems)
            {
                var match = destination.From<PlotLocation>()
                    .Where("PlotID = @PlotID")
                    .Query2(new { i.PlotID }).FirstOrDefault();

                if (match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM PlotLocation WHERE PlotID = @PlotID;",
                        new { i.PlotID });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }

        private void SyncPlot_Strata(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<Plot_Stratum_Tombstone>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

            foreach (var i in deletedItems)
            {
                var match = destination.From<Plot_Stratum>()
                    .Where("CruiseID = @CruiseID AND PlotNumber = @PlotNumber AND StratumCode = @StratumCode")
                    .Query2(new { i.CruiseID, i.PlotNumber, i.StratumCode }).FirstOrDefault();

                if (match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM Plot_Stratum WHERE CruiseID = @CruiseID AND PlotNumber = @PlotNumber AND StratumCode = @StratumCode;",
                        new { i.CruiseID, i.PlotNumber, i.StratumCode });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }

        private void SyncTree(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<Tree_Tombstone>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

            foreach (var i in deletedItems)
            {
                var match = destination.From<Tree>()
                    .Where("TreeID = @TreeID")
                    .Query2(new { i.TreeID }).FirstOrDefault();

                if (match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM Tree WHERE TreeID = @TreeID;",
                        new { i.TreeID });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }

        private void SyncTallyLedger(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<TallyLedger_Tombstone>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

            foreach(var i in deletedItems)
            {
                var match = destination.From<TallyLedger>()
                    .Where("TallyLedgerID = @TallyLedgerID")
                    .Query2(i.TallyLedgerID)
                    .FirstOrDefault();

                if (match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM TallyLedger WHERE TallyLedgerID = @TallyLedgerID;",
                        new { i.TallyLedgerID });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }

        private void synclog(string cruiseid, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<Log_Tombstone>()
                .Where("CruiseID = @p1")
                .Query(cruiseid);

            foreach(var i in deletedItems)
            {
                var match = destination.From<Log>()
                    .Where("LogID = @LogID")
                    .Query2(new { i.LogID })
                    .FirstOrDefault();

                if(match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM Log WHERE LogID = @LogID;", new { i.LogID });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }

        private void SyncStem(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<Stem_Tombstone>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

            foreach(var i in deletedItems)
            {
                var match = destination.From<Stem>()
                    .Where("Stem = @StemID")
                    .Query2(new { i.StemID })
                    .FirstOrDefault();

                if(match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM Stem WHERE StemID = @StemID;", new { i.StemID });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }

        private void SyncVolumeEquations(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<VolumeEquation_Tombstone>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

            foreach(var i in deletedItems)
            {
                var match = destination.From<VolumeEquation>()
                    .Where("CruiseID = @CruiseID AND Species = @Species AND PrimaryProduct = @PrimaryProduct AND VolumeEquationNumber = @VolumeEquationNumber")
                    .Query2( new { i.CruiseID, i.Species, i.PrimaryProduct, i.VolumeEquationNumber })
                    .FirstOrDefault();

                if(match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM VolumeEquation WHERE CruiseID = @CruiseID AND Species = @Species AND PrimaryProduct = @PrimaryProduct AND VolumeEquationNumber = @VolumeEquationNumber", new { i.CruiseID, i.Species, i.PrimaryProduct, i.VolumeEquationNumber });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }

        private void SyncReports(string cruiseID, DbConnection source, DbConnection destination, CruiseSyncOptions options)
        {
            var deletedItems = source.From<Reports_Tombstone>()
                .Where("CruiseID = @p1")
                .Query(cruiseID);

            foreach(var i in deletedItems)
            {
                var match = destination.From<Reports>()
                    .Where("CruiseID = @CruiseID AND ReportID = @ReportID")
                    .Query2(new { i.CruiseID, i.ReportID })
                    .FirstOrDefault();

                if(match != null)
                {
                    var x = destination.ExecuteNonQuery2("DELETE FROM Reports WHERE CruiseID = @CruiseID AND ReportID = @ReportID;",
                        new { i.CruiseID, i.ReportID });
                }
                else
                {
                    destination.Insert(i);
                }
            }
        }
    }
}
