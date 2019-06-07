using Backpack.SqlBuilder;
using FluentAssertions;
using FMSC.ORM.Core;
using FMSC.ORM.Test.TestSupport.TestModels;
using FMSC.ORM.TestSupport;
using FMSC.ORM.TestSupport.TestModels;
using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.SQLite
{
    public class SQLiteDatastoreTest : TestBase, IDisposable
    {
        private readonly string _tempDir;
        private readonly string _testCreatePath;
        private readonly string _testReadOnlyPath;

        public SQLiteDatastoreTest(ITestOutputHelper output) : base(output)
        {
            _tempDir = System.IO.Path.GetTempPath();
            _testCreatePath = System.IO.Path.Combine(_tempDir, "testCreate.db");

            _testReadOnlyPath = System.IO.Path.Combine(_tempDir, "testReadOnly.db");
        }

        public void Dispose()
        {
            if (System.IO.File.Exists(_testCreatePath))
            {
                System.IO.File.Delete(_testCreatePath);
            }

            if (System.IO.File.Exists(_testReadOnlyPath))
            {
                System.IO.File.SetAttributes(_testReadOnlyPath, System.IO.FileAttributes.Normal);
                System.IO.File.Delete(_testReadOnlyPath);
            }
        }

        [Fact]
        public void Ctor_with_null_path()
        {
            Action action = () =>
            {
                var db = new SQLiteDatastore(null);
            };
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Ctor_inMemory()
        {
            using (var db = new SQLiteDatastore())
            {
                db.Path.Should().NotBeNullOrWhiteSpace();
                db.Execute("Select 1;");
            }
        }

        [Fact]
        public void Ctor_with_empty_path()
        {
            Action action = () =>
            {
                var db = new SQLiteDatastore("");
            };
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ReleaseConnection_force()
        {
            using (var db = new SQLiteDatastore())
            {
                var connection = db.OpenConnection();

                var trans = connection.BeginTransaction();
                var result = connection.ExecuteScalar("SELECT 1;", null, trans);
                //trans.Commit();

                db.ReleaseConnection(true);

                try
                {
                    connection.State.Should().Be(ConnectionState.Closed);
                }
                catch (ObjectDisposedException) { }
            }
        }

        [Fact]
        public void CreateSQLiteDatastoreTest()
        {
            Output.WriteLine("Path: " + _testCreatePath);
            var dbBuilder = new TestDBBuilder();

            using (var ds = new SQLiteDatastore(_testCreatePath))
            {
                Assert.False(ds.Exists, "Pre-condition failed file already created");

                //TODO hide builder inside dataStore
                dbBuilder.CreateDatastore(ds);

                ds.Extension.Should().NotBeNullOrWhiteSpace("file must have extention");
                ds.Path.Should().NotBeNullOrWhiteSpace();

                VerifySQLiteDatastore(ds);
            }

            File.Exists(_testCreatePath + "-journal").Should().BeFalse();
            var file = new FileInfo(_testCreatePath);
            file.Invoking(f => f.Delete()).Should().NotThrow();
        }

        [Fact]
        public void CreateInmemorySQLiteDatastoreTest()
        {
            var dbBuilder = new TestDBBuilder();

            using (var ds = new SQLiteDatastore())
            {
                Assert.True(ds.Exists);

                //TODO hide builder inside dataStore
                dbBuilder.CreateDatastore(ds);

                VerifySQLiteDatastore(ds);
            }
        }

        protected void VerifySQLiteDatastore(SQLiteDatastore ds)
        {
            ds.Should().NotBeNull();
            ds.Exists.Should().BeTrue();

            ds.Invoking(x => x.Execute("EXPLAIN SELECT 1;")).Should().NotThrow();

            ds.GetRowCount("sqlite_master", null, null).Should().BeGreaterThan(0);
        }

        [Fact]
        public void AddFieldTest()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.Execute("CREATE TABLE TableA (ID INTEGER PRIMARY KEY);");
                Assert.True(ds.CheckFieldExists("TableA", "ID"));
                Assert.False(ds.CheckFieldExists("TABLEA", "Data"));

                ds.AddField("TableA", new ColumnInfo() { Name = "Data", DBType = System.Data.DbType.AnsiString });

                Assert.True(ds.CheckFieldExists("TABLEA", "Data"));

                Assert.Throws<SQLException>(() => ds.AddField("TableA", new ColumnInfo() { Name = "Data", DBType = System.Data.DbType.AnsiString }));
            }
        }

        [Fact]
        public void CheckTableExistsTest()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.Execute("CREATE TABLE tbl (col1 TEXT);");

                ds.CheckTableExists("tbl");
                ds.CheckTableExists("notATable").Should().BeFalse();
            }
        }

        [Fact]
        public void CheckFieldExistsTest()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.Execute("CREATE TABLE TableA (ID INTEGER PRIMARY KEY);");
                Assert.True(ds.CheckFieldExists("TableA", "id"));
                //test ignores case
                Assert.True(ds.CheckFieldExists("TableA", "ID"));
                //test ignores white space
                Assert.True(ds.CheckFieldExists("TableA", " ID"));
                Assert.False(ds.CheckFieldExists("TABLEA", "data"));
            }
        }

        [Fact]
        public void CreateTableTest()
        {
            using (var ds = new SQLiteDatastore())
            {
                var cols = new ColumnInfo[]
                    {
                        new ColumnInfo("ID", System.Data.DbType.Int64) {AutoIncrement = true },
                        new ColumnInfo("Field1", System.Data.DbType.AnsiString)
                    };

                ds.CreateTable("TableA", cols, false);

                ds.Invoking(x => x.Execute("EXPLAIN SELECT * FROM TableA;")).Should().NotThrow();

                ds.CheckTableExists("TableA").Should().BeTrue();
            }
        }

        [Fact]
        public void GetTableInfoTest()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.Execute(TestDBBuilder.CREATE_MULTIPROPTABLE);

                var ti = ds.GetTableInfo("MultiPropTable");

                ti.Should().NotBeNullOrEmpty();

                foreach (string fieldName in TestSQLConstants.MULTI_PROP_TABLE_FIELDS)
                {
                    ti.Should().Contain(x => x.Name.ToLower() == fieldName.ToLower());
                }
            }
        }

        [Fact]
        public void HasForeignKeyErrors()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.Execute("PRAGMA foreign_keys = off;");
                ds.Execute("Create table TableA (ID INTEGER PRIMARY KEY);");
                ds.Execute("CREATE TABLE TABLEB (ID_B REFERENCES TABLEA (ID));");

                ds.Execute("INSERT INTO TABLEA ([ID]) VALUES (1);");
                ds.Execute("INSERT INTO TABLEB VALUES (1);");
                Assert.False(ds.HasForeignKeyErrors("TableB"));
                ds.Execute("INSERT INTO TABLEB VALUES (2);");
                Assert.True(ds.HasForeignKeyErrors("TableB"));
            }
        }

        [Fact]
        public void GetRowCountTest()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.CreateTable("something", new ColumnInfo[] { new ColumnInfo("data") });

                var rowCnt = ds.GetRowCount("something", null);
                Assert.True(rowCnt == 0);

                ds.Execute("INSERT INTO something DEFAULT VALUES");

                rowCnt = ds.GetRowCount("something", null);
                Assert.Equal(1, rowCnt);
            }
        }

        [Fact]
        public void GetTableSQLTest()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.CreateTable("something", new ColumnInfo[] { new ColumnInfo("data") });

                var tableSQL = ds.GetTableSQL("something");
                tableSQL.Should().NotBeNullOrWhiteSpace();
                Output.WriteLine(tableSQL);
            }
        }

        [Fact]
        public void GetLastInsertRowID()
        {
            using (var ds = new SQLiteDatastore())
            {
                using (var connection = ds.OpenConnection())
                {
                    connection.ExecuteNonQuery("CREATE TABLE tbl (id INTEGER PRIMARY KEY AUTOINCREMENT, col1 TEXT);", null, null);

                    connection.ExecuteNonQuery("INSERT INTO tbl (col1) VALUES ('something');", null, null);

                    ds.GetLastInsertRowID(connection, (DbTransaction)null).Should().Be(1);
                }
            }
        }

        [Fact]
        public void GetLastInsertKeyValue()
        {
            using (var ds = new SQLiteDatastore())
            {
                using (var connection = ds.OpenConnection())
                {
                    connection.ExecuteNonQuery("CREATE TABLE tbl (id TEXT PRIMARY KEY);", null, null);

                    connection.ExecuteNonQuery("INSERT INTO tbl (id) VALUES ('something');", null, null);
                    ds.GetLastInsertKeyValue(connection, "tbl", "id", null);
                }
            }
        }

        [Fact]
        public void SetTableAutoIncrementStartTest()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.Execute("CREATE TABLE tbl (id INTEGER PRIMARY KEY AUTOINCREMENT);");
                ds.ExecuteScalar<int>("SELECT max(id) FROM tbl;").Should().Be(0);
                ds.Execute("INSERT INTO tbl DEFAULT VALUES;");
                ds.ExecuteScalar<int>("SELECT max(id) FROM tbl;").Should().Be(1);

                ds.SetTableAutoIncrementStart("tbl", 100);
                ds.ExecuteScalar<int>("SELECT max(id) FROM tbl;").Should().Be(1);
                ds.Execute("INSERT INTO tbl DEFAULT VALUES;");
                ds.ExecuteScalar<int>("SELECT max(id) FROM tbl;").Should().Be(101);
            }
        }

        //[Fact]
        //public void SetTableAutoIncrementStartTest()
        //{
        //    using (var ds = new SQLiteDatastore())
        //    {
        //        ds.Execute(TestDBBuilder.CREATE_AUTOINCREMENT_TABLE);

        //        var seq = ds.From<SQLiteSequenceModel>()
        //            .Where("name = 'AutoIncrementTable'").Query().FirstOrDefault();

        //        Assert.Null(seq);

        //        ds.Insert(new AutoIncrementTable() { Data = "something" }, OnConflictOption.Fail);

        //        seq = ds.From<SQLiteSequenceModel>()
        //            .Where("name = 'AutoIncrementTable'").Query().FirstOrDefault();

        //        Assert.NotNull(seq);
        //        Assert.Equal(1, seq.Seq);

        //        var incVal = 200;
        //        ds.SetTableAutoIncrementStart("AutoIncrementTable", incVal);

        //        seq = ds.From<SQLiteSequenceModel>()
        //            .Where("name = 'AutoIncrementTable'").Query().FirstOrDefault();

        //        Assert.NotNull(seq);
        //        Assert.Equal(incVal, seq.Seq);

        //        ds.Insert(new AutoIncrementTable() { Data = "something" }, OnConflictOption.Fail);

        //        seq = ds.From<SQLiteSequenceModel>()
        //            .Where("name = 'AutoIncrementTable'").Query().FirstOrDefault();

        //        Assert.NotNull(seq);
        //        Assert.Equal(incVal + 1, seq.Seq);
        //    }
        //}

        [Fact]
        public void ReadOnly_Throws_On_Insert()
        {
            var path = _testReadOnlyPath;

            Output.WriteLine(path);

            using (var ds = new SQLiteDatastore(path))
            {
                ds.Execute(TestDBBuilder.CREATE_AUTOINCREMENT_TABLE);
                ds.CreateTable("Tbl", new ColumnInfo[] { new ColumnInfo() { Name = "Data", DBType = System.Data.DbType.String } }, false);

                System.IO.File.Exists(path).Should().BeTrue();
                System.IO.File.SetAttributes(path, System.IO.FileAttributes.ReadOnly);

                //TODO assert that connection is not open and transaction depth is 0
                ds.Invoking(x => x.Execute("INSERT INTO Tbl (Data) VALUES ('something');")).Should().Throw<ReadOnlyException>();
            }
        }

        [Fact
#if MICROSOFT_DATA_SQLITE
             (Skip = "not supported my Microsoft.Data.Sqlite")
#endif
             ]
        public void ReadOnly_Throws_On_BeginTransaction()
        {
            var path = _testReadOnlyPath;

            Output.WriteLine(path);

            using (var ds = new SQLiteDatastore(path))
            {
                ds.Execute(TestDBBuilder.CREATE_AUTOINCREMENT_TABLE);
                ds.CreateTable("Tbl", new ColumnInfo[] { new ColumnInfo() { Name = "Data", DBType = System.Data.DbType.String } }, false);

                System.IO.File.Exists(path).Should().BeTrue();
                System.IO.File.SetAttributes(path, System.IO.FileAttributes.ReadOnly);

                ds.Invoking(x => x.BeginTransaction()).Should().Throw<ReadOnlyException>();
            }
        }

        //[Fact] // GetTableUniques needs to be scrapped
        //public void GetTableUniquesTest()
        //{
        //    using (var ds = _fixture.StaticDatastore)
        //    {
        //        var tableUniques = ds.GetTableUniques("MultiPropTable");
        //        Assert.Empty(tableUniques);
        //    }

        //    using (var ds = new SQLiteDatastore())
        //    {
        //        ds.BeginTransaction();
        //        try
        //        {
        //            ds.Execute(
        //                "CREATE TABLE TableA " +
        //                "( ID INTEGER PRIMARY KEY AUTOINCREMENT, " +
        //                "FIELD1 TEXT, " +
        //                "FIELD2 TEXT, " +
        //                "UNIQUE (FIELD1, FIELD2));");

        //            var uniques = ds.GetTableUniques("TableA").ToArray();
        //            base._output.WriteLine(string.Join(",", uniques));
        //            Assert.Contains(uniques, x => string.Equals(x, "FIELD1"));
        //            Assert.Contains(uniques, x => string.Equals(x, "FIELD2"));
        //        }
        //        finally
        //        {
        //            ds.CancelTransaction();
        //        }
        //    }
        //}

        [Fact]
        public void ExecuteScalar_Generic()
        {
            using (var ds = new SQLiteDatastore())
            {
                VerifyExecuteScalarWithType<int>(1, ds);

                VerifyExecuteScalarWithType<long>(1, ds);

                VerifyExecuteScalarWithType<bool>(true, ds);

                VerifyExecuteScalarWithType<decimal>(1, ds);

                VerifyExecuteScalarWithType<DateTime>(DateTime.Today, ds);

                //TODO support types that don't implement IConvertible
                //VerifyExecuteScalarWithType<Guid>(Guid.Empty, ds);
                //VerifyExecuteScalarWithType<Guid>(Guid.NewGuid(), ds);
            }
        }

        [Theory]
        [InlineData("SELECT 1;", "1")]
        [InlineData("SELECT NULL;", (string)null)]
        [InlineData("SELECT 'something';", "something")]
        [InlineData("SELECT CAST(1 AS INTEGER);", "1")]
        public void ExecuteScalar_Generic_String(string query, string expectedValue)
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.ExecuteScalar<string>(query).Should().Be(expectedValue);
            }
        }

        [Fact]
        public void ExecuteScalar_Generic_Guid()
        {
            var query = "SELECT @p1;";
            var expectedValue = Guid.NewGuid();

            using (var ds = new SQLiteDatastore())
            {
                ds.ExecuteScalar<Guid>(query, expectedValue).Should().Be(expectedValue);
            }
        }

        private void VerifyExecuteScalarWithType<T>(T expected, SQLiteDatastore ds)
            where T : struct
        {
            Output.WriteLine("testing value {0} : {1}", expected, typeof(T).Name);

            T result = ds.ExecuteScalar<T>("SELECT @p1;", expected);
            Output.WriteLine("     as {0} gives {1}", typeof(T).Name, result.ToString());
            Assert.Equal(expected, result);

            result = ds.ExecuteScalar<T>("SELECT NULL;");
            Output.WriteLine("     NULL as {0} gives {1}", typeof(T).Name, result.ToString());
            Assert.Equal(default(T), result);

            T? nResult = ds.ExecuteScalar<T?>("SELECT @p1;", expected);
            Output.WriteLine("     as {0} gives {1}", typeof(T?).Name, nResult.ToString());
            Assert.True(nResult.HasValue);
            Assert.Equal(expected, nResult.Value);

            nResult = ds.ExecuteScalar<T?>("SELECT NULL;");
            Output.WriteLine("     NULL as {0} gives {1}", typeof(T?).Name, nResult.ToString());
            Assert.False(nResult.HasValue);
        }

        //[Fact]
        //public void QuerySingleRecordTest()
        //{
        //    using (var ds = new SQLiteDatastore())
        //    {
        //        ds.Execute(TestDBBuilder.CREATE_MULTIPROPTABLE);

        //        var setup = "INSERT INTO MultiPropTable (IntField) VALUES (1);";
        //        ds.Execute(setup);

        //        Assert.Equal(1, ds.GetRowCount("MultiPropTable", "WHERE IntField = 1"));

        //        var result = ds.QuerySingleRecord<POCOMultiTypeObject>(new WhereClause("IntField = 1"));
        //        Assert.NotNull(result);
        //        Assert.Equal(1, result.IntField);

        //        var result1 = ds.QuerySingleRecord<POCOMultiTypeObject>(new WhereClause("IntField = 1"));
        //        Assert.NotSame(result, result1);
        //        Assert.Equal(1, result1.IntField);
        //    }
        //}

        //[Fact]
        //public void QueryTest()
        //{
        //    int recordsToCreate = 1000;
        //    using (var ds = new SQLiteDatastore())
        //    {
        //        ds.Execute(TestDBBuilder.CREATE_MULTIPROPTABLE);
        //        ds.BeginTransaction();
        //        for (int i = 1; i <= recordsToCreate; i++)
        //        {
        //            ds.Execute(string.Format(" INSERT INTO MultiPropTable (IntField) VALUES ({0});\r\n", i));
        //        }
        //        ds.CommitTransaction();

        //        //ds.Execute(setup.ToString());

        //        Assert.Equal(recordsToCreate, ds.GetRowCount("MultiPropTable", null));

        //        StartTimer();
        //        var result = ds.Query<POCOMultiTypeObject>((WhereClause)null);
        //        EndTimer();
        //        Assert.Equal(recordsToCreate, result.Count);
        //    }
        //}

        [Fact]
        public void FluentInterfaceTest()
        {
            int recordsToCreate = 1000;

            using (var ds = new SQLiteDatastore())
            {
                ds.Execute(TestDBBuilder.CREATE_MULTIPROPTABLE);
                ds.BeginTransaction();
                for (int i = 1; i <= recordsToCreate; i++)
                {
                    ds.Execute(string.Format(" INSERT INTO MultiPropTable (IntField, NIntField) VALUES ({0}, {0});\r\n", i));
                }
                ds.CommitTransaction();

                Assert.Equal(recordsToCreate, ds.GetRowCount("MultiPropTable", null));

                StartTimer();
                var result = ds.From<POCOMultiTypeObject>().Limit(5000, 0).Query();
                EndTimer();

                result.Should().NotBeEmpty();
                result.Should().HaveCount(recordsToCreate);

                foreach (DOMultiPropType item in
                    ds.From<DOMultiPropType>().Read())
                {
                    item.FloatField = 1.0F;
                    ds.Update(item);
                }
            }
        }

        [Fact]
        public void QueryRowIDAsPrimaryKeyObject()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.Execute(RowIDAsPrimaryKey.CREATE_TABLE_COMMAND);

                ds.Insert(new RowIDAsPrimaryKey { StringField = "something" });

                var result = ds.From<RowIDAsPrimaryKey>().Query().First();
                result.RowID.Should().Be(1);
            }
        }

        [Fact]
        public void CommitTransaction_WtihNoTransaction()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.Execute("CREATE TABLE TableA (Data TEXT);");

                ds.CurrentTransaction.Should().BeNull();
                ds.CommitTransaction();//extra commit should not throw exception, but will fail Debug.Assert

                ds.CheckTableExists("TableA").Should().BeTrue();
            }
        }

        [Fact]
        public void CommitTransaction_WtihTransaction()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.CurrentTransaction.Should().BeNull();

                ds.BeginTransaction();

                ds.CurrentTransaction.Should().NotBeNull();

                ds.Execute("CREATE TABLE TableA (Data TEXT);");

                ds.CommitTransaction();

                ds.CurrentTransaction.Should().BeNull();
                ds.TransactionDepth.Should().Be(0);

                ds.CheckTableExists("TableA").Should().BeTrue();
            }
        }

        [Fact]
        public void Read_Empty_Table()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.Execute(TestDBBuilder.CREATE_MULTIPROPTABLE);

                ds.CheckTableExists("MultiPropTable").Should().BeTrue();

                ds.From<POCOMultiTypeObject>().Invoking(x => x.Query()).Should().NotThrow();
            }
        }

        [Fact]
        public void RollBackTransaction_WtihNoTransaction()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.Execute("CREATE TABLE TableA (Data TEXT);");

                ds.CurrentTransaction.Should().BeNull();
                ds.RollbackTransaction();//extra rollback should not throw exception, but will fail Debug.Assert
                ds.TransactionDepth.Should().Be(0);

                ds.CheckTableExists("TableA").Should().BeTrue();
            }
        }

        [Fact]
        public void RollBackTransaction_WtihTransaction()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.BeginTransaction();

                ds.Execute("CREATE TABLE TableA (Data TEXT);");

                ds.RollbackTransaction();

                ds.CheckTableExists("TableA").Should().BeFalse();
            }
        }

        [Fact]
        public void NestedTransactionTest_RollbackThenCommit()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.CurrentTransaction.Should().BeNull();
                ds.TransactionDepth.Should().Be(0);

                ds.Execute("CREATE TABLE TableA (Data TEXT);");

                ds.BeginTransaction();

                ds.CurrentTransaction.Should().NotBeNull();
                ds.TransactionDepth.Should().Be(1);

                ds.BeginTransaction();

                ds.CurrentTransaction.Should().NotBeNull();
                ds.TransactionDepth.Should().Be(2);

                ds.Execute("INSERT INTO TableA VALUES ('something');");

                ds.RollbackTransaction();
                ds.GetRowCount("TableA", null).Should().Be(1);
                ds.CommitTransaction();
                ds.GetRowCount("TableA", null).Should().Be(0);
            }
        }

        [Fact]
        public void NestedTransactionTest_CommitThenRollback()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.CurrentTransaction.Should().BeNull();
                ds.TransactionDepth.Should().Be(0);

                ds.Execute("CREATE TABLE TableA (Data TEXT);");

                ds.BeginTransaction();

                ds.CurrentTransaction.Should().NotBeNull();
                ds.TransactionDepth.Should().Be(1);

                ds.BeginTransaction();

                ds.CurrentTransaction.Should().NotBeNull();
                ds.TransactionDepth.Should().Be(2);

                ds.Execute("INSERT INTO TableA VALUES ('something');");

                ds.CommitTransaction();
                ds.GetRowCount("TableA", null).Should().Be(1);
                ds.RollbackTransaction();
                ds.GetRowCount("TableA", null).Should().Be(0);
            }
        }

        [Fact]
        public void NestedTransactionTest_FullCommit()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.CurrentTransaction.Should().BeNull();
                ds.TransactionDepth.Should().Be(0);

                ds.Execute("CREATE TABLE TableA (Data TEXT);");

                ds.BeginTransaction();

                ds.CurrentTransaction.Should().NotBeNull();
                ds.TransactionDepth.Should().Be(1);

                ds.BeginTransaction();

                ds.CurrentTransaction.Should().NotBeNull();
                ds.TransactionDepth.Should().Be(2);

                ds.Execute("INSERT INTO TableA VALUES ('something');");

                ds.CommitTransaction();
                ds.GetRowCount("TableA", null).Should().Be(1);

                ds.CurrentTransaction.Should().NotBeNull();
                ds.TransactionDepth.Should().Be(1);

                ds.CommitTransaction();
                ds.GetRowCount("TableA", null).Should().Be(1);

                ds.CurrentTransaction.Should().BeNull();
                ds.TransactionDepth.Should().Be(0);
            }
        }
    }
}