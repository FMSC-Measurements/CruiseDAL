using CruiseDAL.Schema;
using FMSC.ORM.Core;
using System.Data.Common;

namespace CruiseDAL.Update
{
    public class UpdateTo_3_5_3 : DbUpdateBase
    {
        public UpdateTo_3_5_3() :
            base(targetVersion: "3.5.3", sourceVersions: new[] { "3.5.2", })
        {
        }

        protected override void BeforeBegin(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=OFF;", exceptionProcessor: exceptionProcessor);
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            RebuildTable(conn, transaction, exceptionProcessor, new CruiseTableDefinition_3_5_3());
        }

        protected override void AfterCommit(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=ON;", exceptionProcessor: exceptionProcessor);
        }
    }
}