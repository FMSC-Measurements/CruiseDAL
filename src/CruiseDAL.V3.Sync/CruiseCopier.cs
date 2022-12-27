using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync
{
    public class CruiseCopier
    {
        public void Copy(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID)
        {
            var srcConn = source.OpenConnection();
            try
            {
                var destConn = destination.OpenConnection();
                try
                {
                    destination.BeginTransaction();
                    try
                    {
                        Copy(srcConn, destConn, cruiseID);
                        destination.CommitTransaction();
                    }
                    catch
                    {
                        destination.RollbackTransaction();
                        throw;
                    }
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

        public void Copy(DbConnection source, DbConnection destination, string cruiseID)
        {
            CopySale(source, destination, cruiseID);
            CopyCruise(source, destination, cruiseID);

            CopyTable<Device>(source, destination, cruiseID);

            CopyTable<CuttingUnit>(source, destination, cruiseID);
            CopyTable<CuttingUnit_Tombstone>(source, destination, cruiseID);

            CopyTable<Stratum>(source, destination, cruiseID);
            CopyTable<Stratum_Tombstone>(source, destination, cruiseID);

            CopyTable<CuttingUnit_Stratum>(source, destination, cruiseID);
            CopyTable<CuttingUnit_Stratum_Tombstone>(source, destination, cruiseID);

            CopyTable<SampleGroup>(source, destination, cruiseID);
            CopyTable<SampleGroup_Tombstone>(source, destination, cruiseID);

            CopyTable<SamplerState>(source, destination, cruiseID);

            CopyTable<Species>(source, destination, cruiseID);

            CopyTable<Species_Product>(source, destination, cruiseID);

            CopyTable<SubPopulation>(source, destination, cruiseID);
            CopyTable<SubPopulation_Tombstone>(source, destination, cruiseID);

            CopyTable<TallyDescription>(source, destination, cruiseID);

            CopyTable<TallyHotKey>(source, destination, cruiseID);

            CopyTable<FixCNTTallyPopulation>(source, destination, cruiseID);

            CopyTable<Plot>(source, destination, cruiseID);
            CopyTable<Plot_Tombstone>(source, destination, cruiseID);
            CopyPlotLocation(source, destination, cruiseID);

            CopyTable<Plot_Stratum>(source, destination, cruiseID);
            CopyTable<Plot_Stratum_Tombstone>(source, destination, cruiseID);

            CopyTable<Tree>(source, destination, cruiseID);

            //Tree Tombstone
            var treetmb = source.From<Tree_Tombstone>().Where("CruiseID = @p1;").Query(cruiseID);
            // because TreeMeasurment, TreeFieldValue, and TreeLocation don't have a CruiseID field, we need to copy over tombstones for each deleated tree
            // we are working on the assumption that those records are only deleted when a tree is deleted. There is no other way to delete those record types.
            foreach (var ttmb in treetmb)
            {
                destination.Insert(ttmb);

                var tmtmb = source.From<TreeMeasurment_Tombstone>().Where("TreeID = @p1;").Query(ttmb.TreeID).FirstOrDefault();
                if (tmtmb != null) { destination.Insert(tmtmb); }

                var treeFieldValuestmb = source.From<TreeFieldValue_Tombstone>().Where("TreeID = @p1;").Query(ttmb.TreeID);
                foreach (var tfvtmb in treeFieldValuestmb)
                {
                    destination.Insert(tfvtmb);
                }

                var tltmb = source.From<TreeLocation_Tombstone>().Where("TreeID = @p1;").Query(ttmb.TreeID).FirstOrDefault();
                if (tltmb != null) { destination.Insert(tltmb); }
            }

            CopyTreeMeasurment(source, destination, cruiseID);
            CopyTreeFieldValue(source, destination, cruiseID);
            CopyTreeLocation(source, destination, cruiseID);

            CopyTable<Log>(source, destination, cruiseID);
            CopyTable<Log_Tombstone>(source, destination, cruiseID);

            CopyTable<Stem>(source, destination, cruiseID);
            CopyTable<Stem_Tombstone>(source, destination, cruiseID);

            CopyTable<TallyLedger>(source, destination, cruiseID);
            CopyTable<TallyLedger_Tombstone>(source, destination, cruiseID);

            CopyTable<TreeDefaultValue>(source, destination, cruiseID);
            CopyTable<TreeDefaultValue_Tombstone>(source, destination, cruiseID);

            //TODO TreeField table?

            // Field Setup
            CopyTable<LogFieldSetup>(source, destination, cruiseID);
            CopyTable<LogFieldSetup_Tombstone>(source, destination, cruiseID);

            CopyTable<LogFieldHeading>(source, destination, cruiseID);

            CopyTable<TreeFieldSetup>(source, destination, cruiseID);
            CopyTable<TreeFieldSetup_Tombstone>(source, destination, cruiseID);

            CopyTable<TreeFieldHeading>(source, destination, cruiseID);

            // Audit Rules
            CopyTable<TreeAuditRule>(source, destination, cruiseID);
            CopyTable<TreeAuditRule_Tombstone>(source, destination, cruiseID);

            CopyTable<TreeAuditRuleSelector>(source, destination, cruiseID);
            CopyTable<TreeAuditRuleSelector_Tombstone>(source, destination, cruiseID);

            CopyTable<TreeAuditResolution>(source, destination, cruiseID);
            CopyTable<TreeAuditResolution_Tombstone>(source, destination, cruiseID);

            CopyTable<LogGradeAuditRule>(source, destination, cruiseID);
            CopyTable<LogGradeAuditRule_Tombstone>(source, destination, cruiseID);

            // Processing
            CopyTable<BiomassEquation>(source, destination, cruiseID);
            CopyTable<Reports>(source, destination, cruiseID);
            CopyTable<ValueEquation>(source, destination, cruiseID);
            CopyTable<VolumeEquation>(source, destination, cruiseID);

            // template
            CopyTable<StratumTemplate>(source, destination, cruiseID);
            CopyTable<StratumTemplate_Tombstone>(source, destination, cruiseID);

            CopyTable<StratumTemplateTreeFieldSetup>(source, destination, cruiseID);
            CopyTable<StratumTemplateTreeFieldSetup_Tombstone>(source, destination, cruiseID);

            CopyTable<StratumTemplateLogFieldSetup>(source, destination, cruiseID);
            CopyTable<StratumTemplateLogFieldSetup_Tombstone>(source, destination, cruiseID);

            // util
            CopyTable<CruiseLog>(source, destination, cruiseID);
        }

        public void CopyTable<TTable>(DbConnection source, DbConnection destination, string cruiseID) where TTable : class, new()
        {
            var sourceRecs = source.From<TTable>().Where("CruiseID = @p1;").Query(cruiseID);
            foreach (var rec in sourceRecs)
            {
                destination.Insert(rec);
            }
        }

        private static void CopyPlotLocation(DbConnection source, DbConnection destination, string cruiseID)
        {
            var plotLocations = source.From<PlotLocation>().Join("Plot AS p", "USING(PlotID)").Where("p.CruiseID = @p1;").Query(cruiseID);
            foreach (var pl in plotLocations)
            {
                destination.Insert(pl);
            }
        }

        private static void CopyTreeLocation(DbConnection source, DbConnection destination, string cruiseID)
        {
            var treeLocations = source.From<TreeLocation>().Join("Tree AS t", "USING(TreeID)").Where("t.CruiseID = @p1;").Query(cruiseID);
            foreach (var tl in treeLocations)
            {
                destination.Insert(tl);
            }
        }

        private static void CopyTreeFieldValue(DbConnection source, DbConnection destination, string cruiseID)
        {
            var treeFieldValues = source.From<TreeFieldValue>().Join("Tree AS t", "USING(TreeID)").Where("t.CruiseID = @p1;").Query(cruiseID);
            foreach (var tfv in treeFieldValues)
            {
                destination.Insert(tfv);
            }
        }

        private static void CopyTreeMeasurment(DbConnection source, DbConnection destination, string cruiseID)
        {
            var treeMeasurments = source.From<TreeMeasurment>().Join("Tree AS t", "USING(TreeID)").Where("t.CruiseID = @p1;").Query(cruiseID);
            foreach (var tm in treeMeasurments)
            {
                destination.Insert(tm);
            }
        }

        private static void CopyCruise(DbConnection source, DbConnection destination, string cruiseID)
        {
            var cruise = source.From<Cruise>().Where("CruiseID = @p1;").Query(cruiseID).FirstOrDefault();
            destination.Insert(cruise);
        }

        private static void CopySale(DbConnection source, DbConnection destination, string cruiseID)
        {
            if (destination.ExecuteScalar<int>("SELECT count(*) FROM Sale JOIN Cruise USING (SaleNumber) WHERE CruiseID = @p1;", new[] { cruiseID }) == 0)
            {
                var sale = source.From<Sale>()
                    .Join("Cruise AS cr", "USING (SaleNumber)")
                    .Where("CruiseID = @p1")
                    .Query(cruiseID).FirstOrDefault();
                destination.Insert(sale);
            }
        }
    }
}