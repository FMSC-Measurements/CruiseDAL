using CruiseDAL.Schema;
using CruiseDAL.Schema.Views;
using FMSC.ORM.Core;
using System.Collections.Generic;
using System.Data.Common;

namespace CruiseDAL.Update
{
    // update 3.3.4 notes:
    // added change tracking fields to StratumTempalte, StratumTemplateTreeFieldSetup, and StratumTemplateLogFileSetup
    // added tombstone tables for StratumTempalte, StratumTemplateTreeFieldSetup, and StratumTemplateLogFileSetup
    public class UpdateTo_3_3_4 : DbUpdateBase
    {
        public UpdateTo_3_3_4()
            : base(targetVersion: "3.3.4", sourceVersions: new[] { "3.3.3" })
        {
        }

        protected override void BeforeBegin(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=OFF;", exceptionProcessor: exceptionProcessor);
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            var stratumTemplateTableDef = new StratumTemplateTableDefinition();
            var stratumTemplateTreeFieldSetupTableDef = new StratumTemplateTreeFieldSetupTableDefinition();
            var stratumTemplateLogFieldSetupTableDef = new StratumTemplateLogFieldSetupTableDefinition();

            conn.ExecuteNonQuery("DROP VIEW StratumDefault;", transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery("DROP VIEW TreeFieldSetupDefault;", transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery("DROP VIEW LogFieldSetupDefault;", transaction: transaction, exceptionProcessor: exceptionProcessor);

            conn.ExecuteNonQuery(stratumTemplateTableDef.CreateTombstoneTable, transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(stratumTemplateTreeFieldSetupTableDef.CreateTombstoneTable, transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(stratumTemplateLogFieldSetupTableDef.CreateTombstoneTable, transaction: transaction, exceptionProcessor: exceptionProcessor);

            RebuildTable(conn, transaction, exceptionProcessor, stratumTemplateTableDef);
            RebuildTable(conn, transaction, exceptionProcessor, stratumTemplateTreeFieldSetupTableDef);
            RebuildTable(conn, transaction, exceptionProcessor, stratumTemplateLogFieldSetupTableDef);

            conn.ExecuteNonQuery(new StratumDefaultViewDefinition().CreateView, transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(new TreeFieldSetupDefaultViewDefinition().CreateView, transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(new LogFieldSetupDefaultViewDefinition().CreateView, transaction: transaction, exceptionProcessor: exceptionProcessor);
        }

        protected override void AfterCommit(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=ON;", exceptionProcessor: exceptionProcessor);
        }
    }
}