using FluentAssertions;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.DownConvert.Test.DownMigrators
{
    public class TreeDownMigrator_Test : DownMigratorTestBase
    {
        public TreeDownMigrator_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        [InlineData("0432 C53East TS.cruise")]
        public void Tree_Test(string fileName)
        {
            var (orgFile, crz3File, origAgain) = SetUpTestFile(fileName);

            using (var dbv2 = new CruiseDatastore(orgFile))
            using (var dbv2Arain = new CruiseDatastore(origAgain))
            {
                var treev2Again = dbv2Arain.From<V2.Models.Tree>()
                    .Query().ToArray();
                treev2Again.Should().NotBeEmpty();

                var treev2 = dbv2.From<V2.Models.Tree>()
                    .Query().ToArray();
                treev2.Should().NotBeEmpty();

                treev2Again.Should().HaveSameCount(treev2);
                treev2Again.Should().BeEquivalentTo(treev2,
                    config => config
                    .Excluding(y => y.TreeDefaultValue_CN)
                    .Excluding(y => y.TreeFactor)
                    .Excluding(y => y.PointFactor)
                    .Excluding(y => y.ExpansionFactor)
                    .Excluding(y => y.Tree_GUID));
            }
        }
    }
}