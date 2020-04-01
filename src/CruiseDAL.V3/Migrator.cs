using FMSC.ORM;
using FMSC.ORM.Core;
using FMSC.ORM.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;

namespace CruiseDAL
{
    public static class Migrator
    {
        public static ILogger Logger { get; set; } = LoggerProvider.Get();

        public static string GetConvertedPath(string v2Path)
        {
            // get path, directory and filename of original file
            var fileDirectory = System.IO.Path.GetDirectoryName(v2Path);
            var fileName = System.IO.Path.GetFileNameWithoutExtension(v2Path);

            // create path for new temp file
            var newFileName = fileName + ".crz3";
            var newFilePath = System.IO.Path.Combine(fileDirectory, newFileName);

            return newFilePath;
        }

        public static string MigrateFromV2ToV3(string v2Path, bool overwrite = false)
        {
            var newFilePath = GetConvertedPath(v2Path);
            var newFileName = System.IO.Path.GetFileName(newFilePath);

            // check temp file doesn't already exist, otherwise throw exception
            var fileAlreadyExists = File.Exists(newFilePath);
            if (fileAlreadyExists && !overwrite) { throw new UpdateException(newFileName + " already exists"); }

            MigrateFromV2ToV3(v2Path, newFilePath);

            return newFilePath;
        }

        public static void MigrateFromV2ToV3(string v2Path, string newFilePath)
        {
            using (var v2Cruise = new CruiseDatastore(v2Path, false, null, new Updater_V2()))
            using (var newCruise = new CruiseDatastore_V3(newFilePath, true))
            {
                MigrateFromV2ToV3(v2Cruise, newCruise);
            }
        }

        public static void MigrateFromV2ToV3(CruiseDatastore v2db, CruiseDatastore_V3 v3db)
        {
            var oldDbAlias = "v2";
            v3db.AttachDB(v2db, oldDbAlias);

            try
            {
                var connection = v3db.OpenConnection();
                MigrateFromV2ToV3(connection, oldDbAlias);
            }
            finally
            {
                v3db.ReleaseConnection();
                v3db.DetachDB(oldDbAlias);
            }
        }

        public static void MigrateFromV2ToV3(DbConnection connection, string from)
        {
            var to = "main";
            using (var transaction = connection.BeginTransaction())
            {
                foreach (var command in Schema.Migrations.GetMigrateCommands(to, from))
                {
                    try
                    {
                        connection.ExecuteNonQuery(command, transaction: transaction);
                    }
                    catch (Exception e)
                    {
                        Logger.LogException(e, new { Command = command });
                        throw;
                    }
                }

                transaction.Commit();
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


            var srcDataSource = sourceConn.DataSource;
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