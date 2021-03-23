using FMSC.ORM.Core;
using FMSC.ORM.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL
{
    public class Updater_V3 : IUpdater
    {
        public static ILogger Logger { get; set; } = LoggerProvider.Get();

        public void Update(CruiseDatastore datastore)
        {
            var version = datastore.DatabaseVersion;
            if (version == "3.0.0" 
                || version == "3.0.1"
                || version == "3.0.2"
                || version == "3.0.3")
            {
                UpdateTo_3_1_0(datastore);
            }
            if(version == "3.1.0")
            {
                UpdateTo_3_2_0(datastore);
            }
        }

        public static void UpdateTo_3_1_0(CruiseDatastore ds)
        {
            // create an in-memory database
            // to migrate into
            using (var newDatastore = new CruiseDatastore_V3())
            {
                var excludeTables = new[] { "SamplerState" };
                // migrate contents of old db into new in-memory database
                Migrate(ds, newDatastore, excludeTables);

                // use back up rutine to replace old database with 
                // migrated contents
                newDatastore.BackupDatabase(ds);
            }
        }

        // update notes: Added table LK_District and updated initialization for LK_Forests
        public static void UpdateTo_3_2_0(CruiseDatastore ds)
        {
            // create an in-memory database
            // to migrate into
            using (var newDatastore = new CruiseDatastore_V3())
            {
                var excludeTables = new[] { "LK_Forest" };
                // migrate contents of old db into new in-memory database
                Migrate(ds, newDatastore, excludeTables);

                // use back up rutine to replace old database with 
                // migrated contents
                newDatastore.BackupDatabase(ds);
            }
        }

        public static void Migrate(CruiseDatastore sourceDS, CruiseDatastore destinationDS, IEnumerable<string> excluding = null)
        {
            var destConn = destinationDS.OpenConnection();
            var sourceConn = sourceDS.OpenConnection();


            try
            {
                Migrate(sourceConn, destConn, excluding);
            }
            finally
            {
                destinationDS.ReleaseConnection();
                sourceDS.ReleaseConnection();
            }
        }

        public static void Migrate(DbConnection sourceConn, DbConnection destConn, IEnumerable<string> excluding = null)
        {
            var to = "main"; // alias used by the source database
            var from = "fromdb";

            // get the initial state of foreign keys, used to restore foreign key setting at end of merge process
            var foreignKeys = destConn.ExecuteScalar<string>("PRAGMA foreign_keys;", null, null);
            destConn.ExecuteNonQuery("PRAGMA foreign_keys = off;");

#if SYSTEM_DATA_SQLITE
            var srcDataSource = ((System.Data.SQLite.SQLiteConnection)sourceConn).FileName;
#else
            var srcDataSource = sourceConn.DataSource;
#endif
            destConn.ExecuteNonQuery($"ATTACH DATABASE \"{srcDataSource}\" AS {from};");
            try
            {
                using (var transaction = destConn.BeginTransaction())
                {
                    // get list of all tables that are in both databases
                    IEnumerable<string> tables = ListTablesIntersect(destConn, sourceConn);
                    try
                    {
                        foreach (var table in tables)
                        {
                            if (excluding?.Contains(table) ?? false)
                            { continue; }

                            if (table == "Globals")
                            {
                                destConn.ExecuteNonQuery(
    $@"INSERT OR IGNORE INTO {to}.{table} (""Block"", ""Key"", ""Value"")
SELECT ""Block"", ""Key"", ""Value""
FROM {from}.{table} 
WHERE ""Block"" != 'Database' AND ""Key"" != 'Version';", null, transaction);
                                continue;
                            }
                            else
                            {
                                // get the interscetion of fields in table from both databases 
                                // encased in double quotes just incase any field names are sql keywords
                                string[] both = ListFieldsIntersect(sourceConn, destConn, table);
                                var fields = string.Join(",", both);

                                destConn.ExecuteNonQuery(
    $@"INSERT OR IGNORE INTO {to}.{table} ({fields})
SELECT {fields} FROM {from}.{table};"
                                    , null, transaction);

                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        Logger.LogException(e);
                        throw;
                    }
                    finally
                    {
                        sourceConn.ExecuteNonQuery($"PRAGMA foreign_keys = {foreignKeys};");
                    }
                }
            }
            finally
            {
                destConn.ExecuteNonQuery($"DETACH DATABASE {from};");
            }
        }

        public static string[] ListFieldsIntersect(DbConnection sourceConn, DbConnection destConn, string table)
        {
            var sourceFields = sourceConn.QueryScalar2<string>($@"SELECT '""' || Name || '""' FROM pragma_table_info('{table}');");

            var destFields = destConn.QueryScalar2<string>($@"SELECT '""' || Name || '""' FROM pragma_table_info('{table}');");

            var both = sourceFields.Intersect(destFields).ToArray();
            return both;
        }

        public static string[] ListTablesIntersect(DbConnection conn1, DbConnection conn2)
        {
            var query = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite^_%' ESCAPE '^';";

            var tables1 = conn1.QueryScalar2<string>(query);

            var tables2 = conn2.QueryScalar2<string>(query);

            return tables1.Intersect(tables2).ToArray();
        }
    }
}