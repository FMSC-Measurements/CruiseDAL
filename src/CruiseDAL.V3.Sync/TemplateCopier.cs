using CruiseDAL.V3.Models;

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
                new CopyTableConfig(typeof(TreeAuditRule)),
                new CopyTableConfig(typeof(TreeAuditRuleSelector), dependsOn: new[] {nameof(TreeAuditRule)}),

                new CopyTableConfig(typeof(LogGradeAuditRule)),

                //processing
                new CopyTableConfig(typeof(Reports)),
                new CopyTableConfig(typeof(VolumeEquation)),
                new CopyTableConfig(typeof(ValueEquation)),
                new CopyTableConfig(typeof(BiomassEquation)),
            };
        }
    }
}