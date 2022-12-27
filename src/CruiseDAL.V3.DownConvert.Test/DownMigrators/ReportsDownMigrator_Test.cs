using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.DownConvert.Test.DownMigrators
{
    public class ReportsDownMigrator_Test : DownMigratorTestBase
    {
        public ReportsDownMigrator_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("R2_Test.cruise")]
        public void Reports_Test_WithFile(string fileName)
        {
            var (orgFile, crz3File, origAgain) = SetUpTestFile(fileName);

            using (var dbv2 = new CruiseDatastore(orgFile))
            using (var dbv2Again = new CruiseDatastore(origAgain))
            {
                var reportsV2Again = dbv2Again.From<V2.Models.Reports>()
                    .Query().ToArray();
                reportsV2Again.Should().NotBeEmpty();

                var reportsV2 = dbv2.From<V2.Models.Reports>()
                    .Query().ToArray();
                reportsV2.Should().NotBeEmpty();

                reportsV2Again.Should().HaveSameCount(reportsV2);
                reportsV2Again.Should().BeEquivalentTo(reportsV2);
            }
        }

        [Fact]
        public void Reports_Test()
        {

            var init = new DatabaseInitializer();

            var v3Path = GetTempFilePathWithExt(".crz3");
            var v3Db = init.CreateDatabaseFile(v3Path);

            var report = new Reports()
            {
                CruiseID = init.CruiseID,
                ReportID = "A01",
                Selected = true,
                Title = "Strata, Unit, Payment Unit and Sample Group Report (A1)",
            };
            v3Db.Insert(report);

            var v2Path = GetTempFilePathWithExt(".cruise");
            var v2Db = new DAL(v2Path, true);

            var downConvert = new DownMigrator();
            downConvert.MigrateFromV3ToV2(init.CruiseID, v3Db, v2Db);

            var reportAgain = v2Db.From<CruiseDAL.V2.Models.Reports>()
                .Query().Single();

            reportAgain.Should().BeEquivalentTo(report, 
                config: x => x.Excluding(y => y.CruiseID)
                .Excluding(y => y.CreatedBy)
                .Excluding(y => y.Created_TS)
                .Excluding(y => y.ModifiedBy)
                .Excluding(y => y.Modified_TS));
        }
    }
}