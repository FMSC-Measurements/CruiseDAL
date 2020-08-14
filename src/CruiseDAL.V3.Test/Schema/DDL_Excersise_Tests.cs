using CruiseDAL.V3.Tests;
using FluentAssertions;
using FMSC.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.Tests.Schema
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
    }
}
