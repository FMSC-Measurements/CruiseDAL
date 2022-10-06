using CruiseDAL.Schema;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Update
{
    /// <summary>
    /// Fixes some issues with Grade constraints by adding empty string as valid value. 
    /// </summary>

    public class UpdateTo_3_5_6 : DbUpdateBase
    {
        public UpdateTo_3_5_6()
    : base(targetVersion: "3.5.6", sourceVersions: new[] { "3.5.4", "3.5.5" })
        {
        }

        protected override void BeforeBegin(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=OFF;", exceptionProcessor: exceptionProcessor);
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            var suspendViews = new IViewDefinition[]
            {
                new LogGradeErrorViewDefinition(),
                new PlotErrorViewDefinition(),
                new Tree_TreeDefaultValue(),
                new TreeErrorViewDefinition(),
                new TreeFieldValue_AllViewDefinition(),
                new TreeFieldValue_TreeMeasurment_FilteredViewDefinition(),
                new TreeFieldValue_TreeMeasurmentViewDefinition(),
                new TreeMeasurment_DefaultResolved(),
                new TreeAuditErrorViewDefinition(),
                new TallyPopulationViewDefinition_3_3_2(),
            };

            foreach (var view in suspendViews)
            {
                conn.ExecuteNonQuery($"DROP VIEW {view.ViewName};",
                    transaction: transaction, exceptionProcessor: exceptionProcessor);
            }

            RebuildTable(conn, transaction, exceptionProcessor, new SaleTableDefinition_3_5_5(),
                new[] { new KeyValuePair<string, string>("District", "CASE WHEN District NOT NULL THEN substr(District, 1, min(2, length(District))) ELSE NULL END") });

            RebuildTable(conn, transaction, exceptionProcessor, new CuttingUnitTableDefinition_3_5_5());
            RebuildTable(conn, transaction, exceptionProcessor, new LogTableDefinition_3_5_6(),
                new[] {
                    new KeyValuePair<string, string>("Grade", "CASE Grade WHEN NULL THEN NULL ELSE substr(trim(Grade),-1,1) END"),
                    new KeyValuePair<string, string>("SeenDefect", "min(SeenDefect, 100)"),
                    new KeyValuePair<string, string>("PercentRecoverable", "min(PercentRecoverable, 100)"),
                });
            RebuildTable(conn, transaction, exceptionProcessor, new LogGradeAuditRuleTableDefinition_3_5_5());
            RebuildTable(conn, transaction, exceptionProcessor, new PlotTableDefinition_3_5_5(),
                new[] {
                    new KeyValuePair<string, string>("Slope", "min(Slope, 200)"),
                    new KeyValuePair<string, string>("Aspect", "min(Aspect, 200)"),
                });
            RebuildTable(conn, transaction, exceptionProcessor, new Plot_StratumTableDefinition_3_5_5());
            RebuildTable(conn, transaction, exceptionProcessor, new SampleGroupTableDefinition_3_5_5());
            RebuildTable(conn, transaction, exceptionProcessor, new SamplerStateTableDefinition_3_5_5(),
                new[] { new KeyValuePair<string,string>( "InsuranceIndex", "Cast (InsuranceIndex AS INTEGER)"),
                         new KeyValuePair<string,string>( "InsuranceCounter", "Cast (InsuranceCounter AS INTEGER)"),});

            RebuildTable(conn, transaction, exceptionProcessor, new TreeTableDefinition_3_5_5());
            RebuildTable(conn, transaction, exceptionProcessor, new TreeDefaultValueTableDefinition_3_5_5(),
                new[] {
                    new KeyValuePair<string, string>("CullPrimary", "min(CullPrimary, 100)"),
                    new KeyValuePair<string, string>("CullPrimaryDead", "min(CullPrimaryDead, 100)"),
                    new KeyValuePair<string, string>("HiddenPrimary", "min(HiddenPrimary, 100)"),
                    new KeyValuePair<string, string>("HiddenPrimaryDead", "min(HiddenPrimaryDead, 100)"),
                    new KeyValuePair<string, string>("TreeGrade", "CASE TreeGrade WHEN NULL THEN NULL ELSE substr(trim(TreeGrade),-1,1) END"),
                    new KeyValuePair<string, string>("TreeGradeDead", "CASE TreeGradeDead WHEN NULL THEN NULL ELSE substr(trim(TreeGradeDead),-1,1) END"),
                    new KeyValuePair<string, string>("CullSecondary", "min(CullSecondary, 100)"),
                    new KeyValuePair<string, string>("HiddenSecondary", "min(HiddenSecondary, 100)"),
                    new KeyValuePair<string, string>("Recoverable", "min(Recoverable, 100)"),
                });
            RebuildTable(conn, transaction, exceptionProcessor, new TreeMeasurmentTableDefinition_3_5_6(), 
                new[] {
                    new KeyValuePair<string, string>("Grade", "CASE Grade WHEN NULL THEN NULL ELSE substr(trim(Grade),-1,1) END"),
                    new KeyValuePair<string, string>("SeenDefectPrimary", "min(SeenDefectPrimary, 100)"),
                    new KeyValuePair<string, string>("SeenDefectSecondary", "min(SeenDefectSecondary, 100)"),
                    new KeyValuePair<string, string>("RecoverablePrimary", "min(RecoverablePrimary, 100)"),
                    new KeyValuePair<string, string>("HiddenPrimary", "min(HiddenPrimary, 100)"),
                    new KeyValuePair<string, string>("VoidPercent", "min(HiddenPrimary, 100)"),
                    new KeyValuePair<string, string>("Slope", "min(Slope, 200)"),
                    new KeyValuePair<string, string>("Aspect", "min(Aspect, 200)"),
                });
            RebuildTable(conn, transaction, exceptionProcessor, new TreeAuditRuleSelectorTableDefinition_3_5_5());

            foreach (var view in suspendViews)
            {
                conn.ExecuteNonQuery(view.CreateView,
                    transaction: transaction, exceptionProcessor: exceptionProcessor);
            }
        }

        protected override void AfterCommit(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=ON;", exceptionProcessor: exceptionProcessor);
        }
    }
}
