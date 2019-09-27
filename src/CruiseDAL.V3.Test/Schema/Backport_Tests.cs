using CruiseDAL.DataObjects;
using CruiseDAL.V2.Models;
using CruiseDAL.V3.Models;
using CruiseDAL.V3.Tests;
using FluentAssertions;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test.Schema
{
    public class Backport_Tests : TestBase
    {
        public Backport_Tests(ITestOutputHelper output) : base(output)
        {
        }


        [Theory]
        [InlineData("7Wolf.cruise")]
        [InlineData("0432 C53East TS.cruise")]
        public void CuttingUnits_Test(string fileName)
        {
            var (orgFile, crz3File) = SetUpTestFile(fileName);

            using (var dbv2 = new CruiseDatastore(orgFile))
            using (var dbv3 = new CruiseDatastore_V3(crz3File))
            {
                var cuttingUnitsv3 = dbv3.From<V2.Models.CuttingUnit>()
                    .Query();

                var cuttingUnitsV2 = dbv2.From<V2.Models.CuttingUnit>()
                    .Query();

                cuttingUnitsv3.Should().BeEquivalentTo(cuttingUnitsV2, 
                    x => x.Excluding(y=> y.TallyHistory));
            }
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        [InlineData("0432 C53East TS.cruise")]
        public void Stratum_Test(string fileName)
        {
            var (orgFile, crz3File) = SetUpTestFile(fileName);

            using (var dbv2 = new CruiseDatastore(orgFile))
            using (var dbv3 = new CruiseDatastore_V3(crz3File))
            {
                var stratav3 = dbv3.From<V2.Models.Stratum>()
                    .Query();

                var stratav2 = dbv2.From<V2.Models.Stratum>()
                    .Query();

                stratav3.Should().BeEquivalentTo(stratav2);
            }
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        [InlineData("0432 C53East TS.cruise")]
        public void SampleGroup_Test(string fileName)
        {
            var (orgFile, crz3File) = SetUpTestFile(fileName);

            using (var dbv2 = new CruiseDatastore(orgFile))
            using (var dbv3 = new CruiseDatastore_V3(crz3File))
            {
                var sgv3 = dbv3.From<SampleGroup>()
                    .Query();

                var sgv2 = dbv2.From<SampleGroup>()
                    .Query();

                sgv3.Should().BeEquivalentTo(sgv2);
            }
        }



        [Theory]
        [InlineData("7Wolf.cruise")]
        [InlineData("0432 C53East TS.cruise")]
        public void CountTree_Test_only_positiveTreeCounts(string fileName)
        {
            var (orgFile, crz3File) = SetUpTestFile(fileName);

            using (var dbv2 = new CruiseDatastore(orgFile))
            using (var dbv3 = new CruiseDatastore_V3(crz3File))
            {
                var countTreeV3 = dbv3.From<CountTree>()
                    .Where("TreeCount > 0")
                    .Query();
                countTreeV3.Should().NotBeEmpty();

                var countTreeV2 = dbv2.From<CountTree>()
                    .Where("TreeCount > 0")
                    .GroupBy("CuttingUnit_CN", "SampleGroup_CN", "ifnull(TreeDefaultValue_CN, '')")
                    .Query();
                countTreeV2.Should().NotBeEmpty();

                countTreeV3.Should().HaveSameCount(countTreeV2);
                countTreeV3.Should().BeEquivalentTo(countTreeV2,
                    x => x.Excluding(y => y.Tally_CN)
                    .Excluding(y => y.CountTree_CN));
            }
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        [InlineData("0432 C53East TS.cruise")]
        public void CountTreeDO_Test_only_positiveTreeCounts(string fileName)
        {
            var (orgFile, crz3File) = SetUpTestFile(fileName);

            using (var dbv2 = new CruiseDatastore(orgFile))
            using (var dbv3 = new CruiseDatastore_V3(crz3File))
            {
                var countTreeV3 = dbv3.From<CountTreeDO>()
                    .Where("TreeCount > 0")
                    .Query();
                countTreeV3.Should().NotBeEmpty();

                var countTreeV2 = dbv2.From<CountTreeDO>()
                    .Where("TreeCount > 0")
                    .GroupBy("CuttingUnit_CN", "SampleGroup_CN", "ifnull(TreeDefaultValue_CN, '')")
                    .Query();
                countTreeV2.Should().NotBeEmpty();

                countTreeV3.Should().HaveSameCount(countTreeV2);
                //countTreeV3.Should().BeEquivalentTo(countTreeV2,
                //    x => x.Excluding(y => y.Tally_CN)
                //    .Excluding(y => y.CountTree_CN));
            }
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        [InlineData("0432 C53East TS.cruise")]
        public void Tree_Test(string fileName)
        {
            var (orgFile, crz3File) = SetUpTestFile(fileName);

            using (var dbv2 = new CruiseDatastore(orgFile))
            using (var dbv3 = new CruiseDatastore_V3(crz3File))
            {
                var treev3 = dbv3.From<Tree>()
                    .Query().ToArray();
                treev3.Should().NotBeEmpty();

                var treev2 = dbv2.From<Tree>()
                    .Query().ToArray();
                treev2.Should().NotBeEmpty();

                treev3.Should().HaveSameCount(treev2);
                treev3.Should().BeEquivalentTo(treev2,
                    x=> x.Excluding(y=>y.TreeFactor)
                    .Excluding(y=>y.PointFactor)
                    .Excluding(y=>y.ExpansionFactor)
                    .Excluding(y=>y.Tree_GUID));
            }
        }

        [Theory(Skip = "TreeDO doesn't work with CruiseDatastore_V3 right now")]
        [InlineData("7Wolf.cruise")]
        [InlineData("0432 C53East TS.cruise")]
        public void Tree_Test_single(string fileName)
        {
            var (orgFile, crz3File) = SetUpTestFile(fileName);

            using (var dbv2 = new DAL(orgFile))
            using (var dbv3 = new CruiseDatastore_V3(crz3File))
            {
                var treev2 = dbv2.From<TreeDO>()
                    .Query().ToArray();
                treev2.Should().NotBeEmpty();

                var singleTreev2 = treev2.First();

                var cuttingUnitCode = singleTreev2.CuttingUnit.Code;
                var stratumCode = singleTreev2.Stratum.Code;
                var treeNumber = singleTreev2.TreeNumber;

                var singleTreev3 = dbv3.From<TreeDO>()
                    .Join("CuttingUnit", "USING (CuttingUnit_CN)")
                    .Join("Stratum", "USING (Stratum_CN)")
                    .Where("CuttingUnit.Code = @p1 AND Stratum.Code = @p2 AND TreeNumber = @p3")
                    .Query(cuttingUnitCode, stratumCode, treeNumber)
                    .FirstOrDefault();

                singleTreev3.Should().BeEquivalentTo(singleTreev2, 
                    x => x.Excluding(y => y.DAL)
                    .Excluding(y => y.CuttingUnit)
                    .Excluding(y => y.Plot)
                    .Excluding(y=> y.SampleGroup)
                    .Excluding(y=> y.Self)
                    .Excluding(y=>y.Stratum)
                    .Excluding(y=>y.TreeDefaultValue)
                    .Excluding(y=>y.Validator)
                    .Excluding(y=>y.Error)
                    .Excluding(y=>y.Tree_GUID)
                    .Excluding(y=> ((DataObject)y).DAL)
                    .Excluding(y=>y.TreeValidator)
                    .Excluding(y=>y.ExpansionFactor)// exclude expansion factor, because this values is calculated during processing and the values is lost during migration
                    );

                var cuttingUnitv3 = singleTreev3.CuttingUnit;
                cuttingUnitv3.Should().NotBeNull();
            }
        }

        private (string, string) SetUpTestFile(string fileName)
        {
            var filePath = Path.Combine(TestFilesDirectory, fileName);

            var baseFileName = Path.GetFileName(fileName);
            var orgFile = Path.Combine(TestTempPath, fileName);
            var crz3File = (string)null;

            // create copy of base file
            if (File.Exists(orgFile) == false)
            {
                File.Copy(filePath, orgFile);
            }
            crz3File = Migrator.MigrateFromV2ToV3(orgFile, true);

            return (orgFile, crz3File);
        }
    }
}