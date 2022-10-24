using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.DownConvert.Test
{
    public class DownMigrator_Issues_Test : TestBase
    {
        public DownMigrator_Issues_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Goshen_Blueberry_TS()
        {
            var fromPath = GetTestFile("52203_Goshen Blueberry_TS_202210201203_GalaxyTabActivePro-8PC5.crz3");
            var fromDb = new CruiseDatastore_V3(fromPath);
            var cruiseID = fromDb.From<Cruise>().Query().Single().CruiseID;

            var toPath = GetTempFilePathWithExt(".cruise");
            var toDb = new DAL(toPath, true);

            var downMigrator = new DownMigrator();
            downMigrator.MigrateFromV3ToV2(cruiseID, fromDb, toDb);

            var toSale = toDb.From<V2.Models.Sale>().Query().SingleOrDefault();
            toSale.Should().NotBeNull();
        }
    }
}
