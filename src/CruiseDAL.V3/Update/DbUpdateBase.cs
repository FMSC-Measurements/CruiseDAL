using CruiseDAL.Schema;
using FMSC.ORM;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL.Update
{
    public abstract class DbUpdateBase : IDbUpdate
    {
        private static object _connSyncLock = new Object();

        public string TargetVersion { get; set; }
        public IEnumerable<string> SourceVersions { get; set; }

        protected DbUpdateBase(string targetVersion, IEnumerable<string> sourceVersions)
        {
            TargetVersion = targetVersion;
            SourceVersions = sourceVersions;
        }

        public virtual void Update(DbConnection conn, IExceptionProcessor exceptionProcessor = null)
        {
            var syncLock = GetSyncLock(conn);
            lock (syncLock)
            {
                BeforeBegin(conn, exceptionProcessor);
                var transaction = conn.BeginTransaction();
                try
                {
                    DoUpdate(conn, transaction, exceptionProcessor);
                    SetDatabaseVersion(conn, transaction);
                    transaction.Commit();
                    AfterCommit(conn, exceptionProcessor);
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new SchemaUpdateException("", TargetVersion, e);
                }
            }
        }

        protected abstract void DoUpdate(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor);

        protected virtual void BeforeBegin(DbConnection conn, IExceptionProcessor exceptionProcessor)
        { }

        protected virtual void AfterCommit(DbConnection conn, IExceptionProcessor exceptionProcessor)
        { }

        protected object GetSyncLock(DbConnection conn)
        {
            return _connSyncLock;
        }

        protected void SetDatabaseVersion(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor = null)
        {
            var targetVersion = TargetVersion;
            conn.ExecuteNonQuery($"UPDATE Globals SET Value = '{targetVersion}' WHERE Block = 'Database' AND Key = 'Version';",
                transaction: transaction, exceptionProcessor: exceptionProcessor);

            conn.LogMessage($"Updated structure version to {targetVersion}");
        }

        public static void CreateTable(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor, ITableDefinition tableDef)
        {
            var createCommand = tableDef.CreateTable;
            conn.ExecuteNonQuery(createCommand);

            var createIndexes = tableDef.CreateIndexes;
            if (createIndexes != null)
            {
                conn.ExecuteNonQuery(createIndexes,
                        transaction: transaction, exceptionProcessor: exceptionProcessor);
            }

            var createTombstone = tableDef.CreateTombstoneTable;
            if (createTombstone != null)
            {
                conn.ExecuteNonQuery(createTombstone,
                        transaction: transaction, exceptionProcessor: exceptionProcessor);
            }

            var initialize = tableDef.InitializeTable;
            if (initialize != null)
            {
                conn.ExecuteNonQuery(initialize,
                        transaction: transaction, exceptionProcessor: exceptionProcessor);
            }

            var triggers = tableDef.CreateTriggers;
            if (triggers != null)
            {
                foreach (var trigger in triggers)
                {
                    conn.ExecuteNonQuery(trigger,
                            transaction: transaction, exceptionProcessor: exceptionProcessor);
                }
            }
        }

        // IMPORTANT: additional steps must be done before and after calling this method
        // before you must:
        //      1) set PRAGMA foreign_keys=off
        //      2) begin a transaction
        // after you must:
        //      1) commit  the transaction
        //      2) set PRAGMA foreign_keys back to what it was before

        // see https://www.sqlite.org/lang_altertable.html#otheralter for more details on the procedure for rebuilding a table.
        public static void RebuildTable(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor, ITableDefinition tableDef, IEnumerable<KeyValuePair<string, string>> customFieldMaps = null)
        {
            var tableName = tableDef.TableName;

            var tempTableName = "new_" + tableName;
            var createNewTable = tableDef.GetCreateTable(tempTableName);
            conn.ExecuteNonQuery(createNewTable, transaction: transaction, exceptionProcessor: exceptionProcessor);

            var fieldIntersectArray = ListFieldsIntersect(conn, tableName, tempTableName, transaction: transaction, exceptionProcessor: exceptionProcessor);

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

            conn.ExecuteNonQuery($"INSERT INTO main.{tempTableName} ( {fieldListTo.Aggregate((a, b) => a + ", " + b)} ) SELECT {fieldListFrom.Aggregate((a, b) => a + ", " + b)} FROM main.{tableName};");

            conn.ExecuteNonQuery($"DROP TABLE main.{tableName};");
            conn.ExecuteNonQuery($"ALTER TABLE {tempTableName} RENAME TO {tableName}");

            var createIndexes = tableDef.CreateIndexes;
            if (createIndexes != null)
            {
                conn.ExecuteNonQuery(createIndexes, transaction: transaction, exceptionProcessor: exceptionProcessor);
            }

            var triggers = tableDef.CreateTriggers;
            if (triggers != null)
            {
                foreach (var trigger in triggers)
                {
                    conn.ExecuteNonQuery(trigger, transaction: transaction, exceptionProcessor: exceptionProcessor);
                }
            }
        }

        // some table alterations can be done by manually editing the stored create table statements
        // this method IS NOT APROPRIATE for removing or reordering columns, removing unique constraints or removing primary key constraints.
        // this method IS APROPRIATE for removing check, foreign key, not null, or default value constraints. 
        // https://www.sqlite.org/lang_altertable.html#otheralter

        // IMPORTANT: call BeginTransaction before calling this method and EndTransaction after
        //            you may also need to drop and recreate any views, triggers, or indexes depending on what you have changed on the table
        public static void UpdateTableDDL(DbConnection conn, DbTransaction transaction, IExceptionProcessor exceptionProcessor, ITableDefinition tableDef)
        {
            var schemaVersion = conn.ExecuteScalar<int>("PRAGMA schema_version;", transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery("PRAGMA writable_schema=ON;", transaction: transaction, exceptionProcessor: exceptionProcessor);
            var tableName = tableDef.TableName;
            conn.ExecuteNonQuery("UPDATE sqlite_master SET sql=@p1 WHERE type='table' AND name=@p2", new object[] { tableDef.CreateTable, tableName }, transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery($"PRAGMA schema_version={schemaVersion + 1};", transaction: transaction, exceptionProcessor: exceptionProcessor);
            var integrityCheck = conn.QueryScalar<string>("PRAGMA main.integrity_check;", transaction: transaction, exceptionProcessor: exceptionProcessor)
                .ToArray();
            if (integrityCheck.FirstOrDefault() != "ok")
            {
                //Logger.Log(String.Join("|", integrityCheck), "Updater_V3", FMSC.ORM.Logging.LogLevel.Trace);
                throw new SchemaException("Integrity Check Failed While Updating Table Definition On " + tableName);
            }
        }

        public static string[] ListFieldsIntersect(DbConnection conn, string table1, string table2, DbTransaction transaction = null, IExceptionProcessor exceptionProcessor = null)
        {
            var sourceFields = conn.QueryScalar2<string>($@"SELECT '""' || Name || '""' FROM pragma_table_info('{table1}');",
                transaction: transaction, exceptionProcessor: exceptionProcessor);

            var destFields = conn.QueryScalar2<string>($@"SELECT '""' || Name || '""' FROM pragma_table_info('{table2}');",
                transaction: transaction, exceptionProcessor: exceptionProcessor);

            var both = sourceFields.Intersect(destFields).ToArray();
            return both;
        }

        public static void RecreateView(DbConnection conn, IViewDefinition viewDef, DbTransaction transaction = null, IExceptionProcessor exceptionProcessor = null)
        {
            var viewName = viewDef.ViewName;
            conn.ExecuteNonQuery($"DROP VIEW {viewName};", transaction: transaction, exceptionProcessor: exceptionProcessor);
            conn.ExecuteNonQuery(viewDef.CreateView, transaction: transaction, exceptionProcessor: exceptionProcessor);
        }
    }
}