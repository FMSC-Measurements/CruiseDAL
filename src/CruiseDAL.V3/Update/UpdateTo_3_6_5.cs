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

            // in older versions of CruiseDAL.V3.Upconvert TallyLedger would be initialized with TallyLedgerIDs that weren't globally unique 
            // this would cause issues when importing cruises into FScruiser. To mitigate this issue we can include add the cruise ID to the
            // tallyLedgerID making it at least unique to the cruise. 
            foreach(var cruiseID in conn.QueryScalar<string>("SELECT DISTINCT CruiseID FROM TallyLedger WHERE TallyLedgerID LIKE 'migrateFromTree-%' OR TallyLedgerID LIKE 'initFromCountTree-%';"))
            {
                conn.ExecuteNonQuery($"UPDATE TallyLedger SET TallyLedgerID = '{cruiseID}' || '-' || TallyLedgerID WHERE TallyLedgerID LIKE 'migrateFromTree-%' OR TallyLedgerID LIKE 'initFromCountTree-%';");
                conn.ExecuteNonQuery("INSERT INTO CruiseLog (CruiseID, Message) VALUES (@p1, @p2);", 
                    new[] { cruiseID, "Applied update 3.6.5 Fix to TallyLedgerIDs" });
            }


            //if(conn.ExecuteScalar<bool>("SELECT count(1) > 0 FROM TallyLedger WHERE TallyLedgerID LIKE 'migrateFromTree-%' OR TallyLedgerID LIKE 'initFromCountTree-%';"))
            //{
            //    conn.ExecuteNonQuery("UPDATE TallyLedger SET TallyLedgerID = CruiseID || '-' || TallyLedgerID WHERE TallyLedgerID LIKE 'migrateFromTree-%' OR TallyLedgerID LIKE 'initFromCountTree-%';");
            //}
        }
    }
}