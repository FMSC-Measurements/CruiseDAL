using CruiseDAL.Schema;
using FMSC.ORM.Core;
using FMSC.ORM.SQLite;
using System;

namespace CruiseDAL
{
    public class CruiseDALDatastoreBuilder : SQLiteDatabaseBuilder
    {
        public override void CreateTables(DatastoreRedux datastore)
        {
            var conn = datastore.OpenConnection();
            CreateDatabase(conn);

            datastore.ReleaseConnection();
        }

        public void CreateDatabase(System.Data.Common.DbConnection conn)
        {
            var transaction = conn.BeginTransaction();

            foreach (var cmd in DDL.CREATE_COMMANDS)
            {
                try
                {
                    conn.ExecuteNonQuery(cmd, (object[])null, transaction);
                }
                catch (Exception e)
                {
                    Logger.Log.E(e);
                    throw;
                }
            }

            transaction.Commit();
        }

        public override void CreateTriggers(DatastoreRedux datastore)
        {
            //datastore.Execute(Schema.Schema.CREATE_TRIGGERS);
        }

        public override void UpdateDatastore(DatastoreRedux datastore)
        {
            Updater.Update((DAL)datastore);
        }
    }
}