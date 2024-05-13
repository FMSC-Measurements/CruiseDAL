using Backpack.SqlBuilder;
using CruiseDAL.V3.Models;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.V3.Sync
{
    public class TemplateCopier : DatabaseCopier
    {
        public TemplateCopier()
        {
            Tables = new[]
            {
                // field setup
                new CopyTableConfig(typeof(TreeFieldHeading)),
                new CopyTableConfig(typeof(LogFieldHeading)),

                // design templates
                new CopyTableConfig(typeof(StratumTemplate)),
                new CopyTableConfig(typeof(StratumTemplateLogFieldSetup), dependsOn: new[] {nameof(StratumTemplate)}),
                new CopyTableConfig(typeof(StratumTemplateTreeFieldSetup), dependsOn: new[] {nameof(StratumTemplate)}),

                // species
                new CopyTableConfig(typeof(Species)),
                new CopyTableConfig(typeof(Species_Product), dependsOn: new[] {nameof(Species)}),

                new CopyTableConfig(typeof(TreeDefaultValue), new[] {nameof(Species)}),

                // audit rules
                new CopyTableConfig(nameof(TreeAuditRule), action: CopyTreeAuditRules),
                new CopyTableConfig(nameof(TreeAuditRuleSelector), action: CopyTreeAuditRuleSelector, dependsOn: new[] {nameof(TreeAuditRule)}),

                new CopyTableConfig(typeof(LogGradeAuditRule)),

                //processing
                new CopyTableConfig(typeof(Reports)),
                new CopyTableConfig(typeof(VolumeEquation)),
                new CopyTableConfig(typeof(ValueEquation)),
                new CopyTableConfig(typeof(BiomassEquation)),
            };
        }

        private byte[] TreeAuditRuleMaskBytes { get; set; }

        private Dictionary<string, string> TreeAuditRuleIDMaps { get; set; }


        // when copying tree audit rules from a template to a cruise file, we need to change the treeAuditRuleID values so that 
        // cruises created by this template can co-exist with other cruises in the save database.
        public void CopyTreeAuditRules(DbConnection source, DbConnection destination, string cruiseID, string destCruiseID, OnConflictOption conflictOption)
        {
            var tarIDMaps =  new Dictionary<string, string>();
            bool remapTarIDs = destCruiseID != null && destCruiseID != cruiseID;
            

            var tars = source.From<TreeAuditRule>().Where("CruiseID = @p1").Query(cruiseID);
            foreach(var tar in tars)
            {
                if (remapTarIDs)
                {
                    var newTarID = GetNewTreeAuditRuleID(tar.TreeAuditRuleID, destCruiseID);

                    tarIDMaps.Add(tar.TreeAuditRuleID, newTarID);

                    tar.TreeAuditRuleID = newTarID;
                    tar.CruiseID = destCruiseID;
                }

                destination.Insert(tar, persistKeyvalue: false, option: conflictOption);
            }

            TreeAuditRuleIDMaps = (remapTarIDs) ? tarIDMaps : null;
        }

        public static string GetNewTreeAuditRuleID(string tarID, string cruiseID)
        {
            var cruiseIDmask = new Guid(cruiseID).ToByteArray();
            var tarIDBytes = new Guid(tarID).ToByteArray();
            var newTarIBytes = tarIDBytes.Zip(cruiseIDmask, (x, y) => (byte)(x ^ y)).ToArray();
            return new Guid(newTarIBytes).ToString();
        }

        public void CopyTreeAuditRuleSelector(DbConnection source, DbConnection destination, string cruiseID, string destCruiseID, OnConflictOption conflictOption)
        {
            var tarIDMaps = TreeAuditRuleIDMaps;

            var tarSels = source.From<TreeAuditRuleSelector>().Where("CruiseID = @p1").Query(cruiseID);
            foreach(var tarSel in  tarSels)
            {
                if (tarIDMaps != null)
                {
                    tarSel.TreeAuditRuleID = tarIDMaps[tarSel.TreeAuditRuleID];
                    tarSel.CruiseID = destCruiseID;
                }

                destination.Insert(tarSel, persistKeyvalue: false, option: conflictOption);
            }
        }

    }
}