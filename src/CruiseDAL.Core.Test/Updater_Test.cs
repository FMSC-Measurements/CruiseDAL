using CruiseDAL.DataObjects;
using FluentAssertions;
using FMSC.ORM;
using FMSC.ORM.SQLite;
using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using System.Linq;
using FMSC.ORM.EntityModel.Attributes;

namespace CruiseDAL.Tests
{
    public class Updater_Test : TestBase
    {
        public Updater_Test(ITestOutputHelper output) : base(output)
        {
        }

        private void ValidateUpdate(CruiseDatastore database)
        {
            VerifyTablesCanDelete(database);

            database.CheckTableExists("SamplerState").Should().BeTrue();
        }

        [Fact]
        public void Update_FROM_05_30_2013()
        {
            var filePath = Path.Combine(TestTempPath, "TestUpdate.cruise");

            try
            {
                using (var setup = new SQLiteDatastore(filePath))
                {
                    setup.Execute(CruiseDAL.Tests.SQL.CRUISECREATE_05_30_2013);
                }

                using(var database = new CruiseDatastore(filePath))
                {
                    var dataStore = new CruiseDatastore(filePath);

                    var updater = new Updater_V2();
                    updater.Invoking(x => x.Update(dataStore)).Should().Throw<IncompatibleSchemaException>();
                }
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Fact]
        public void Update_FROM_2015_01_05()
        {
            var filePath = Path.Combine(TestTempPath, "TestUpdate.cruise");

            try
            {
                using (var setup = new SQLiteDatastore(filePath))
                {
                    setup.Execute(CruiseDAL.Tests.SQL.CRUISECREATE_2015_01_05);
                }

                using (var datastore = new CruiseDatastore(filePath))
                {
                    var dataStore = new CruiseDatastore(filePath);

                    var updater = new Updater_V2();
                    updater.Invoking(x => x.Update(dataStore))
                        .Should().NotThrow();

                    var semVerActual = new Version(datastore.DatabaseVersion);
                    var semVerExpected = new Version("2.6.0");

                    semVerActual.Major.Should().Be(semVerExpected.Major);
                    semVerActual.Minor.Should().Be(semVerExpected.Minor);

                    ValidateUpdate(datastore);
                }
            }
            finally
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        [Theory]
        [InlineData("Beaver_Pro_9132017.cruise")]
        [InlineData("51032_Toad Hill_TS.M.cruise")]
        public void FixVersion_2_5_0(string fileName)
        {
            var targetPath = InitializeTestFile(fileName);

            using (var db = new CruiseDatastore(targetPath))
            {
                //var trees = db.From<TreeDO>().Query().ToArray();

                db.From<TreeDOold>().Invoking(x => x.Query().ToArray()).Should().Throw<Exception>("");

                var errorTrees = db.QueryGeneric("SELECT * FROM Tree WHERE typeof(Tree_GUID) = 'text' AND Tree_GUID NOT LIKE '________-____-____-____-____________';")
                    .ToArray();
                errorTrees.Should().NotBeEmpty();

                var updater = new Updater_V2();
                updater.Update(db);
                db.DatabaseVersion.Should().Be("2.5.1.1");

                var errorTreesAgain = db.QueryGeneric("SELECT * FROM Tree WHERE typeof(Tree_GUID) = 'text' AND Tree_GUID NOT LIKE '________-____-____-____-____________';")
                    .ToArray();
                errorTreesAgain.Should().BeEmpty();

                db.From<TreeDOold>().Invoking(x => x.Query()).Should().NotThrow();
            }
        }

        protected void VerifyTablesCanDelete(CruiseDatastore datastore)
        {
            var tableNames = datastore.ExecuteScalar<string>("SELECT group_concat(Name) FROM sqlite_master WHERE Type = 'table';").Split(',');

            foreach (var table in tableNames)
            {
                try
                {
                    datastore.Execute($"DELETE FROM {table};");
                }
                catch (Exception e)
                {
                    Output.WriteLine(e.Message);
                    Output.WriteLine(e.InnerException.Message);
                }

                //datastore.Invoking(x => x.Execute($"DELETE FROM {table};")).Should().NotThrow();
            }
        }

        [Table("Tree")]
        class TreeDOold
        {
            public Guid Tree_GUID { get; set; }

            public string TreeNumber { get; set; }
        }
    }
}