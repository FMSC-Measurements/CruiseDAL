using CruiseDAL.TestCommon;
using FluentAssertions;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync
{
    public class CruiseFileUtilities_Test : TestBase
    {
        public CruiseFileUtilities_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ClearTombstoneRecords()
        {
            var init = new DatabaseInitializer();

            using var db = init.CreateDatabaseWithAllTables();

            var cruiseID = init.CruiseID;

            db.Execute("DELETE FROM Cruise WHERE CruiseID = @p1;", cruiseID);

            var tombstoneTables = db.GetTableNames().Where(x => x.EndsWith("_Tombstone"));
            foreach (var table in tombstoneTables)
            {
                db.GetRowCount(table, (string)null).Should().BeGreaterThan(0, table);
            }

            CruiseFileUtilities.ClearTombstoneRecords(db, cruiseID);

            foreach (var table in tombstoneTables)
            {
                db.GetRowCount(table, (string)null).Should().Be(0, table);
            }
        }
    }
}