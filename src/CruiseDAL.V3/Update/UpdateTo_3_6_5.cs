using CruiseDAL.Schema.Tables.CrusieLog;
using FMSC.ORM.Core;
using System.Collections.Generic;
using System.Data.Common;

namespace CruiseDAL.Update
{
    public class UpdateTo_3_6_5 : DbUpdateBase
    {
        public UpdateTo_3_6_5()
            : base(targetVersion: "3.6.5", sourceVersions: new[] { "3.6.4", })
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
            RebuildTable(conn, transaction, exceptionProcessor, new CruiseLogTableDefinition_365(),
                new[]
                {
                    new KeyValuePair<string, string>("CreatedBy", "'updater'"),
                    new KeyValuePair<string, string>("Created_TS", "TimeStamp")
                });
        }
    }
}