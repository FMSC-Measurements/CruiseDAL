using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;

namespace CruiseDAL.Tests
{
    public class CruiseDALDatastoreBuilder_Test : TestBase
    {
        public CruiseDALDatastoreBuilder_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void CreateTables_Test()
        {
            using(var database = new FMSC.ORM.SQLite.SQLiteDatastore())
            using (var conn = database.CreateConnection())
            {
                conn.Open();

                var dbBuilder = new CruiseDALDatastoreBuilder();
                dbBuilder.CreateDatabase(conn);
            }
        }
    }
}
