using Backpack.SqlBuilder;
using Backpack.SqlBuilder.Sqlite;
using FluentAssertions;
using FMSC.ORM.SQLite;
using System.Linq;
using Xunit;

namespace FMSC.ORM.ModelGenerator.Test
{
    public class SqliteDatastoreSchemaInfoProvider_Test
    {
        public SqliteDatastoreSchemaInfoProvider_Test()
        {
            SqlBuilder.DefaultDialect = new SqliteDialect();
        }



        [Fact]
        public void Tables_Test()
        {
            using (var datastore = new SQLiteDatastore())
            {
                var createTable = new CreateTable()
                {
                    TableName = "MyTable",
                    Columns = new[]
                    {
                        new ColumnInfo("col1", SqliteDataType.INTEGER)
                        { AutoIncrement = true, IsPK = true },
                        new ColumnInfo("col2", SqliteDataType.REAL),
                        new ColumnInfo("col3", SqliteDataType.TEXT),
                        new ColumnInfo("col4", SqliteDataType.BOOLEAN),
                        new ColumnInfo("IgnoreMe"),
                    },
                };

                datastore.Execute(createTable.ToString());

                // inserting will trigger the creation of Sqlite_squince table
                var insert = new SqlInsertCommand() { TableName = createTable.TableName };
                datastore.Equals(insert.ToString());

                var schemaInfoProvider = new SqliteDatastoreSchemaInfoProvider(datastore, new[] { "IgnoreMe" });

                var tables = schemaInfoProvider.Tables.ToArray();
                tables.Should().HaveCount(1);

                var myTableInfo = tables.First();
                myTableInfo.PrimaryKeyField.Should().NotBeNull();
                myTableInfo.PrimaryKeyField.IsPK.Should().BeTrue();
                myTableInfo.Fields.Should().HaveSameCount(createTable.Columns.Where(x => x.Name != "IgnoreMe"));
            }
        }
    }
}