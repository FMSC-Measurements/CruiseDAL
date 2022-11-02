using CruiseDAL.Schema.Tables.CrusieLog;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Update
{
    public class UpdateTo_3_6_0 : DbUpdateBase
    {
        public UpdateTo_3_6_0()
            : base(targetVersion: "3.6.0", sourceVersions: new[] { "3.6.0", })
        {
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            CreateTable(conn, transaction, exceptionProcessor, new CruiseLogTableDefinition_3_6_0());
        }
    }
}
