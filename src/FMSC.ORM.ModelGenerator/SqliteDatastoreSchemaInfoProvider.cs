using FMSC.ORM.SQLite;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMSC.ORM.ModelGenerator
{
    public class SqliteDatastoreSchemaInfoProvider : ISchemaInfoProvider
    {
        public Datastore Datastore { get; set; }

        public IEnumerable<TableInfo> Tables
        {
            get => GenerateTableInfo(Datastore);
        }

        public SqliteDatastoreSchemaInfoProvider(Datastore datastore)
        {
            Datastore = datastore;
        }

        IEnumerable<TableInfo> GenerateTableInfo(Datastore datastore)
        {
            var tableNamesCSV = datastore.ExecuteScalar<string>("SELECT group_concat(tbl_name) FROM Sqlite_Master WHERE type IN ('table', 'view') AND tbl_name NOT LIKE 'sqlite\\_%' ESCAPE '\\';");
            var tableNames = tableNamesCSV.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach(var tableName in tableNames)
            {
                var tableInfo = new TableInfo() { TableName = tableName };

                var conn = datastore.OpenConnection();

                using (var reader = conn.ExecuteReader($"SELECT * FROM {tableName};", (object[])null, (System.Data.Common.DbTransaction)null))
                {
                    var fieldInfoList = new List<FieldInfo>();
                    var fieldCount = reader.FieldCount;

                    for(int i = 0; i<fieldCount; i++)
                    {
                        var fieldName = reader.GetName(i);
                        var runtimeType = reader.GetFieldType(i);

                        fieldInfoList.Add(new FieldInfo { FieldName = fieldName, RuntimeTimeType = runtimeType });
                    }

                    tableInfo.Fields = fieldInfoList;
                }

                yield return tableInfo;

            }
        }
    }
}
