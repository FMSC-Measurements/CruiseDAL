using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.DownConvert.Test.DownMigrators
{
    public class StratumDownMigrator_Test : DownMigratorTestBase
    {
        public StratumDownMigrator_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        [InlineData("0432 C53East TS.cruise")]
        public void Stratum_Test(string fileName)
        {
            var (orgFile, crz3File, origAgain) = SetUpTestFile(fileName);

            using (var dbv2 = new CruiseDatastore(orgFile))
            using (var dbv2Again = new CruiseDatastore(origAgain))
            {
                var stratav2Again = dbv2Again.From<V2.Models.Stratum>()
                    .Query();

                var stratav2 = dbv2.From<V2.Models.Stratum>()
                    .Query();

                stratav2Again.Should().BeEquivalentTo(stratav2,
                    config => config
                    .Excluding(x => x.VolumeFactor)
                    .Excluding(x => x.Month)
                    .Excluding(x => x.Year));
            }
        }
    }
}