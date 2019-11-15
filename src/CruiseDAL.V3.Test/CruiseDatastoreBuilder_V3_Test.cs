using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;

namespace CruiseDAL.V3.Tests
{
    public class CruiseDALDatastoreBuilder_Test : TestBase
    {
        public CruiseDALDatastoreBuilder_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void CreateTables_Test()
        {
            using (var database = new FMSC.ORM.SQLite.SQLiteDatastore())
            {
                var dbBuilder = new CruiseDatastoreBuilder_V3();

                dbBuilder.Invoking( x => x.CreateDatastore(database)).Should().NotThrow();
            }
        }
    }
}
