using Backpack.SqlBuilder;
using CruiseDAL.V3.Sync;
using FMSC.ORM.Core;
using FMSC.ORM.Core.SQL.QueryBuilder;
using FMSC.ORM.EntityModel.Support;
using FMSC.ORM.Logging;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace CruiseDAL
{
    public class CopyTableConfig
    {
        public CopyTableConfig(Type tableType, IEnumerable<string> dependsOn = null, OnConflictOption onConflict = OnConflictOption.Default)
        {
            var description = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(tableType);
            TableName = description.SourceName;
            TableType = tableType;
            DependsOn = dependsOn;
            OnConflictOption = onConflict;
        }

        public CopyTableConfig(string tableName, Action<DbConnection, DbConnection, string, string, OnConflictOption> action, IEnumerable<string> dependsOn = null, OnConflictOption onConflict = OnConflictOption.Default)
        {
            TableName = !string.IsNullOrEmpty(tableName) ? tableName : throw new ArgumentException($"'{nameof(tableName)}' cannot be null or empty.", nameof(tableName));
            Action = action ?? throw new ArgumentNullException(nameof(action));
            DependsOn = dependsOn;
            OnConflictOption = onConflict;
        }

        public CopyTableConfig(Type tableType, Action<QueryBuilder> configRead, IEnumerable<string> dependsOn = null, OnConflictOption onConflict = OnConflictOption.Default)
        {
            var description = GlobalEntityDescriptionLookup.Instance.LookUpEntityByType(tableType);
            TableName = description.SourceName;
            TableType = tableType;
            ConfigRead = configRead ?? throw new ArgumentNullException(nameof(configRead));
            DependsOn = dependsOn;
            OnConflictOption = onConflict;
        }

        public bool CopyTable { get; set; } = true;

        public string TableName { get; set; }
        public OnConflictOption OnConflictOption { get; set; } = OnConflictOption.Default;

        public Type TableType { get; set; }

        public IEnumerable<string> DependsOn { get; set; }

        public Action<DbConnection, DbConnection, string, string, OnConflictOption> Action { get; set; }

        public Action<QueryBuilder> ConfigRead { get; set; }
    }

    public class DatabaseCopier
    {
        public DatabaseCopier()
        {
            Logger = LoggerProvider.Get();
        }

        private ILogger Logger { get; set; }

        public OnConflictOption DefaultOnConflictOption { get; set; }

        public IEnumerable<CopyTableConfig> Tables { get; protected set; }

        public void Copy(DbConnection source, DbConnection destination, string cruiseID, string destinationCruiseID = null)
        {
            foreach (var table in Tables)
            {
                if (table.CopyTable is false)
                {
                    Logger.Log($"Skipped Copying Table {table.TableName}", "DatabaseCopier", LogLevel.Info);
                    continue;
                }

                CopyTable(table, source, destination, cruiseID, destinationCruiseID);
            }
        }

        public void Copy(CruiseDatastore_V3 source, CruiseDatastore_V3 destination, string cruiseID, string destinationCruiseID = null)
        {
            var srcConn = source.OpenConnection();
            try
            {
                var destConn = destination.OpenConnection();
                try
                {
                    destination.BeginTransaction();
                    try
                    {
                        Copy(srcConn, destConn, cruiseID, destinationCruiseID);
                        destination.CommitTransaction();
                    }
                    catch
                    {
                        destination.RollbackTransaction();
                        throw;
                    }
                }
                finally
                {
                    destination.ReleaseConnection();
                }
            }
            finally
            {
                source.ReleaseConnection();
            }
        }

        public bool CheckIsTableConfigValid(out IEnumerable<string> errors)
        {
            errors = null;
            var errorsList = new List<string>();
            var tables = Tables.ToArray();
            var tablesLookup = tables.ToDictionary(x => x.TableName);

            foreach (var table in tables)
            {
                if (table.CopyTable is false) continue;

                var dependsOn = table.DependsOn;
                if (dependsOn != null && dependsOn.Any())
                {
                    foreach (var depend in dependsOn)
                    {
                        if (tablesLookup.ContainsKey(depend))
                        {
                            var dependsTable = tablesLookup[depend];
                            if (dependsTable.CopyTable is false && table.OnConflictOption != OnConflictOption.Ignore)
                            {
                                errorsList.Add($"{table.TableName} Depends On {depend} but could experience errors with missing records");
                            }
                        }
                        else
                        {
                            errorsList.Add($"{table.TableName} Depends On {depend} but is missing");
                        }
                    }
                }
            }

            if (errorsList.Count > 0)
            {
                errors = errorsList.ToArray();
                return false;
            }
            return true;
        }

        public void CopyTable(CopyTableConfig tableConfig, DbConnection source, DbConnection destination, string cruiseID, string destinatinCruiseID = null)
        {
            Logger.Log($"Copying Table {tableConfig.TableName}", "DatabaseCopier", LogLevel.Info);

            var conflictOption = (tableConfig.OnConflictOption == OnConflictOption.Default) ? DefaultOnConflictOption : tableConfig.OnConflictOption;
            try
            {
                if (tableConfig.Action != null)
                {
                    tableConfig.Action(source, destination, cruiseID, destinatinCruiseID, conflictOption);
                }
                else
                {
                    CopyTable(tableConfig.TableType, source, destination, cruiseID, destinatinCruiseID, conflictOption);
                }
            }
            catch (SqliteException ex)
            {
                throw new DatabaseCopyException($"Error Inserting Into {tableConfig.TableName}", tableConfig.TableName, cruiseID, destinatinCruiseID, ex);
            }
        }

        protected void CopyTable(Type tableType, DbConnection source, DbConnection destination, string cruiseID, string destinatinCruiseID = null, OnConflictOption option = OnConflictOption.Default, Action<QueryBuilder> configRead = null)
        {
            IDictionary<string, object> valueOverrides = GetValueOverrides(destinatinCruiseID);

            var query = source.From(tableType);
            if (configRead != null)
            {
                configRead(query);
            }
            else
            {
                query.Where("CruiseID = @p1");
            }

            var sourceRecs = query.Query(cruiseID);

            foreach (var rec in sourceRecs)
            {
                destination.Insert(rec,
                    option: option,
                    valueOverrides: valueOverrides,
                    persistKeyvalue: false); // using persisKeyValue = false, generally primary keys are RowID fields and are not used for fKeys
            }
        }

        protected IDictionary<string, object> GetValueOverrides(string destinatinCruiseID)
        {
            if (destinatinCruiseID != null)
            {
                return new Dictionary<string, object>
                {
                    { "CruiseID", destinatinCruiseID }
                };
            }
            return null;
        }
    }
}