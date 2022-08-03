using FMSC.ORM.Core;
using System.Data.Common;

namespace CruiseDAL.Update
{
    // UpdateTo_3_4_1 forgot to add CountOrMeasure, TreeCount, and AverageHeight fields to
    // the Plot_Stratum table. This update checks to see if they need to be added and adds them
    // if missing
    public class UpdateTo_3_4_2 : DbUpdateBase
    {
        public UpdateTo_3_4_2()
            : base(targetVersion: "3.4.2", sourceVersions: new[] { "3.4.1" })
        {
        }

        protected override void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor)
        {
            conn.ExecuteNonQuery("ALTER TABLE Plot_Stratum_Tombstone ADD COLUMN CountOrMeasure TEXT COLLATE NOCASE;", transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery("ALTER TABLE Plot_Stratum_Tombstone ADD COLUMN TreeCount INTEGER Default 0;", transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery("ALTER TABLE Plot_Stratum_Tombstone ADD COLUMN AverageHeight REAL Default 0.0;", transaction: transaction, exceptionProcessor: exceptionProcessor);
        }
    }
}