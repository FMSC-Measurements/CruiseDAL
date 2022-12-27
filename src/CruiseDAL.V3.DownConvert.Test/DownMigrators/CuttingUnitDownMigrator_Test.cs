using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.DownConvert.Test.DownMigrators
{
    public class CuttingUnitDownMigrator_Test : DownMigratorTestBase
    {
        public CuttingUnitDownMigrator_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        [InlineData("0432 C53East TS.cruise")]
        public void CuttingUnits_Test(string fileName)
        {
            var (orgFile, crz3File, origAgain) = SetUpTestFile(fileName);

            using (var dbv2 = new CruiseDatastore(orgFile))
            using (var dbv2Again = new CruiseDatastore(origAgain))
            {
                var cuttingUnitsV2Again = dbv2Again.From<V2.Models.CuttingUnit>()
                    .Query();

                var cuttingUnitsv2 = dbv2.From<V2.Models.CuttingUnit>()
                    .Query();

                cuttingUnitsV2Again.Should().BeEquivalentTo(cuttingUnitsv2,
                    config => config
                    .Using<string>(x => x.Subject.Should().Be(x.Expectation?.Trim())).WhenTypeIs<string>()// ignore whitespace when type is string
                    .Excluding(y => y.TallyHistory));
            }
        }
    }
}