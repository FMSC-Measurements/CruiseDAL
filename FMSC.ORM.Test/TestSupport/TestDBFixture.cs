using FMSC.ORM.SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FMSC.ORM.TestSupport
{
    public class TestDBFixture : IDisposable
    {
        public string CreatedDBPath { get; set; }
        public string StaticDBPath { get; set; }
        public string WorkingDBPath { get; set; }

        /// <summary>
        /// datastore that doesn't change during tests
        /// </summary>
        public SQLiteDatastore StaticDatastore
        {
            get
            {
                var path = StaticDBPath;
                path = System.IO.Path.GetFullPath(path);
                return new SQLiteDatastore(path);
            }
        }

        /// <summary>
        /// datastore that may be changed by tests
        /// </summary>
        public SQLiteDatastore WorkingDatastore
        {
            get
            {
                var path = WorkingDBPath;
                path = System.IO.Path.GetFullPath(path);
                return new SQLiteDatastore(path);
            }
        }

        public TestDBFixture()
        {
            CreatedDBPath = "CreatedTestDB.db";
            WorkingDBPath = "WorkingDB.db";
            StaticDBPath = "StaticDB.db";
            File.Delete(CreatedDBPath);
            File.Delete(WorkingDBPath);
            File.Delete(StaticDBPath);

            CreateStaticDB();
            CreateWorkingDB();
        }

        void CreateStaticDB()
        {
            using (var ds = StaticDatastore)
            {
                Assert.False(ds.Exists, "Assert file doesn't already exist");


                //TODO hide builder inside dataStore
                var dbBuilder = new TestDBBuilder();
                dbBuilder.Datastore = ds;
                dbBuilder.CreateDatastore();

                VerifySQLiteDatastore(ds);
            }

        }

        void CreateWorkingDB()
        {
            using (var ds = WorkingDatastore)
            {
                Assert.False(ds.Exists, "Assert file doesn't already exist");


                //TODO hide builder inside dataStore
                var dbBuilder = new TestDBBuilder();
                dbBuilder.Datastore = ds;
                dbBuilder.CreateDatastore();

                VerifySQLiteDatastore(ds);
            }

        }

        public void VerifySQLiteDatastore(SQLiteDatastore ds)
        {
            Assert.True(ds.Exists, "Assert file exists");
            AssertEx.NotNullOrWhitespace(ds.Extension, "Assert file has extension");

            Assert.NotNull(ds);
            AssertEx.NotNullOrWhitespace(ds.Path);

            var explaneSelectResult = ds.Execute("EXPLAIN SELECT 1;");
            Assert.NotNull(explaneSelectResult);

        }

        public void Dispose()
        {

        }

    }
}
