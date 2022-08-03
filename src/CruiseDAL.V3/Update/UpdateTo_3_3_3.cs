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
    // update 3.3.3 notes:
    // added column TemplateFile to Cruise table
    // added lookup table LK_TallyEntryType
    // remove check constraint on EntryType and add FKey on EntryType
    public class UpdateTo_3_3_3 : DbUpdateBase
    {
        public UpdateTo_3_3_3() 
            : base(targetVersion: "3.3.3", sourceVersions: new[] { "3.3.2" })
        {
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("ALTER TABLE main.Cruise ADD COLUMN TemplateFile TEXT;",
                transaction: transaction, exceptionProcessor: exceptionProcessor);

            // create table LK_TallyEntryType
            var tallyEntryTypeTableDef = new LK_TallyEntryType();
            CreateTable(conn, transaction, exceptionProcessor, tallyEntryTypeTableDef);

            // remove check constraint on EntryType and add FKey on EntryType
            var tallyLedgerTableDef = new TallyLedgerTableDefinition();
            UpdateTableDDL(conn, transaction, exceptionProcessor, tallyLedgerTableDef);
        }

    }
}
