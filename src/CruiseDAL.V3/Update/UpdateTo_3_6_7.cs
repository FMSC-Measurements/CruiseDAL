using CruiseDAL.Schema;
using CruiseDAL.Schema.Tables.TreeAuditResolution;
using CruiseDAL.Schema.Tables.TreeAuditRule;
using CruiseDAL.Schema.Tables.TreeAuditRuleSelector;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Update
{
    public class UpdateTo_3_6_7 : DbUpdateBase
    {
        public UpdateTo_3_6_7()
            : base(targetVersion: "3.6.7", sourceVersions: new[] { "3.6.6", })
        {
        }

        protected override void BeforeBegin(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=OFF;", exceptionProcessor: exceptionProcessor);
        }

        protected override void AfterCommit(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=ON;", exceptionProcessor: exceptionProcessor);
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            var hangingtarSels = conn.QueryScalar<int>("SELECT TreeAuditRuleSelector_CN FROM TreeAuditRuleSelector LEFT JOIN TreeAuditRule AS tar USING (TreeAuditRuleID, CruiseID) WHERE tar.RowID IS NULL")
                .ToArray();

            foreach (var cn in hangingtarSels)
            {
                conn.ExecuteNonQuery("DELETE FROM TreeAuditRuleSelector WHERE TreeAuditRuleSelector_CN = @p1;", new object[] { cn, });
            }

            var hangingTaRes = conn.QueryScalar<int>("SELECT TreeAuditResolution_CN FROM TreeAuditResolution LEFT JOIN TreeAuditRule AS tar USING (TreeAuditRuleID, CruiseID) WHERE tar.RowID IS NULL")
                .ToArray();

            foreach (var cn in hangingTaRes)
            {
                conn.ExecuteNonQuery("DELETE FROM TreeAuditResolution WHERE TreeAuditResolution_CN = @p1;", new object[] { cn, });
            }

            conn.ExecuteNonQuery("DROP VIEW TreeError;");
            conn.ExecuteNonQuery("DROP VIEW TreeAuditError;");

            RebuildTable(conn, transaction, exceptionProcessor, new TreeAuditRuleTableDefinition_367());
            RebuildTable(conn, transaction, exceptionProcessor, new TreeAuditResolutionTableDefinition_367());
            RebuildTable(conn, transaction, exceptionProcessor, new TreeAuditRuleSelectorTableDefinition_367());

            conn.ExecuteNonQuery(TreeAuditErrorViewDefinition.CREATE_VIEW_3_5_1);
            conn.ExecuteNonQuery(TreeErrorViewDefinition.v3_4_3);
        }
    }
}