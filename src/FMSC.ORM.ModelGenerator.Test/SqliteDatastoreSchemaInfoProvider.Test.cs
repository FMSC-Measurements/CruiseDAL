using Backpack.SqlBuilder;
using Backpack.SqlBuilder.Dialects;
using FluentAssertions;
using FMSC.ORM.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FMSC.ORM.ModelGenerator.Test
{
    public class SqliteDatastoreSchemaInfoProvider_Test
    {
        [Fact]
        public void Tables_Test()
        {
            using (var datastore = new SQLiteDatastore())
            {
                var createTable = new CreateTable(new SqliteDialect())
                {
                    TableName = "MyTable",
                    Columns = new[]
                    {
                        new ColumnInfo("col1", SqliteDataType.INTEGER)
                        { AutoIncrement = true, IsPK = true },
                        new ColumnInfo("col2", SqliteDataType.REAL),
                        new ColumnInfo("col3", SqliteDataType.TEXT),
                        new ColumnInfo("col4", SqliteDataType.BOOLEAN),
                    },
                    
                };

                
                datastore.Execute(createTable.ToString());

                // inserting will trigger the creation of Sqlite_squince table
                var insert = new SqlInsertCommand() { TableName = createTable.TableName };
                datastore.Equals(insert.ToString());

                var schemaInfoProvider = new SqliteDatastoreSchemaInfoProvider(datastore);

                var tables = schemaInfoProvider.Tables.ToArray();
                tables.Should().HaveCount(1);

                var myTableInfo = tables.First();
                myTableInfo.Fields.Should().HaveSameCount(createTable.Columns);
            }
        }
    }
}
