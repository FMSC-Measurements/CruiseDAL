using FMSC.ORM.Core.SQL;
using FMSC.ORM.MyXUnit;
using FMSC.ORM.TestSupport;
using FMSC.ORM.TestSupport.TestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                Assert.False(ds.Exists, "Assert file doesn't already exist");

                //TODO hide builder inside dataStore
                dbBuilder.Datastore = ds;
                dbBuilder.CreateDatastore();

                _fixture.VerifySQLiteDatastore(ds);
            }
            
        }


        [Fact]
        public void AddFieldTest()
        {
            using (var ds = _fixture.WorkingDatastore)
            {
                ds.AddField("MultiPropTable", "newField TEXT");

                Assert.True(ds.CheckFieldExists("MultiPropTable", "newField"));

                Assert.Throws<SQLException>(() => ds.AddField("MultiPropTable", "newField TEXT"));
            }
        }

        [Fact]
        public void CheckTableExistsTest()
        {
            using (var ds = _fixture.StaticDatastore)
            {
                Assert.True(ds.CheckTableExists("MultiPropTable"));

                Assert.False(ds.CheckTableExists("notATable"));
            }
        }

        [Fact]
        public void CheckFieldExistsTest()
        {
            using (var ds = _fixture.StaticDatastore)
            {
                Assert.True(ds.CheckFieldExists("MultiPropTable", "StringField"));

                Assert.False(ds.CheckFieldExists("MultiPropTable", "notAField"));
            }
        }

        [Fact]
        public void GetTableInfoTest()
        {
            using (var ds = _fixture.StaticDatastore)
            {
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
                    _output.WriteLine( string.Join(",", (from colInfo in ti
                                      select colInfo.Name).OrderBy(x => x)));
                }

            }
        }



        [Fact]
        public void HasForeignKeyErrors()
        {
            using (var ds = _fixture.WorkingDatastore)
            {
                Assert.False(ds.HasForeignKeyErrors("MultiPropTable"));

                throw new NotImplementedException();
            }
        }

        [Fact]
        public void GetRowCountTest()
        {
            using (var ds = _fixture.WorkingDatastore)
            {
                var rowCnt = ds.GetRowCount("MultiPropTable", null);
                Assert.True(rowCnt == 0);

                ds.Execute("INSERT INTO MultiPropTable DEFAULT VALUES");

                rowCnt = ds.GetRowCount("MultiPropTable", null);
                Assert.True(rowCnt == 1);
            }
        }

        [Fact]
        public void GetTableSQLTest()
        {
            using (var ds = _fixture.StaticDatastore)
            {
                var tableSQL = ds.GetTableSQL("MultiPropTable");
                AssertEx.NotNullOrWhitespace(tableSQL);
                _output.WriteLine(tableSQL);

                Assert.Equal(tableSQL, TestDBBuilder.CREATE_MULTIPROPTABLE, 
                    ignoreCase: true, 
                    ignoreLineEndingDifferences: true, 
                    ignoreWhiteSpaceDifferences: true);
            }
        }

        [Fact]
        public void GetTableUniquesTest()
        {
            using (var ds = _fixture.StaticDatastore)
            {
                var tableUniques = ds.GetTableUniques("MultiPropTable");
                Assert.Empty(tableUniques);


                throw new NotImplementedException();
            }
        }

        [Fact]
        public void ExecuteScalarTest()
        {

            throw new NotImplementedException();
        }

        [Fact]
        public void ExecuteScalarGenericTest()
        {
            using (var ds = _fixture.StaticDatastore)
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

                VerifyExecuteScalarWithType<DateTime>(DateTime.Today,  ds);

            }
        }

        void VerifyExecuteScalarWithType<T>(T expected, SQLiteDatastore ds)
            where T :struct
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
            using (var ds = _fixture.WorkingDatastore)
            {
                var setup = "DELETE FROM MultiPropTable; "
                            + "INSERT INTO MultiPropTable (IntField) VALUES (1);";
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
            using (var ds = _fixture.WorkingDatastore)
            {
                var setup = "DELETE FROM MultiPropTable; "
                            + "INSERT INTO MultiPropTable (IntField) VALUES (1);"
                            + "INSERT INTO MultiPropTable (IntField) VALUES (2);";
                ds.Execute(setup);

                Assert.Equal(2, ds.GetRowCount("MultiPropTable", null));

                var result = ds.Query<POCOMultiTypeObject>((WhereClause)null);
                Assert.Equal(2, result.Count);
            }
        }

    }
}
