using CruiseDAL.Schema;
using FMSC.ORM.Core;
using System.Data.Common;

namespace CruiseDAL.Update
{
    public class UpdateTo_3_5_1 : DbUpdateBase
    {
        public UpdateTo_3_5_1()
            : base(targetVersion: "3.5.1", sourceVersions: new[] { "3.5.0", })
        {
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("DROP VIEW Tree_TreeDefaultValue;",
                transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery("DROP VIEW TreeAuditError",
                transaction: transaction, exceptionProcessor: exceptionProcessor);

            conn.ExecuteNonQuery(Tree_TreeDefaultValue.CREATE_VIEW_3_5_1,
                transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(TreeAuditErrorViewDefinition.CREATE_VIEW_3_5_1,
                transaction: transaction, exceptionProcessor: exceptionProcessor);
        }
    }
}