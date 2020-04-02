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
using System.Collections.Generic;
using FMSC.ORM.EntityModel.Support;

namespace CruiseDAL.Tests
{
    public class Updater_Test : TestBase
    {
        public Updater_Test(ITestOutputHelper output) : base(output)
        {
        }

        protected void ValidateUpdate(CruiseDatastore database)
        {
            VerifyTablesCanDelete(database);
            ValidateCRUD(database);

            using (var fresh = new DAL())
            {
                var tDiff = DiffTables(fresh, database);
                tDiff.Should().BeEmpty();

                var diff = DiffTableInfo(fresh, database);
                diff.Should().BeEmpty();
            }

            database.CheckTableExists("SamplerState").Should().BeTrue();
        }

        void ValidateCRUD(CruiseDatastore ds)
        {
            var dataobjects = typeof(TreeDO).Assembly.GetTypes()
                .Where(x => x.Namespace == "CruiseDAL.DataObjects");

            foreach(var doType in dataobjects)
            {
                ValidateCRUD(ds, doType);
            }
        }

        void ValidateCRUD(CruiseDatastore ds, Type type)
        {
            var entDesc = new EntityDescription(type);
            var commandBuilder = entDesc.CommandBuilder;
            var selectBuilder = commandBuilder.MakeSelectCommand(null);

            var selectCommand = selectBuilder.ToString();
            var selectResult = ds.QueryGeneric(selectCommand);

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

                using (var database = new CruiseDatastore(filePath))
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
                    var semVerExpected = new Version("2.7.0");

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
        //[InlineData("Beaver_Pro_9132017.cruise")]
        [InlineData("51032_Toad Hill_TS.M.cruise")]
        public void FixVersion_2_5_0(string fileName)
        {
            var targetPath = InitializeTestFile(fileName);

            using (var db = new CruiseDatastore(targetPath))
            {
                //var trees = db.From<TreeDO>().Query().ToArray();

                db.From<TreeDOold>().Invoking(x => x.Query().ToArray()).Should().NotThrow();

                var errorTrees = db.QueryGeneric("SELECT * FROM Tree WHERE typeof(Tree_GUID) = 'text' AND Tree_GUID NOT LIKE '________-____-____-____-____________';")
                    .ToArray();
                errorTrees.Should().NotBeEmpty();

                var updater = new Updater_V2();
                updater.Update(db);
                //db.DatabaseVersion.Should().Be("2.5.1.1");

                var errorTreesAgain = db.QueryGeneric("SELECT * FROM Tree WHERE typeof(Tree_GUID) = 'text' AND Tree_GUID NOT LIKE '________-____-____-____-____________';")
                    .ToArray();
                errorTreesAgain.Should().BeEmpty();

                db.From<TreeDOold>().Invoking(x => x.Query()).Should().NotThrow();
            }
        }

        [Fact]
        public void update_from_2_5_to_2_7()
        {
            var fileName = "v2_5_0.cruise"; ;
            var path = InitializeTestFile(fileName);
            Output.WriteLine(path);

            using (var db = new CruiseDatastore(path))
            {
                var updater = new Updater_V2();
                updater.Update(db);

                db.CurrentTransaction.Should().BeNull();

                // insert multiple trees with the same guid to make sure that the tree guid uniqe constraint is removed
                //for (var i = 0; i < 2; i++)
                //{
                //    db.Insert(new V2.Models.Tree()
                //    {
                //        Tree_GUID = "something",
                //        CuttingUnit_CN = 1,
                //        Stratum_CN = 1,
                //        SampleGroup_CN = 1,
                //        TreeDefaultValue_CN = 1,
                //    });
                //}

                db.DatabaseVersion.Should().StartWith("2.7.");
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

        IEnumerable<string> DiffTables(CruiseDatastore left, CruiseDatastore right)
        {
            var leftTables = left.GetTableNames();
            var rightTables = right.GetTableNames();
            return leftTables.Except(rightTables);
        }

        IEnumerable<IEnumerable<string>> DiffTableInfo(CruiseDatastore left, CruiseDatastore right)
        {
            var tables = left.GetTableNames();

            foreach (var t in tables)
            {
                var diff = DiffTableInfo(left, right, t);

                if (diff.Any())
                {
                    yield return diff;
                }
            }
        }

        IEnumerable<string> DiffTableInfo(CruiseDatastore left, CruiseDatastore right, string table)
        {
            var leftValues = left.QueryGeneric(
$@"SELECT '{table}.' || name  FROM pragma_table_info('{table}') where Name != 'CreatedBy' AND Name != 'CreatedDate'; ").ToArray();

            var rightValues = right.QueryGeneric(
$@"SELECT '{table}.' || name  FROM pragma_table_info('{table}'); ").ToArray();

            return leftValues.Select(x => x.Values.First().ToString())
                .Except(rightValues.Select(x => x.Values.First().ToString())).ToArray();
        }

        [Table("Tree")]
        class TreeDOold
        {
            public Guid Tree_GUID { get; set; }

            public string TreeNumber { get; set; }
        }
    }
}