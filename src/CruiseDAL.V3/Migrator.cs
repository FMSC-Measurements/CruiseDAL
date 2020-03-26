using FMSC.ORM;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;

namespace CruiseDAL
{
    public class Migrator
    {
        public static FMSC.ORM.Core.Logger Logger { get; set; } = new FMSC.ORM.Core.Logger();

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

            Migrator.MigrateFromV2ToV3(v2Path, newFilePath);

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
                        connection.ExecuteNonQuery(command, (object[])null, transaction);
                    }
                    catch (Exception e)
                    {
                        Logger.LogException(e);
                        throw;
                    }
                }

                transaction.Commit();
            }
        }

        public static void Migrate(CruiseDatastore sourceDS, CruiseDatastore destinationDS, IEnumerable<string> excluding = null)
        {
            var connection = destinationDS.OpenConnection();

            var fromAlias = "fromdb";
            destinationDS.AttachDB(sourceDS, fromAlias);
            try
            {
                Migrate(connection, fromAlias, excluding);
            }
            finally
            {
                destinationDS.ReleaseConnection();
                destinationDS.DetachDB(fromAlias);
            }
        }

        public static void Migrate(DbConnection connection, string from, IEnumerable<string> excluding = null)
        {
            var to = "main"; // alias used by the source database

            // get the initial state of foreign keys, used to restore foreign key setting at end of merge process
            var foreignKeys = connection.ExecuteScalar<string>("PRAGMA foreign_keys;", null, null);
            connection.ExecuteNonQuery("PRAGMA foreign_keys = off;");

            using (var transaction = connection.BeginTransaction())
            {
                // get a list of all the tables in the database
                var tables = connection.ExecuteScalar<string>(
                    "SELECT group_concat(name) FROM ( " +
                    $"SELECT name FROM {from}.sqlite_master WHERE type='table' " +
                    "UNION  " +
                    $"SELECT name FROM {from}.sqlite_master WHERE type='table' )" +
                    "WHERE name NOT LIKE 'sqlite^_%' ESCAPE '^';", (object[])null, transaction)
                .Split(',');
                try
                {
                    foreach (var table in tables)
                    {
                        if (excluding?.Contains(table) ?? false)
                        { continue; }

                        if (table == "Globals")
                        {
                            connection.ExecuteNonQuery(
$@"INSERT OR IGNORE INTO {to}.{table} (""Block"", ""Key"", ""Value"")
SELECT ""Block"", ""Key"", ""Value""
FROM {from}.{table} 
WHERE ""Block"" != 'Database' AND ""Key"" != 'Version';", null, transaction);
                            continue;
                        }
                        else
                        {

                            // get the union of fields in table from both databases as a comma seperated list
                            // field names are encased in double quotes just incase any field names are sql keywords
                            var fields = connection.ExecuteScalar<string>(
$@"SELECT group_concat('""' || Name || '""', ', ') FROM
(
    SELECT Name FROM {to}.pragma_table_info('{table}') 
    UNION
    SELECT Name FROM {from}.pragma_table_info('{table}')
);", null, transaction);

                            connection.ExecuteNonQuery(
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
                    connection.ExecuteNonQuery($"PRAGMA foreign_keys = {foreignKeys};");
                }
            }
        }
    }
}