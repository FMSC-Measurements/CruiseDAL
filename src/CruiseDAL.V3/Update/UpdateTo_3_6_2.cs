using CruiseDAL.Schema;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Update
{
    public class UpdateTo_3_6_2 : DbUpdateBase
    {
        public UpdateTo_3_6_2()
            : base(targetVersion: "3.6.2", sourceVersions: new[] { "3.6.1", })
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
            var suspendViews = new IViewDefinition[]
             {
                new TallyPopulationViewDefinition_3_3_2(),
                new TreeMeasurment_DefaultResolvedViewDefinition(),
                new Tree_TreeDefaultValueViewDefinition(),
                new TreeErrorViewDefinition(),
                new TreeAuditErrorViewDefinition(),
             };

            foreach (var view in suspendViews)
            {
                conn.ExecuteNonQuery($"DROP VIEW {view.ViewName};",
                    transaction: transaction, exceptionProcessor: exceptionProcessor);
            }


            RebuildTable(conn, transaction, exceptionProcessor, new SampleGroupTableDefinition_3_6_2());


            foreach (var view in suspendViews)
            {
                conn.ExecuteNonQuery(view.CreateView,
                    transaction: transaction, exceptionProcessor: exceptionProcessor);
            }
        }
    }
}
