using Backpack.SqlBuilder;
using Backpack.SqlBuilder.Sqlite;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FMSC.ORM.ModelGenerator
{
    public class SqliteDatastoreSchemaInfoProvider : ISchemaInfoProvider
    {
        public Datastore Datastore { get; set; }
        public ISqlDialect Dialect { get; }
        public IEnumerable<string> IgnoreColumns { get; }

        public IEnumerable<TableInfo> Tables
        {
            get => GenerateTableInfo(Datastore, IgnoreColumns);
        }

        public SqliteDatastoreSchemaInfoProvider(Datastore datastore, IEnumerable<string> ignoreColumns)
        {
            Datastore = datastore;
            Dialect = new SqliteDialect();
            IgnoreColumns = ignoreColumns;
        }

        private IEnumerable<TableInfo> GenerateTableInfo(Datastore datastore, IEnumerable<string> ignoreColumnNames = null)
        {
            var dialect = Dialect;
            ignoreColumnNames = ignoreColumnNames ?? new string[0];

            // note: we are only generating types off of tables because sqlite may fail to reflect the type on columns in views
            var tableNames = datastore.QueryScalar<string>("SELECT tbl_name FROM Sqlite_Master WHERE type IN ('table') AND tbl_name NOT LIKE 'sqlite\\_%' ESCAPE '\\';");

            foreach (var tableName in tableNames)
            {
                //var tblInfo = datastore.GetTableInfo(tableName);
                //var foreignkeys = datastore.Query<ForeignKeyInfo>($"PRAGMA foreign_key_list('{tableName}');");

                //var fieldInfo = tblInfo.Where(x => ignoreColumnNames.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase) == false)
                //    .Select(x =>
                //        new FieldInfo
                //        {
                //            FieldName = x.Name,
                //            IsPK = x.IsPK,
                //            RuntimeTimeType = dialect.Try(
                //                y => y.MapSQLtypeToSystemType(x.Type),
                //                (z, e) => { Console.WriteLine($"table:{tableName}| field:{x.Name}| {x.Type ?? "null"}| {e.Message}"); return typeof(string); }),

                //            //HACK make an assumption that the field is AutoIncr.
                //            //IsAutoIncr = x.Type == SqliteDataType.INTEGER && x.IsPK && x.NotNull == false,
                //            NotNull = x.NotNull,
                //        }).ToList();

                //var pkField = fieldInfo.Where(x => x.IsPK).SingleOrDefault();

                var fields = datastore.Query<FieldInfo>(
$@"
SELECT * FROM  
PRAGMA_TABLE_INFO('{tableName}') AS ti 
Left JOIN 
(SELECT *, 1 AS IsFK, [from] as name, group_concat([table]) AS fkReferences FROM PRAGMA_FOREIGN_KEY_LIST('{tableName}') group by name) AS fkl
using (name);")
                    .Where(x => ignoreColumnNames.Contains(x.FieldName, StringComparer.InvariantCultureIgnoreCase) == false)
                    .ToArray();

                var pkField = fields.SingleOrDefault(x => x.IsPK);

                var foreignKeys = datastore.Query<ForeignKeyInfo>(
$@"SELECT [table], group_concat([from]) AS FromFieldNames, group_concat([to]) AS ToFieldNames FROM PRAGMA_FOREIGN_KEY_LIST('{tableName}') GROUP BY [table];").ToArray();

                var tableInfo = new TableInfo()
                {
                    TableName = tableName,
                    Fields = fields,
                    PrimaryKeyField = pkField,
                    ForeignKeys = foreignKeys,
                };

                //var conn = datastore.OpenConnection();

                //using (var reader = conn.ExecuteReader($"SELECT * FROM {tableName};", (object[])null, (System.Data.Common.DbTransaction)null))
                //{
                //    var fieldInfoList = new List<FieldInfo>();
                //    var fieldCount = reader.FieldCount;

                //    for (int i = 0; i < fieldCount; i++)
                //    {
                //        var fieldName = reader.GetName(i);
                //        var runtimeType = reader.GetFieldType(i);

                //        fieldInfoList.Add(new FieldInfo { FieldName = fieldName, RuntimeTimeType = runtimeType });
                //    }

                //    tableInfo.Fields = fieldInfoList;
                //}

                yield return tableInfo;
            }
        }
    }


    
}