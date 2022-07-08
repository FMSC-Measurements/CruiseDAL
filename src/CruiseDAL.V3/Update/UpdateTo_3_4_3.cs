using CruiseDAL.Schema;
using FMSC.ORM.Core;
using System.Data.Common;

namespace CruiseDAL.Update
{
    // add validation on tree for DBH and DRC
    public class UpdateTo_3_4_3 : DbUpdateBase
    {
        public UpdateTo_3_4_3()
            : base(targetVersion: "3.4.3", sourceVersions: new[] { "3.4.2" })
        {
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("DROP VIEW TreeError;", transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(TreeErrorViewDefinition.v3_4_3, transaction: transaction, exceptionProcessor: exceptionProcessor);
        }
    }
}