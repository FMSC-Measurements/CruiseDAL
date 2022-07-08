using CruiseDAL.Schema;
using FMSC.ORM.Core;
using System.Data.Common;

namespace CruiseDAL.Update
{
    // Add TreeCount, Average Height, and CountOrMeasure fields to plot stratum
    public class UpdateTo_3_4_1 : DbUpdateBase
    {
        public UpdateTo_3_4_1()
            : base(targetVersion: "3.4.1", sourceVersions: new[] { "3.4.0", })
        {
        }

        protected override void BeforeBegin(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=OFF;", exceptionProcessor: exceptionProcessor);
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            //Rebuild Plot_Stratum table
            conn.ExecuteNonQuery("DROP VIEW main.PlotError", transaction: transaction, exceptionProcessor: exceptionProcessor);
            RebuildTable(conn, transaction, exceptionProcessor, new Plot_StratumTableDefinition_3_4_1());
            conn.ExecuteNonQuery(new PlotErrorViewDefinition().CreateView, transaction: transaction, exceptionProcessor: exceptionProcessor);

            //create a bunch of clearTombstone triggers
            conn.ExecuteNonQuery(CuttingUnit_StratumTableDefinition.CREATE_TRIGGER_CuttingUnit_Stratum_OnInsert_ClearTombstone, transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(LogFieldSetupTableDefinition.CREATE_TRIGGER_LogFieldSetup_OnInsert_ClearTombstone, transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(SubPopulationTableDefinition.CREATE_TRIGGER_SubPopulation_OnInsert_ClearTombstone, transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(TreeAuditRuleSelectorTableDefinition.CREATE_TRIGGER_TreeAuditRuleSelector_OnInsert_ClearTombstone, transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(TreeFieldSetupTableDefinition.CREATE_TRIGGER_TreeFieldSetup_OnInsert_ClearTombstone, transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(ReportsTableDefinition.CREATE_TRIGGER_Reports_OnInsert_ClearTombstone, transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(VolumeEquationTableDefinition.CREATE_TRIGGE_VolumeEquation_OnInsert_ClearTombstone, transaction: transaction, exceptionProcessor: exceptionProcessor);

            // recreate index on Reports_Tombstone
            conn.ExecuteNonQuery("DROP INDEX Reports_Tombstone_ReportID;", transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery("CREATE INDEX Reports_Tombstone_ReportID_CruiseID ON Reports_Tombstone (ReportID, CruiseID);", transaction: transaction, exceptionProcessor: exceptionProcessor);
        }

        protected override void AfterCommit(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=ON;", exceptionProcessor: exceptionProcessor);
        }
    }
}