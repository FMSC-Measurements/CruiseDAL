using CruiseDAL.Schema;
using FMSC.ORM;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL
{
    public partial class Updater_V3 : IUpdater
    {
        public static void RecreateView(CruiseDatastore datastore, IViewDefinition viewDef)
        {
            var viewName = viewDef.ViewName;
            datastore.Execute($"DROP VIEW {viewName};");
            datastore.Execute(viewDef.CreateView);
        }

        // some table alterations can be done by manualy editing the stored create table statments
        // this method IS NOT APROPRIATE for removing or reordering colums, removing unique constraints or removing primary key constraints.
        // this method IS APROPRIATE for removing check, foreign key, not null, or default value constraints. 
        // https://www.sqlite.org/lang_altertable.html#otheralter

        // IMPORTANT: call BeginTransaction before calling this method and EndTransaction after
        //            you may allso need to drop and recreate any views, triggers, or indexes depending on what you have changed on the table
        public static void UpdateTableDDL(CruiseDatastore db, ITableDefinition tableDef)
        {
            var schemaVersion = db.ExecuteScalar<int>("PRAGMA schema_version;");
            db.Execute("PRAGMA writable_schema=ON;");
            var tableName = tableDef.TableName;
            db.Execute("UPDATE sqlite_master SET sql=@p1 WHERE type='table' AND name=@p2", tableDef.CreateTable, tableName);
            db.Execute($"PRAGMA schema_version={schemaVersion + 1};");
            var integrityCheck = db.QueryScalar<string>("PRAGMA main.integrity_check;").ToArray();
            if (integrityCheck.FirstOrDefault() != "ok")
            {
                Logger.Log(String.Join("|", integrityCheck), "Updater_V3", FMSC.ORM.Logging.LogLevel.Trace);
                throw new SchemaException("Integrity Check Failed While Updating Table Definition On " + tableName);
            }
        }


        // IMPORTANT: additional steps must be done before and after calling this method
        // before you must:
        //      1) set PRAGMA foreign_keys=off
        //      2) begin a transaction
        // after you must:
        //      1) commit  the transaction
        //      2) set PRAGMA foreign_keys back to what it was beofore

        // see https://www.sqlite.org/lang_altertable.html#otheralter for more details on the procedure for rebuilding a table.

        public static void RebuildTable(CruiseDatastore db, ITableDefinition tableDef, IEnumerable<KeyValuePair<string, string>> customFieldMaps = null )
        {
            var tableName = tableDef.TableName;

            var tempTableName = "new_" + tableName;
            var createNewTable = tableDef.GetCreateTable(tempTableName);
            db.Execute(createNewTable);

            var fieldIntersectArray = ListFieldsIntersect(db, tableName, tempTableName);

            var fieldListFrom = new List<string>(fieldIntersectArray);
            var fieldListTo = new List<string>(fieldIntersectArray);

            if (customFieldMaps != null)
            {
                foreach (var map in customFieldMaps)
                {
                    var i = fieldListTo.FindIndex(x => string.Compare(x, map.Key, true) is 0);
                    if (i > 0)
                    {
                        fieldListTo[i] = map.Value;
                    }
                    else
                    {
                        fieldListTo.Add(map.Key);
                        fieldListFrom.Add(map.Value);
                    }
                }
            }

            db.Execute($"INSERT INTO main.{tempTableName} ( {fieldListTo.Aggregate((a, b) => a + ", " + b)} ) SELECT {fieldListFrom.Aggregate((a, b) => a + ", " + b)} FROM main.{tableName};");

            db.Execute($"DROP TABLE main.{tableName};");
            db.Execute($"ALTER TABLE {tempTableName} RENAME TO {tableName}");

            var createIndexes = tableDef.CreateIndexes;
            if (createIndexes != null)
            {
                db.Execute(createIndexes);
            }

            var triggers = tableDef.CreateTriggers;
            if (triggers != null)
            {
                foreach (var trigger in triggers)
                {
                    db.Execute(trigger);
                }
            }
        }

        public static void CreateTable(CruiseDatastore db, ITableDefinition tableDef)
        {
            var createCommand = tableDef.CreateTable;
            db.Execute(createCommand);

            var createIndexes = tableDef.CreateIndexes;
            if (createIndexes != null)
            {
                db.Execute(createIndexes);
            }

            var createTombstone = tableDef.CreateTombstoneTable;
            if (createTombstone != null)
            {
                db.Execute(createTombstone);
            }

            var initialize = tableDef.InitializeTable;
            if (initialize != null)
            {
                db.Execute(initialize);
            }

            var triggers = tableDef.CreateTriggers;
            if (triggers != null)
            {
                foreach (var trigger in triggers)
                {
                    db.Execute(trigger);
                }
            }
        }

        public static void Migrate(CruiseDatastore sourceDS, CruiseDatastore destinationDS, IEnumerable<string> excluding = null, bool excludeLookupTables = false)
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

        public static void Migrate(DbConnection sourceConn, DbConnection destConn, IEnumerable<string> excluding = null, bool excludeLookupTables = false)
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
                            if (excludeLookupTables && table.StartsWith("LK_"))
                            { continue; }
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

        public static string[] ListFieldsIntersect(CruiseDatastore db, string table1, string table2)
        {
            var sourceFields = db.QueryScalar2<string>($@"SELECT '""' || Name || '""' FROM pragma_table_info('{table1}');");

            var destFields = db.QueryScalar2<string>($@"SELECT '""' || Name || '""' FROM pragma_table_info('{table2}');");

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

        public static void SetDatabaseVersion(CruiseDatastore db, string newVersion)
        {
            string command = String.Format("UPDATE Globals SET Value = '{0}' WHERE Block = 'Database' AND Key = 'Version';", newVersion);
            db.Execute(command);
            db.LogMessage($"Updated structure version to {newVersion}");
        }
    }
}
