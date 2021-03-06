﻿using CruiseDAL.Schema;
using FluentAssertions;
using FMSC.ORM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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

        [SkippableTheory]
        [ClassData(typeof(TableNamesTestDataProvider))]
        public void RunDelete_WithConvertedFile(string tableName)
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

            var testFile = "MultiTest.2014.10.31.cruise";
            var (orgFile, crz3) = SetUpTestFile(testFile);

            var initializer = new DatabaseInitializer();
            using (var database = new CruiseDatastore_V3(crz3))
            {
                //database.OpenConnection();
                //database.Execute("PRAGMA foreign_keys=0;");
                database.Invoking(x => x.Execute($"DELETE FROM {tableName};"))
                        .Should().NotThrow();
                //var fKeyErrors = database.QueryGeneric("PRAGMA foreign_key_check;");
                //Output.WriteLine(string.Join("|\r\n",fKeyErrors.Select(x=> x.ToString()).ToArray()));
                //fKeyErrors.Should().BeEmpty();
            }
        }

        private (string, string) SetUpTestFile(string fileName, [CallerMemberName] string testName = null)
        {
            var filePath = Path.Combine(TestFilesDirectory, fileName);

            var baseFileName = Path.GetFileName(fileName);
            var orgFile = Path.Combine(TestTempPath,  testName + fileName);
            var crz3File = (string)null;

            // create copy of base file
            if (File.Exists(orgFile) == false)
            {
                File.Copy(filePath, orgFile);
            }
            crz3File = Migrator.MigrateFromV2ToV3(orgFile, true);

            return (orgFile, crz3File);
        }
    }
}
