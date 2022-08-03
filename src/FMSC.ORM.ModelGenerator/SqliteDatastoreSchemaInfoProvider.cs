using Backpack.SqlBuilder;
using Backpack.SqlBuilder.Sqlite;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            ignoreColumnNames = ignoreColumnNames ?? Enumerable.Empty<string>();

            // note: we are only generating types off of tables because sqlite may fail to reflect the type on columns in views
            var tableNames = datastore.QueryScalar<string>("SELECT tbl_name FROM Sqlite_Master WHERE type = 'table' AND tbl_name NOT LIKE 'sqlite\\_%' ESCAPE '\\' ORDER BY tbl_name;");

            foreach (var tableName in tableNames)
            {
                var tableInfo = ReadTableInfo(datastore, tableName, ignoreColumnNames);
                tableInfo.Type = TableType.Table;
                yield return tableInfo;
            }

            var viewNames = datastore.QueryScalar<string>("SELECT tbl_name FROM Sqlite_Master WHERE type = 'view' AND tbl_name NOT LIKE 'sqlite\\_%' ESCAPE '\\' ORDER BY tbl_name;");

            foreach (var viewName in viewNames)
            {
                var viewInfo = ReadTableInfo(datastore, viewName, ignoreColumnNames);
                viewInfo.Type = TableType.View;
                yield return viewInfo;
            }
        }

        protected TableInfo ReadTableInfo(Datastore datastore, string tableName, IEnumerable<string> ignoreColumnNames)
        {
            var conn = datastore.OpenConnection();

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



            // we have better luck getting fild type for views 
            // by using a datareader
            foreach (var f in fields)
            {
                f.Table = tableName;

                var fieldName = f.FieldName;
                var command = conn.CreateCommand();
                command.CommandText = $"SELECT {fieldName} FROM {tableName};";

                using var reader = command.ExecuteReader();
                var dbType = reader.GetDataTypeName(0);

                // 
                if (dbType == SqliteDataType.BLOB) { continue; }

                f.DbType = dbType;
            }



            var pkField = fields.SingleOrDefault(x => x.IsPK);

            var foreignKeys = datastore.Query<ForeignKeyInfo>(
$@"SELECT [table], group_concat([from]) AS FromFieldNames, group_concat([to]) AS ToFieldNames FROM PRAGMA_FOREIGN_KEY_LIST('{tableName}') GROUP BY [table];").ToArray();


            var tableDDLs = datastore.QueryScalar<string>(
                "SELECT sql FROM sqlite_master WHERE tbl_name = @p1 AND type = 'table' " +
                "UNION ALL " +
                "SELECT sql FROM sqlite_master WHERE tbl_name = @p1 AND type = 'trigger' " +
                "UNION ALL " +
                "SELECT sql FROM sqlite_master WHERE tbl_name = @p1 AND type = 'view' " +
                "UNION ALL " +
                "SELECT sql FROM sqlite_master WHERE tbl_name = @p1 AND type = 'index'; ",
                tableName);

            var ddlSb = new StringBuilder();
            foreach (var tableDDL in tableDDLs)
            {
                ddlSb.AppendLine(tableDDL);
                ddlSb.AppendLine();
            }

            var tableInfo = new TableInfo()
            {
                TableName = tableName,
                Fields = fields,
                PrimaryKeyField = pkField,
                ForeignKeys = foreignKeys,
                DDL = ddlSb.ToString(),
            };

            return tableInfo;
        }
    }


    
}