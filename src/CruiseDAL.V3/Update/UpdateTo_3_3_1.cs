using CruiseDAL.Schema;
using FMSC.ORM.Core;
using System.Collections.Generic;
using System.Data.Common;

namespace CruiseDAL.Update
{
    // update 3.3.1 notes:
    // Add CruiseID fields to Log and Stem tables
    // Change is fully backwards compatible with prior versions
    public class UpdateTo_3_3_1 : DbUpdateBase
    {
        public UpdateTo_3_3_1()
            : base(targetVersion: "3.3.1", sourceVersions: new[] { "3.3.0" })
        {
        }

        protected override void BeforeBegin(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=OFF;", exceptionProcessor: exceptionProcessor);
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            // need to drop any views associated with tables we are rebuilding
            conn.ExecuteNonQuery("DROP VIEW LogGradeError;");

            var logTableDef = new LogTableDefinition();
            var stemTableDef = new StemTableDefinition();

            conn.ExecuteNonQuery("DROP TABLE Log_Tombstone;",
                transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery("DROP TABLE Stem_Tombstone;",
                transaction: transaction, exceptionProcessor: exceptionProcessor);

            conn.ExecuteNonQuery(logTableDef.CreateTombstoneTable,
                transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(stemTableDef.CreateTombstoneTable,
                transaction: transaction, exceptionProcessor: exceptionProcessor);

            RebuildTable(conn, transaction, exceptionProcessor, logTableDef, customFieldMaps: new KeyValuePair<string, string>[]
            {
                    new KeyValuePair<string, string>("CruiseID", "(SELECT CruiseID FROM Tree WHERE Tree.TreeID = Log.TreeID)"),
            });

            RebuildTable(conn, transaction, exceptionProcessor, stemTableDef, customFieldMaps: new KeyValuePair<string, string>[]
            {
                    new KeyValuePair<string, string>("CruiseID", "(SELECT CruiseID FROM Tree WHERE Tree.TreeID = Stem.TreeID)"),
            });

            var lgeViewDef = new LogGradeErrorViewDefinition();
            conn.ExecuteNonQuery(lgeViewDef.CreateView,
                transaction: transaction, exceptionProcessor: exceptionProcessor);
        }

        protected override void AfterCommit(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=ON;", exceptionProcessor: exceptionProcessor);
        }
    }
}