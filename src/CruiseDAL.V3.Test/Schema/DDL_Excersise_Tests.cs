using CruiseDAL.V3.Tests;
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

        public DAL CreateDatastore()
        {
            var datastore = new DAL();
            return datastore;
        }

        [Fact]
        public void Views_Read_Test()
        {
            using (var datastore = CreateDatastore())
            {
                var views = datastore.ExecuteScalar<string>("SELECT group_concat(name) FROM Sqlite_Master WHERE type = 'view';")
                    .Split(',');

                foreach(var view in views)
                {
                    datastore.Execute($"SELECT * FROM {view};");
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
                    try
                    {

                        datastore.Execute($"DELETE FROM {view};");
                    }
                    catch(SQLException e)
                    {
                        if(e.InnerException != null 
                            && e.InnerException.Message.Contains("because it is a view"))
                        { }
                        else
                        { throw; }
                    }
                }
            }
        }
    }
}
