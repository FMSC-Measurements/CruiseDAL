using FMSC.ORM.Core.SQL;
using FMSC.ORM.TestSupport;
using FMSC.ORM.TestSupport.TestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.SQLite
{
    public class SQLiteDatastoreTest : TestClassBase, IClassFixture<TestDBFixture>
    {
        TestDBFixture _fixture;

        public SQLiteDatastoreTest(ITestOutputHelper output, TestDBFixture fixture) : base(output)
        {
            _fixture = fixture;
        }

        [Fact]
        public void CreateSQLiteDatastoreTest()
        {
            var path = _fixture.CreatedDBPath;
            path = System.IO.Path.GetFullPath(path);
            _output.WriteLine("Path: " + path);
            var dbBuilder = new TestDBBuilder();

            using (var ds = new SQLiteDatastore(path))
            {
                Assert.False(ds.Exists, "Pre-condition failed file already created");

                //TODO hide builder inside dataStore
                dbBuilder.Datastore = ds;
                dbBuilder.CreateDatastore();

                AssertEx.NotNullOrWhitespace(ds.Extension, "Assert file has extension");
                AssertEx.NotNullOrWhitespace(ds.Path);

                VerifySQLiteDatastore(ds);
            }
        }

        [Fact]
        public void CreateInmemorySQLiteDatastoreTest()
        {
            var dbBuilder = new TestDBBuilder();

            using (var ds = new SQLiteDatastore())
            {
                Assert.True(ds.Exists);

                //TODO hide builder inside dataStore
                dbBuilder.Datastore = ds;
                dbBuilder.CreateDatastore();

                VerifySQLiteDatastore(ds);
            }
        }

        public void VerifySQLiteDatastore(SQLiteDatastore ds)
        {
            Assert.True(ds.Exists, "Assert file exists");
            

            Assert.NotNull(ds);
            //AssertEx.NotNullOrWhitespace(ds.Extension, "Assert file has extension");
            //AssertEx.NotNullOrWhitespace(ds.Path);

            var explaneSelectResult = ds.Execute("EXPLAIN SELECT 1;");
            Assert.NotNull(explaneSelectResult);
            Assert.True(ds.GetRowCount("sqlite_master", null, null) > 0);

        }

        [Fact]
        public void AddFieldTest()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.Execute("CREATE TABLE TableA (ID INTEGER PRIMARY KEY);");
                Assert.True(ds.CheckFieldExists("TableA", "ID"));
                Assert.False(ds.CheckFieldExists("TABLEA", "Data"));

                ds.AddField("TableA", new ColumnInfo() { Name = "Data", DBType = "TEXT" });

                Assert.True(ds.CheckFieldExists("TABLEA", "Data"));

                Assert.Throws<SQLException>(() => ds.AddField("TableA", new ColumnInfo() { Name = "Data", DBType = "TEXT" }));
            }
        }

        [Fact]
        public void CheckTableExistsTest()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.Execute(TestDBBuilder.CREATE_MULTIPROPTABLE);

                Assert.True(ds.CheckTableExists("MultiPropTable"));

                Assert.False(ds.CheckTableExists("notATable"));
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
                        new ColumnInfo("ID", Types.INTEGER, true, false, null),
                        new ColumnInfo("Field1", Types.TEXT, false, true, null)
                    };

                ds.CreateTable("TableA", cols, false);

                Assert.True(ds.CheckTableExists("TableA"));
            }
        }

        [Fact]
        public void GetTableInfoTest()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.Execute(TestDBBuilder.CREATE_MULTIPROPTABLE);

                var ti = ds.GetTableInfo("MultiPropTable");

                Assert.NotNull(ti);
                Assert.NotEmpty(ti);

                try
                {
                    foreach (string fieldName in TestSQLConstants.MULTI_PROP_TABLE_FIELDS)
                    {
                        bool contains = ti.Any(x => x.Name.ToLower() == fieldName.ToLower());
                        _output.WriteLine("{0} exists.. {1}", fieldName, contains);
                        Assert.True(contains);
                    }
                }
                finally
                {
                    _output.WriteLine(string.Join(",", (from colInfo in ti
                                                        select colInfo.Name).OrderBy(x => x)));
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
                ds.CreateTable("something", new ColumnInfo[]{ new ColumnInfo("data") });

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
                AssertEx.NotNullOrWhitespace(tableSQL);
                _output.WriteLine(tableSQL);

            }
        }

        [Fact]
        public void SetTableAutoIncrementStartTest()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.Execute(TestDBBuilder.CREATE_AUTOINCREMENT_TABLE);

                var seq = ds.From<SQLiteSequenceModel>()
                    .Where("name = 'AutoIncrementTable'").Query().FirstOrDefault();

                Assert.Null(seq);

                ds.Insert(new AutoIncrementTable() { Data = "something" }, OnConflictOption.Fail);

                seq = ds.From<SQLiteSequenceModel>()
                    .Where("name = 'AutoIncrementTable'").Query().FirstOrDefault();

                Assert.NotNull(seq);
                Assert.Equal(1, seq.Seq);

                var incVal = 200;
                ds.SetTableAutoIncrementStart("AutoIncrementTable", incVal);

                seq = ds.From<SQLiteSequenceModel>()
                    .Where("name = 'AutoIncrementTable'").Query().FirstOrDefault();

                Assert.NotNull(seq);
                Assert.Equal(incVal, seq.Seq);

                ds.Insert(new AutoIncrementTable() { Data = "something" }, OnConflictOption.Fail);

                seq = ds.From<SQLiteSequenceModel>()
                    .Where("name = 'AutoIncrementTable'").Query().FirstOrDefault();

                Assert.NotNull(seq);
                Assert.Equal(incVal + 1, seq.Seq);
            }
        }

        [Fact]
        public void TestReadOnly()
        {
            var path = System.IO.Path.GetFullPath("readOnly.db");

            _output.WriteLine(path);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.SetAttributes(path, System.IO.FileAttributes.Normal);
                System.IO.File.Delete(path);
            }

            using (var ds = new SQLiteDatastore("readOnly.db"))
            {
                ds.Execute(TestDBBuilder.CREATE_AUTOINCREMENT_TABLE);
                ds.CreateTable("Tbl", new ColumnInfo[] { new ColumnInfo() { Name = "Data", DBType = "TEXT" } }, false);

                System.IO.File.SetAttributes(path, System.IO.FileAttributes.ReadOnly);

                //ds.BeginTransaction();
                Assert.Throws<ReadOnlyException>(() => ds.BeginTransaction());
                Assert.Throws<ReadOnlyException>(() => ds.Execute("INSERT INTO Tbl (Data) VALUES ('something');"));
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
        public void ExecuteScalarGenericTest()
        {
            using (var ds = new SQLiteDatastore())
            {
                var query = "SELECT 1;";
                var nQuery = "SELECT NULL;";
                string sResult = ds.ExecuteScalar<string>(query);
                AssertEx.NotNullOrEmpty(sResult);

                sResult = ds.ExecuteScalar<string>(nQuery);
                Assert.Null(sResult);

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

        void VerifyExecuteScalarWithType<T>(T expected, SQLiteDatastore ds)
            where T : struct
        {
            _output.WriteLine("testing value {0} : {1}", expected, typeof(T).Name);

            T result = ds.ExecuteScalar<T>("SELECT ?;", expected);
            _output.WriteLine("     as {0} gives {1}", typeof(T).Name, result.ToString());
            Assert.Equal(expected, result);

            result = ds.ExecuteScalar<T>("SELECT NULL;");
            _output.WriteLine("     NULL as {0} gives {1}", typeof(T).Name, result.ToString());
            Assert.Equal(default(T), result);

            T? nResult = ds.ExecuteScalar<T?>("SELECT ?;", expected);
            _output.WriteLine("     as {0} gives {1}", typeof(T?).Name, nResult.ToString());
            Assert.True(nResult.HasValue);
            Assert.Equal(expected, nResult.Value);

            nResult = ds.ExecuteScalar<T?>("SELECT NULL;");
            _output.WriteLine("     NULL as {0} gives {1}", typeof(T?).Name, nResult.ToString());
            Assert.False(nResult.HasValue);
        }

        [Fact]
        public void QuerySingleRecordTest()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.Execute(TestDBBuilder.CREATE_MULTIPROPTABLE);

                var setup = "INSERT INTO MultiPropTable (IntField) VALUES (1);";
                ds.Execute(setup);

                Assert.Equal(1, ds.GetRowCount("MultiPropTable", "WHERE IntField = 1"));

                var result = ds.QuerySingleRecord<POCOMultiTypeObject>(new WhereClause("IntField = 1"));
                Assert.NotNull(result);
                Assert.Equal(1, result.IntField);

                var result1 = ds.QuerySingleRecord<POCOMultiTypeObject>(new WhereClause("IntField = 1"));
                Assert.NotSame(result, result1);
                Assert.Equal(1, result1.IntField);
            }
        }

        [Fact]
        public void QueryTest()
        {
            int recordsToCreate = 1000;
            using (var ds = new SQLiteDatastore())
            {
                ds.Execute(TestDBBuilder.CREATE_MULTIPROPTABLE);
                ds.BeginTransaction();
                for (int i = 1; i <= recordsToCreate; i++)
                {
                    ds.Execute(string.Format(" INSERT INTO MultiPropTable (IntField) VALUES ({0});\r\n", i));
                }
                ds.CommitTransaction();

                //ds.Execute(setup.ToString());

                Assert.Equal(recordsToCreate, ds.GetRowCount("MultiPropTable", null));

                StartTimer();
                var result = ds.Query<POCOMultiTypeObject>((WhereClause)null);
                EndTimer();
                Assert.Equal(recordsToCreate, result.Count);
            }
        }

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
                    ds.Execute(string.Format(" INSERT INTO MultiPropTable (IntField) VALUES ({0});\r\n", i));
                }
                ds.CommitTransaction();

                Assert.Equal(recordsToCreate, ds.GetRowCount("MultiPropTable", null));

                StartTimer();
                var result = ds.From<POCOMultiTypeObject>().Limit(5000, 0).Query().ToList();
                EndTimer();

                foreach (DOMultiPropType item in
                    ds.From<DOMultiPropType>().Read())
                {
                    item.FloatField = 1.0F;
                    item.Save();
                }

                Assert.NotEmpty(result);
                Assert.Equal(recordsToCreate, result.Count);
            }
        }

        [Fact]
        public void BadTransactionMGMTDoesntFuckShitUp()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.Execute("CREATE TABLE TableA (Data TEXT);");

                ds.CommitTransaction();//extra commit should not throw exception, but will fail Debug.Assert

                ds.BeginTransaction();

                ds.Execute("INSERT INTO TableA VALUES ('something');");

                ds.RollbackTransaction();
                Assert.Equal(0, ds.GetRowCount("TableA", null));

                ds.BeginTransaction();

                ds.Execute("INSERT INTO TableA VALUES ('something');");

                ds.CommitTransaction();
                Assert.Equal(1, ds.GetRowCount("TableA", null));
            }

            using (var ds = new SQLiteDatastore())
            {
                ds.Execute("CREATE TABLE TableA (Data TEXT);");

                ds.RollbackTransaction();//extra rollback should not throw exception, but will fail Debug.Assert

                ds.BeginTransaction();

                ds.Execute("INSERT INTO TableA VALUES ('something');");

                ds.RollbackTransaction();
                Assert.Equal(0, ds.GetRowCount("TableA", null));

                ds.BeginTransaction();

                ds.Execute("INSERT INTO TableA VALUES ('something');");

                ds.CommitTransaction();
                Assert.Equal(1, ds.GetRowCount("TableA", null));
            }
        }

        [Fact]
        public void NestedTransactionTest()
        {
            using (var ds = new SQLiteDatastore())
            {
                ds.Execute("CREATE TABLE TableA (Data TEXT);");

                ds.BeginTransaction();
                ds.BeginTransaction();

                ds.Execute("INSERT INTO TableA VALUES ('something');");

                ds.RollbackTransaction();
                Assert.Equal(1, ds.GetRowCount("TableA", null));
                ds.CommitTransaction();
                Assert.Equal(0, ds.GetRowCount("TableA", null));

                ds.BeginTransaction();
                ds.BeginTransaction();

                ds.Execute("INSERT INTO TableA VALUES ('something');");

                ds.CommitTransaction();
                Assert.Equal(1, ds.GetRowCount("TableA", null));
                ds.RollbackTransaction();
                Assert.Equal(0, ds.GetRowCount("TableA", null));

                ds.BeginTransaction();
                ds.BeginTransaction();

                ds.Execute("INSERT INTO TableA VALUES ('something');");

                ds.CommitTransaction();
                Assert.Equal(1, ds.GetRowCount("TableA", null));
                ds.CommitTransaction();
                Assert.Equal(1, ds.GetRowCount("TableA", null));
            }
        }
    }
}