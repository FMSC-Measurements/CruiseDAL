using CruiseDAL.Schema;
using FMSC.ORM.Core;
using FMSC.ORM.SQLite;
using System;
using System.Data.Common;

namespace CruiseDAL
{
    public class CruiseDatastoreBuilder_V3 : SQLiteDatabaseBuilder
    {
        public override void BuildDatabase(Datastore datastore)
        {
            var conn = datastore.OpenConnection();

            try
            {
                var transaction = conn.BeginTransaction();

                BuildDatabase(conn, transaction);

                transaction.Commit();
            }
            finally
            {
                datastore.ReleaseConnection();
            }
        }

        public void BuildDatabase(DbConnection connection, DbTransaction transaction)
        {
            foreach (var cmd in DDL.CREATE_COMMANDS)
            {
                try
                {
                    connection.ExecuteNonQuery(cmd, (object[])null, transaction);
                }
                catch (Exception e)
                {
                    Logger.Log.E(e);
                    throw;
                }
            }
        }
    }
}