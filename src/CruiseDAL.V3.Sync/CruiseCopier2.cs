using Backpack.SqlBuilder;
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
    public class CruiseCopier2 : DatabaseCopier
    {
        public CruiseCopier2()
        {
            Tables = new[]
            {
                new CopyTableConfig(nameof(Sale), action: CopySale),
                new CopyTableConfig(nameof(Cruise), action: CopyCruise),
                new CopyTableConfig(typeof(Device)),
                new CopyTableConfig(typeof(CuttingUnit)),
                new CopyTableConfig(typeof(CuttingUnit_Tombstone)),
                new CopyTableConfig(typeof(Stratum)),
                new CopyTableConfig(typeof(Stratum_Tombstone)),
                new CopyTableConfig(typeof(CuttingUnit_Stratum)),
                new CopyTableConfig(typeof(CuttingUnit_Stratum_Tombstone)),
                new CopyTableConfig(typeof(SampleGroup)),
                new CopyTableConfig(typeof(SampleGroup_Tombstone)),
                new CopyTableConfig(typeof(SamplerState)),
                new CopyTableConfig(typeof(Species)),
                new CopyTableConfig(typeof(Species_Product)),
                new CopyTableConfig(typeof(SubPopulation)),
                new CopyTableConfig(typeof(SubPopulation_Tombstone)),
                new CopyTableConfig(typeof(TallyDescription)),
                new CopyTableConfig(typeof(TallyHotKey)),
                new CopyTableConfig(typeof(FixCNTTallyPopulation)),
                new CopyTableConfig(typeof(Plot)),
                new CopyTableConfig(typeof(Plot_Tombstone)),
                new CopyTableConfig(typeof(PlotLocation), configRead: x => x.Join("Plot as p", "USING (PlotID)").Where("p.CruiseID = @p1")),// custom
                new CopyTableConfig(typeof(Plot_Stratum)),
                new CopyTableConfig(typeof(Plot_Stratum_Tombstone)),
                new CopyTableConfig(typeof(Tree)),

                new CopyTableConfig(typeof(Tree_Tombstone)),
                new CopyTableConfig(nameof(TreeMeasurment_Tombstone), action: CopyTreeMeasurment_Tombstones),
                new CopyTableConfig(nameof(TreeFieldValue_Tombstone), action: CopyTreeFieldValue_Tombstone),
                new CopyTableConfig(nameof(TreeLocation_Tombstone), action: CopyTreeLocation_Tombstone),

                new CopyTableConfig(typeof(TreeMeasurment), configRead: x => x.Join("Tree AS t", "USING (TreeID)").Where("t.CruiseID = @p1")),
                new CopyTableConfig(typeof(TreeFieldValue), configRead: x=> x.Join("Tree AS t", "USING(TreeID)").Where("t.CruiseID = @p1;")),
                new CopyTableConfig(typeof(TreeLocation), configRead: x => x.Join("Tree AS t", "USING(TreeID)").Where("t.CruiseID = @p1;")),
                new CopyTableConfig(typeof(Log)),
                new CopyTableConfig(typeof(Log_Tombstone)),
                new CopyTableConfig(typeof(Stem)),
                new CopyTableConfig(typeof(Stem_Tombstone)),
                new CopyTableConfig(typeof(TallyLedger)),
                new CopyTableConfig(typeof(TallyLedger_Tombstone)),
                new CopyTableConfig(typeof(TreeDefaultValue)),
                new CopyTableConfig(typeof(TreeDefaultValue_Tombstone)),
                new CopyTableConfig(typeof(LogFieldSetup)),
                new CopyTableConfig(typeof(LogFieldSetup_Tombstone)),
                new CopyTableConfig(typeof(TreeFieldSetup)),
                new CopyTableConfig(typeof(TreeFieldSetup_Tombstone)),
                new CopyTableConfig(typeof(TreeFieldHeading)),

                new CopyTableConfig(typeof(TreeAuditRule)),
                new CopyTableConfig(typeof(TreeAuditRule_Tombstone)),

                new CopyTableConfig(typeof(TreeAuditRuleSelector)),
                new CopyTableConfig(typeof(TreeAuditRuleSelector_Tombstone)),
                new CopyTableConfig(typeof(TreeAuditResolution)),
                new CopyTableConfig(typeof(TreeAuditResolution_Tombstone)),

                new CopyTableConfig(typeof(LogGradeAuditRule)),
                new CopyTableConfig(typeof(LogGradeAuditRule_Tombstone)),

                new CopyTableConfig(typeof(BiomassEquation)),
                new CopyTableConfig(typeof(Reports)),
                new CopyTableConfig(typeof(ValueEquation)),
                new CopyTableConfig(typeof(VolumeEquation)),

                new CopyTableConfig(typeof(StratumTemplate)),
                new CopyTableConfig(typeof(StratumTemplate_Tombstone)),

                new CopyTableConfig(typeof(StratumTemplateTreeFieldSetup)),
                new CopyTableConfig(typeof(StratumTemplateTreeFieldSetup_Tombstone)),

                new CopyTableConfig(typeof(StratumTemplateLogFieldSetup)),
                new CopyTableConfig(typeof(StratumTemplateLogFieldSetup_Tombstone)),

                new CopyTableConfig(typeof(CruiseLog)),
            };

        }

        private void CopyTreeLocation_Tombstone(DbConnection source, DbConnection dest, string srcCruiseID, string destCruiseID, OnConflictOption option)
        {
            var valueOverrides = GetValueOverrides(destCruiseID);

            var treetmb = source.From<Tree_Tombstone>().Where("CruiseID = @p1;").Query(srcCruiseID);
            foreach (var ttmb in treetmb)
            {
                var tltmb = source.From<TreeLocation_Tombstone>().Where("TreeID = @p1;").Query(ttmb.TreeID).FirstOrDefault();
                if (tltmb != null) { dest.Insert(tltmb, valueOverrides: valueOverrides, option: option); }
            }
        }

        private void CopyTreeFieldValue_Tombstone(DbConnection source, DbConnection dest, string srcCruiseID, string destCruiseID, OnConflictOption option)
        {
            var valueOverrides = GetValueOverrides(destCruiseID);

            var treetmb = source.From<Tree_Tombstone>().Where("CruiseID = @p1;").Query(srcCruiseID);
            foreach (var ttmb in treetmb)
            {
                var treeFieldValuestmb = source.From<TreeFieldValue_Tombstone>().Where("TreeID = @p1;").Query(ttmb.TreeID);
                foreach (var tfvtmb in treeFieldValuestmb)
                {
                    dest.Insert(tfvtmb, valueOverrides: valueOverrides, option: option);
                }
            }
        }

        private void CopyTreeMeasurment_Tombstones(DbConnection source, DbConnection dest, string srcCruiseID, string destCruiseID, OnConflictOption option)
        {
            var valueOverrides = GetValueOverrides(destCruiseID);

            var treetmb = source.From<Tree_Tombstone>().Where("CruiseID = @p1;").Query(srcCruiseID);
            foreach (var ttmb in treetmb)
            {
                var tmtmb = source.From<TreeMeasurment_Tombstone>().Where("TreeID = @p1;").Query(ttmb.TreeID).FirstOrDefault();
                if (tmtmb != null) { dest.Insert(tmtmb, valueOverrides: valueOverrides, option: option); }
            }
        }

        private void CopySale(DbConnection source, DbConnection destination, string srcCruiseID, string destCruiseID, OnConflictOption option)
        {
            var valueOverrides = GetValueOverrides(destCruiseID);
            var cruiseID = destCruiseID ?? srcCruiseID;


            if (destination.ExecuteScalar<int>("SELECT count(*) FROM Sale JOIN Cruise USING (SaleNumber) WHERE CruiseID = @p1;", new[] { cruiseID }) == 0)
            {
                var sale = source.From<Sale>()
                    .Join("Cruise AS cr", "USING (SaleNumber)")
                    .Where("CruiseID = @p1")
                    .Query(srcCruiseID).FirstOrDefault();
                destination.Insert(sale, valueOverrides: valueOverrides, option: option);
            }
        }

        private void CopyCruise(DbConnection source, DbConnection dest, string srcCruiseID, string destCruiseID, OnConflictOption option)
        {
            var valueOverrides = GetValueOverrides(destCruiseID);

            var cruise = source.From<Cruise>().Where("CruiseID = @p1;").Query(srcCruiseID).FirstOrDefault();
            dest.Insert(cruise, valueOverrides: valueOverrides, option: option);
        }

        private static void CopySale(DbConnection source, DbConnection destination, string cruiseID, OnConflictOption option)
        {
            if (destination.ExecuteScalar<int>("SELECT count(*) FROM Sale JOIN Cruise USING (SaleNumber) WHERE CruiseID = @p1;", new[] { cruiseID }) == 0)
            {
                var sale = source.From<Sale>()
                    .Join("Cruise AS cr", "USING (SaleNumber)")
                    .Where("CruiseID = @p1")
                    .Query(cruiseID).FirstOrDefault();
                destination.Insert(sale, option: option);
            }
        }
    }
}
