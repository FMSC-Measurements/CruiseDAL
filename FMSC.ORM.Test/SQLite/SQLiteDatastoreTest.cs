using FMSC.ORM.MyXUnit;
using FMSC.ORM.TestSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FMSC.ORM.SQLite
{
    public class SQLiteDatastoreTest : TestClassBase
    {
        public SQLiteDatastoreTest(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public void CreateSQLiteDatastoreTest()
        {
            var path = "TestDB.db";
            path = System.IO.Path.GetFullPath(path);
            _output.WriteLine(path);
            var dbBuilder = new TestDBBuilder();
            try
            {
                using (var ds = new SQLiteDatastore(path))
                {
                    Assert.False(ds.Exists);

                    //TODO hide builder inside dataStore
                    dbBuilder.Datastore = ds;
                    dbBuilder.CreateDatastore();

                    Assert.True(ds.Exists);
                    AssertEx.NotNullOrWhitespace(ds.Extension);

                    VerifySQLiteDatastore(ds);
                }
            }
            finally
            {
                //System.IO.File.Delete(path);
            }
        }

        protected void VerifySQLiteDatastore(SQLiteDatastore ds)
        {
            Assert.NotNull(ds);
            AssertEx.NotNullOrWhitespace(ds.Path);

            var explaneSelectResult = ds.Execute("EXPLAIN SELECT 1;");
            Assert.NotNull(explaneSelectResult);

        }

        protected SQLiteDatastore CreateTestInstance()
        {
            var path = "TestDB.db";
            var ds = new SQLiteDatastore(path);

            return ds;
        }

        [Fact]
        public void AddFieldTest()
        {
            using (var ds = CreateTestInstance())
            {
                ds.AddField("something", "someField TEXT");

                Assert.True(ds.CheckFieldExists("something", "someField"));

                throw new NotImplementedException();
            }
        }

        [Fact]
        public void CheckTableExistsTest()
        {
            using (var ds = CreateTestInstance())
            {
                Assert.True(ds.CheckTableExists("MultiPropTable"));

                Assert.False(ds.CheckTableExists("notATable"));
            }
        }

        [Fact]
        public void CheckFieldExistsTest()
        {
            using (var ds = CreateTestInstance())
            {
                Assert.True(ds.CheckFieldExists("MultiPropTable", "StringField"));

                Assert.False(ds.CheckFieldExists("MultiPropTable", "notAField"));
            }
        }

        [Fact]
        public void GetTableInfoTest()
        {
            using (var ds = CreateTestInstance())
            {
                var ti = ds.GetTableInfo("MultiPropTable");

                Assert.NotNull(ti);
                Assert.NotEmpty(ti);

                foreach(string fieldName in TestSQLConstants.MULTI_PROP_TABLE_FIELDS)
                {
                    Assert.Contains(ti, x => x.Name.ToLower() == fieldName.ToLower());
                }

            }
        }

        [Fact]
        public void HasForeignKeyErrors()
        {
            using (var ds = CreateTestInstance())
            {
                Assert.False(ds.HasForeignKeyErrors("MultiPropTable"));

                throw new NotImplementedException();
            }
        }

        [Fact]
        public void GetRowCountTest()
        {
            using (var ds = CreateTestInstance())
            {
                var rowCnt = ds.GetRowCount("MultiPropTable", null);
                Assert.True(rowCnt == 0);

                throw new NotImplementedException();
            }
        }

        [Fact]
        public void GetTableSQLTest()
        {
            using (var ds = CreateTestInstance())
            {
                var tableSQL = ds.GetTableSQL("MultiPropTable");
                AssertEx.NotNullOrWhitespace(tableSQL);
                _output.WriteLine(tableSQL);

                throw new NotImplementedException();
            }
        }

        [Fact]
        public void GetTableUniquesTest()
        {
            using (var ds = CreateTestInstance())
            {
                var tableUniques = ds.GetTableUniques("MultiPropTable");
                Assert.Empty(tableUniques);


                throw new NotImplementedException();
            }
        }

    }
}
