using CruiseDAL.Schema;
using FMSC.ORM.Core;
using System.Data.Common;

namespace CruiseDAL.Update
{
    // add ON UPDATE CASCADE and remove ON UPDATE DELETE from SaleNumber foreign key
    public class UpdateTo_3_5_2 : DbUpdateBase
    {
        public UpdateTo_3_5_2() :
            base(targetVersion: "3.5.2", sourceVersions: new[] { "3.5.1", })
        {
        }

        protected override void BeforeBegin(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=OFF;", exceptionProcessor: exceptionProcessor);
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            // Rebuild VolumeEquation table adding ForeignKey constraint on CruiseID
            conn.ExecuteNonQuery("DELETE FROM VolumeEquation WHERE CruiseID NOT IN (SELECT CruiseID FROM Cruise)",
                transaction: transaction, exceptionProcessor: exceptionProcessor);
            RebuildTable(conn, transaction, exceptionProcessor, new VolumeEquationTableDefinition());

            RecreateView(conn, new TallyLedger_TotalsViewDefinition(),
                transaction: transaction, exceptionProcessor: exceptionProcessor);
            RecreateView(conn, new TallyLedger_Plot_TotalsViewDefinition(),
                transaction: transaction, exceptionProcessor: exceptionProcessor);
            RecreateView(conn, new TreeErrorViewDefinition(),
                transaction: transaction, exceptionProcessor: exceptionProcessor);
        }

        protected override void AfterCommit(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=ON;", exceptionProcessor: exceptionProcessor);
        }
    }
}