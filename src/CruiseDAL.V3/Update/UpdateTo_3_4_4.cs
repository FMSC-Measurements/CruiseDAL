using CruiseDAL.Schema;
using FMSC.ORM.Core;
using System.Collections.Generic;
using System.Data.Common;

namespace CruiseDAL.Update
{
    // remove foreign keys from TallyLedger table used to keep SpeciesCode, LiveDead, StratumCode and SampleGroupCode
    // in sync between Tree and TallyLedger tables and replace them with triggers
    // this works better in situations where either SpeciesCode or LiveDead were initialy null
    // also removing indexes used to support those foreign keys from tree table
    public class UpdateTo_3_4_4 : DbUpdateBase
    {
        public UpdateTo_3_4_4()
            : base(targetVersion: "3.4.4", sourceVersions: new[] { "3.4.3", })
        {
        }

        protected override void BeforeBegin(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=OFF;", exceptionProcessor: exceptionProcessor);
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("DROP VIEW TallyLedger_Totals;", transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery("DROP VIEW TallyLedger_Tree_Totals;", transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery("DROP VIEW TallyLedger_Plot_Totals;", transaction: transaction, exceptionProcessor: exceptionProcessor);
            RebuildTable(conn, transaction, exceptionProcessor, new TallyLedgerTableDefinition());
            conn.ExecuteNonQuery(new TallyLedger_TotalsViewDefinition().CreateView, transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(new TallyLedger_Tree_TotalsViewDefinition().CreateView, transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(new TallyLedger_Plot_TotalsViewDefinition().CreateView, transaction: transaction, exceptionProcessor: exceptionProcessor);

            conn.ExecuteNonQuery(TreeTableDefinition.CREATE_TRIGGER_TREE_Cascade_Species_Updates, transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(TreeTableDefinition.CREATE_TRIGGER_TREE_Cascade_LiveDead_Updates, transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(TreeTableDefinition.CREATE_TRIGGER_TREE_Cascade_SampleGroupCode_Updates, transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(TreeTableDefinition.CREATE_TRIGGER_TREE_Cascade_StratumCode_Updates, transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery("DROP INDEX UIX_Tree_TreeID_SpeciesCode;", transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery("DROP INDEX UIX_Tree_TreeID_LiveDead;", transaction: transaction, exceptionProcessor: exceptionProcessor);
        }

        protected override void AfterCommit(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=ON;", exceptionProcessor: exceptionProcessor);
        }
    }
}