using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.DownConvert.Test.DownMigrators
{
    public class SampleGroupDownMigrator_Test : DownMigratorTestBase
    {
        public SampleGroupDownMigrator_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        [InlineData("0432 C53East TS.cruise")]
        public void SampleGroup_Test(string fileName)
        {
            var (orgFile, crz3File, origAgain) = SetUpTestFile(fileName);

            using (var dbv2 = new CruiseDatastore(orgFile))
            using (var dbv2Again = new CruiseDatastore(origAgain))
            {
                var sgv2Again = dbv2Again.From<V2.Models.SampleGroup>()
                    .Query();

                var sgv2 = dbv2.From<V2.Models.SampleGroup>()
                    .Query();

                sgv2Again.Should().BeEquivalentTo(sgv2, x =>
                    x.Excluding(y => y.SampleSelectorState)
                    .Excluding(y => y.SampleSelectorType)
                    .Excluding(y => y.TallyMethod));
            }
        }
    }
}