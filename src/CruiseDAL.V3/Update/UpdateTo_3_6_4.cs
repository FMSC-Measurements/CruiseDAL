using CruiseDAL.Schema.Views;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Update
{
    public class UpdateTo_3_6_4 : DbUpdateBase
    {
        public UpdateTo_3_6_4()
            : base(targetVersion: "3.6.4", sourceVersions: new[] { "3.6.3", })
        {
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("DROP VIEW TreeAuditError;");
            conn.ExecuteNonQuery(TreeAuditErrorViewDefinition_364.CREATE_VIEW_364);
        }
    }
}
