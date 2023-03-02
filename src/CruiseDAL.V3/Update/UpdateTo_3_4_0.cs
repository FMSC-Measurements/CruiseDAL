using CruiseDAL.Schema;
using FMSC.ORM;
using FMSC.ORM.Core;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.Update
{
    public class UpdateTo_3_4_0 : DbUpdateBase
    {
        public UpdateTo_3_4_0()
            : base(targetVersion: "3.4.0", sourceVersions: new[] { "3.3.4" })
        {
        }

        protected override void BeforeBegin(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=OFF;", exceptionProcessor: exceptionProcessor);
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            RebuildTable(conn, transaction, exceptionProcessor, new CruiseTableDefinition_3_4_0(), customFieldMaps:
                    new KeyValuePair<string, string>[] { new KeyValuePair<string, string>(
                        "SaleNumber",
                        "(SELECT SaleNumber FROM Sale WHERE cruise.SaleID = sale.SaleID)")
                    });

            var keyCheck = conn.QueryGeneric("PRAGMA foreign_key_check;", transaction: transaction, exceptionProcessor: exceptionProcessor);
            if (keyCheck.Any())
            {
                throw new SchemaException("Foreign Key Check failed");
            }

            conn.ExecuteNonQuery("CREATE INDEX NIX_TreeDefaultValue_PrimaryProduct ON TreeDefaultValue ('PrimaryProduct');", transaction: transaction, exceptionProcessor: exceptionProcessor);

            var tree_tdvViewDef = new Tree_TreeDefaultValueViewDefinition();
            conn.ExecuteNonQuery(tree_tdvViewDef.CreateView, transaction: transaction, exceptionProcessor: exceptionProcessor);

            var tm_defViewDef = new TreeMeasurment_DefaultResolvedViewDefinition();
            conn.ExecuteNonQuery(tm_defViewDef.CreateView, transaction: transaction, exceptionProcessor: exceptionProcessor);
        }

        protected override void AfterCommit(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=ON;", exceptionProcessor: exceptionProcessor);
        }
    }
}