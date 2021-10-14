using CruiseDAL.TestCommon;
using FluentAssertions;
using FMSC.ORM;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test.Schema
{
    public class DDL_Excersise_Tests : TestBase
    {
        public DDL_Excersise_Tests(ITestOutputHelper output) : base(output)
        {
        }

        public CruiseDatastore_V3 CreateDatastore()
        {
            var datastore = new CruiseDatastore_V3();
            return datastore;
        }

        [Fact]
        public void Views_Read_Test()
        {
            using (var datastore = CreateDatastore())
            {
                var views = datastore.ExecuteScalar<string>("SELECT group_concat(name) FROM Sqlite_Master WHERE type = 'view';")
                    .Split(',');

                foreach (var view in views)
                {
                    datastore.Invoking(x => x.Execute($"SELECT * FROM {view};"))
                        .Should().NotThrow(view);
                }
            }
        }

        [Fact]
        public void Views_Delete_Test()
        {
            using (var datastore = CreateDatastore())
            {
                var views = datastore.ExecuteScalar<string>("SELECT group_concat(name) FROM Sqlite_Master WHERE type = 'view';")
                    .Split(',');

                foreach (var view in views)
                {
                    //datastore.Invoking(x => x.Execute($"DELETE FROM {view};"))
                    //        .Should().NotThrow(view);

                    try
                    {
                        datastore.Execute($"DELETE FROM {view};");
                    }
                    catch (SQLException e)
                    {
                        // ignore exceptions that are thrown "because it is a view"
                        // these exceptions are thrown because we are deleteing from a view
                        // rather than because the delete trigger had an error
                        if (e.InnerException != null
                            && e.InnerException.Message.Contains("because it is a view"))
                        { }
                        else
                        { throw; }
                    }
                }
            }
        }

        [Fact]
        // integrity check should find any UNIQUE, CHECK, and NOT NULL constraint errors
        public void IntegrityCheck()
        {
            using (var database = CreateDatastore())
            {
                var ic_results = database.QueryScalar<string>("PRAGMA integrity_check;");
                ic_results.Should().HaveCount(1);
                ic_results.Single().Should().Be("ok");
            }
        }

        [Fact]
        public void RunForeignKeysCheck()
        {
            using (var database = CreateDatastore())
            {
                // calling foreign key check will expose any DDL errors related to foreign keys
                database.Invoking(x => x.Execute("PRAGMA foreign_key_check;")).Should().NotThrow();
            }
        }

        [Fact]
        public void RunDelete()
        {
            using (var database = CreateDatastore())
            {
                var tableDefs = CruiseDatastoreBuilder_V3.TABLE_DEFINITIONS;
                foreach (var tDef in tableDefs)
                {
                    database.Invoking(x => x.Execute($"DELETE FROM {tDef.TableName};"))
                        .Should().NotThrow();
                }
            }
        }

        [SkippableTheory]
        [ClassData(typeof(TableNamesTestDataProvider))]
        public void RunDelete_WithData(string tableName)
        {
            var skipTables = new[]
                {
                    "LK_CruiseMethod",
                    "LK_Product",
                    "LK_Purpose",
                    "LK_Region",
                    "LK_UOM",
                    "Species",
                    "TreeField",
                };
            // some tables don't have cascading deletes so we need to skip them
            Skip.If(skipTables.Contains(tableName));

            var initializer = new DatabaseInitializer();
            using (var database = initializer.CreateDatabase())
            {
                database.Invoking(x => x.Execute($"DELETE FROM {tableName};"))
                        .Should().NotThrow();
            }
        }
    }
}