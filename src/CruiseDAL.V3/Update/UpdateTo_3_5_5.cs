using CruiseDAL.Schema;
using FMSC.ORM.Core;
using System.Collections.Generic;
using System.Data.Common;

namespace CruiseDAL.Update
{
    public class UpdateTo_3_5_5 : DbUpdateBase
    {
        public UpdateTo_3_5_5()
            : base(targetVersion: "3.5.5", sourceVersions: new[] { "3.5.4", })
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
                new Tree_TreeDefaultValueViewDefinition(),
                new TreeErrorViewDefinition(),
                new TreeFieldValue_AllViewDefinition(),
                new TreeFieldValue_TreeMeasurment_FilteredViewDefinition(),
                new TreeFieldValue_TreeMeasurmentViewDefinition(),
                new TreeMeasurment_DefaultResolvedViewDefinition(),
                new TreeAuditErrorViewDefinition(),
                new TallyPopulationViewDefinition_3_3_2(),
            };

            foreach(var view in suspendViews)
            {
                conn.ExecuteNonQuery($"DROP VIEW {view.ViewName};",
                    transaction: transaction, exceptionProcessor: exceptionProcessor);
            }

            RebuildTable(conn, transaction, exceptionProcessor, new SaleTableDefinition_3_5_5(),
                new[] { new KeyValuePair<string, string>("District", "CASE WHEN District NOT NULL THEN substr(District, 1, min(2, length(District))) ELSE NULL END") });

            RebuildTable(conn, transaction, exceptionProcessor, new CuttingUnitTableDefinition_3_5_5());
            RebuildTable(conn, transaction, exceptionProcessor, new LogTableDefinition_3_5_5());
            RebuildTable(conn, transaction, exceptionProcessor, new LogGradeAuditRuleTableDefinition_3_5_5());
            RebuildTable(conn, transaction, exceptionProcessor, new PlotTableDefinition_3_5_5());
            RebuildTable(conn, transaction, exceptionProcessor, new Plot_StratumTableDefinition_3_5_5());
            RebuildTable(conn, transaction, exceptionProcessor, new SampleGroupTableDefinition_3_5_5());
            RebuildTable(conn, transaction, exceptionProcessor, new SamplerStateTableDefinition_3_5_5(), 
                new[] { new KeyValuePair<string,string>( "InsuranceIndex", "Cast (InsuranceIndex AS INTEGER)"),
                         new KeyValuePair<string,string>( "InsuranceCounter", "Cast (InsuranceCounter AS INTEGER)"),});

            RebuildTable(conn, transaction, exceptionProcessor, new TreeTableDefinition_3_5_5());
            RebuildTable(conn, transaction, exceptionProcessor, new TreeDefaultValueTableDefinition_3_5_5());
            RebuildTable(conn, transaction, exceptionProcessor, new TreeMeasurmentsTableDefinition_3_5_5());
            RebuildTable(conn, transaction, exceptionProcessor, new TreeAuditRuleSelectorTableDefinition_3_5_5());

            foreach(var view in suspendViews)
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