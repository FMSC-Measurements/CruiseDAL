using CruiseDAL.Schema;
using FMSC.ORM.Core;
using System.Data.Common;

namespace CruiseDAL.Update
{
    public class UpdateTo_3_5_4 : DbUpdateBase
    {
        public UpdateTo_3_5_4() :
            base(targetVersion: "3.5.4", sourceVersions: new[] { "3.5.3", })
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
        }

        protected override void AfterCommit(DbConnection conn, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("PRAGMA foreign_keys=ON;", exceptionProcessor: exceptionProcessor);
        }
    }
}