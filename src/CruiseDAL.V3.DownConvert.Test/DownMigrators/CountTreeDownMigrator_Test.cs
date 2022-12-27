using CruiseDAL.DataObjects;
using CruiseDAL.TestCommon;
using CruiseDAL.UpConvert;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.DownConvert.Test.DownMigrators
{
    public class CountTreeDownMigrator_Test : TestBase
    {
        public CountTreeDownMigrator_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        //[InlineData("7Wolf.cruise", Skip = "plot count records have positive count values")]
        [InlineData("0432 C53East TS.cruise")]
        // test that all positive counts are the same after round trip conversion
        // we only test positive counts because additional 0 count records can be
        // created in the conversion process
        public void RountTrip_CountTree_Test_only_positiveTreeCounts(string fileName)
        {
            var (orgFile, crz3File, origAgain) = SetUpTestFile(fileName);

            using (var dbv2 = new CruiseDatastore(orgFile))
            using (var dbv2Again = new CruiseDatastore(origAgain))
            {
                var countTreeV2Again = dbv2Again.From<V2.Models.CountTree>()
                    .Where("TreeCount > 0")
                    .Query();
                countTreeV2Again.Should().NotBeEmpty();

                var countTreeV2 = dbv2.From<V2.Models.CountTree>()
                    .Where("TreeCount > 0")
                    .GroupBy("CuttingUnit_CN", "SampleGroup_CN", "ifnull(TreeDefaultValue_CN, '')")
                    .Query();
                countTreeV2.Should().NotBeEmpty();

                countTreeV2Again.Should().HaveSameCount(countTreeV2);
                countTreeV2Again.Should().BeEquivalentTo(countTreeV2,
                    x => x.Excluding(y => y.Tally_CN)
                    .Excluding(y => y.CountTree_CN));
            }
        }

        //[Theory]
        //[InlineData("7Wolf.cruise")]
        //[InlineData("0432 C53East TS.cruise")]
        //public void CountTreeDO_Test_only_positiveTreeCounts(string fileName)
        //{
        //    var (orgFile, crz3File, origAgain) = SetUpTestFile(fileName);

        //    using (var dbv2 = new DAL(orgFile))
        //    using (var dbv2Again = new DAL(origAgain))
        //    {
        //        var countTreeV2Again = dbv2Again.From<CountTreeDO>()
        //            .Where("TreeCount > 0")
        //            .Query();
        //        countTreeV2Again.Should().NotBeEmpty();

        //        var countTreeV2 = dbv2.From<CountTreeDO>()
        //            .Where("TreeCount > 0")
        //            .GroupBy("CuttingUnit_CN", "SampleGroup_CN", "ifnull(TreeDefaultValue_CN, '')")
        //            .Query();
        //        countTreeV2.Should().NotBeEmpty();

        //        countTreeV2Again.Should().HaveSameCount(countTreeV2);
        //        //countTreeV3.Should().BeEquivalentTo(countTreeV2,
        //        //    x => x.Excluding(y => y.Tally_CN)
        //        //    .Excluding(y => y.CountTree_CN));
        //    }
        //}

        [Fact]
        public void CountTree_TreeBased_SingleTallyPop_SingleUnit()
        {
            var fromPath = GetTempFilePath(" CountTree_TreeBased_SingleTallyPop_SingleUnit_MigrateFromV3ToV2.crz3");
            var toPath = GetTempFilePath(" CountTree_TreeBased_SingleTallyPop_SingleUnit_MigrateFromV3ToV2.cruise");

            var init = new DatabaseInitializer()
            {
                Units = new[] { "u1" },
                Strata = new[] { new Stratum { StratumCode = "st1", Method = "STR" } },
                UnitStrata = new[] { new CuttingUnit_Stratum { StratumCode = "st1", CuttingUnitCode = "u1" } },
                SampleGroups = new[] { new SampleGroup { StratumCode = "st1", SampleGroupCode = "sg1", SamplingFrequency = 101, TallyBySubPop = false } },
                Subpops = new[] { new SubPopulation { StratumCode = "st1", SampleGroupCode = "sg1", SpeciesCode = "sp1", LiveDead = "L" } },
            };

            var expectedCountTreeCount = 1;

            using var fromdb = init.CreateDatabaseFile(fromPath);
            using var todb = new DAL(toPath, true);

            var unit = "u1";
            var stratum = "st1";
            var sg = "sg1";

            var tallyLedgers = new TallyLedger[]
            {
                new TallyLedger {CruiseID = init.CruiseID, CuttingUnitCode = "u1", StratumCode = "st1", SampleGroupCode = "sg1", TreeCount = 101, SpeciesCode = "sp1", LiveDead = "L"},
            };
            foreach(var tl in tallyLedgers)
            {
                tl.TallyLedgerID = Guid.NewGuid().ToString();
                fromdb.Insert(tl);
            }

            var tallyPops = fromdb.From<TallyPopulation>().Query().ToArray();
            tallyPops.Should().NotBeEmpty();

            var downMigrator = new DownMigrator();
            downMigrator.MigrateFromV3ToV2(init.CruiseID, fromdb, todb);

            var tls = fromdb.From<TallyLedger>().Query().ToArray();
            tls.Should().HaveCount(1);

            var countTrees = todb.From<V2.Models.CountTree>().Query().ToArray();
            countTrees.Should().HaveCount(expectedCountTreeCount);
            countTrees.Should().OnlyContain(x => x.TreeCount > 0);
        }

        [Fact]
        public void CountTree_TreeBased_SingleTallyPop_MultiUnit()
        {
            var fromPath = GetTempFilePath(" CountTree_TreeBased_SingleTallyPop_MultiUnit_MigrateFromV3ToV2.crz3");
            var toPath = GetTempFilePath(" CountTree_TreeBased_SingleTallyPop_MultiUnit_MigrateFromV3ToV2.cruise");

            var init = new DatabaseInitializer()
            {
                Units = new[] { "u1", "u2" },
                Strata = new[] { new Stratum { StratumCode = "st1", Method = "STR" } },
                UnitStrata = new[] { 
                    new CuttingUnit_Stratum { StratumCode = "st1", CuttingUnitCode = "u1" },
                    new CuttingUnit_Stratum { StratumCode = "st1", CuttingUnitCode = "u2" }
                },
                SampleGroups = new[] { new SampleGroup { StratumCode = "st1", SampleGroupCode = "sg1", SamplingFrequency = 101, TallyBySubPop = false } },
                Subpops = new[] { new SubPopulation { StratumCode = "st1", SampleGroupCode = "sg1", SpeciesCode = "sp1", LiveDead = "L" } },
            };

            var expectedCountTreeCount = 2;

            using var fromdb = init.CreateDatabaseFile(fromPath);
            using var todb = new DAL(toPath, true);

            var unit = "u1";
            var stratum = "st1";
            var sg = "sg1";

            var tallyLedgers = new TallyLedger[]
            {
                new TallyLedger {CruiseID = init.CruiseID, CuttingUnitCode = "u1", StratumCode = "st1", SampleGroupCode = "sg1", TreeCount = 101, SpeciesCode = "sp1", LiveDead = "L"},
            };
            foreach (var tl in tallyLedgers)
            {
                tl.TallyLedgerID = Guid.NewGuid().ToString();
                fromdb.Insert(tl);
            }

            var tallyPops = fromdb.From<TallyPopulation>().Query().ToArray();
            tallyPops.Should().NotBeEmpty();

            var downMigrator = new DownMigrator();
            downMigrator.MigrateFromV3ToV2(init.CruiseID, fromdb, todb);

            var tls = fromdb.From<TallyLedger>().Query().ToArray();
            tls.Should().HaveCount(1);

            var countTrees = todb.From<V2.Models.CountTree>().Query().ToArray();
            countTrees.Should().HaveCount(expectedCountTreeCount);

            var u1Counts = countTrees.First(x => x.CuttingUnit_CN == 1);
            u1Counts.TreeCount.Should().Be(101);

            var u2Counts = countTrees.First(x => x.CuttingUnit_CN == 2);
            u2Counts.TreeCount.Should().Be(0);
        }

        [Fact]
        public void CountTree_MultiTL()
        {
            var fromPath = GetTempFilePath("CountTree_MultiTL_MigrateFromV3ToV2.crz3");
            var toPath = GetTempFilePath("CountTree_MultiTL_MigrateFromV3ToV2.cruise");

            var init = new DatabaseInitializer()
            {
                Units = new[] { "u1", "u2" },
                Strata = new[] { new Stratum { StratumCode = "st1", Method = "STR" } },
                UnitStrata = new[] {
                    new CuttingUnit_Stratum { StratumCode = "st1", CuttingUnitCode = "u1" },
                    new CuttingUnit_Stratum { StratumCode = "st1", CuttingUnitCode = "u2" }
                },
                SampleGroups = new[] { new SampleGroup { StratumCode = "st1", SampleGroupCode = "sg1", SamplingFrequency = 101, TallyBySubPop = false } },
                Subpops = new[] { new SubPopulation { StratumCode = "st1", SampleGroupCode = "sg1", SpeciesCode = "sp1", LiveDead = "L" } },
            };

            var expectedCountTreeCount = 2;

            using var fromdb = init.CreateDatabaseFile(fromPath);
            using var todb = new DAL(toPath, true);

            var unit = "u1";
            var stratum = "st1";
            var sg = "sg1";

            var tallyLedgers = new TallyLedger[]
            {
                new TallyLedger {CruiseID = init.CruiseID, CuttingUnitCode = "u1", StratumCode = "st1", SampleGroupCode = "sg1", TreeCount = 101, SpeciesCode = "sp1", LiveDead = "L"},
                new TallyLedger {CruiseID = init.CruiseID, CuttingUnitCode = "u1", StratumCode = "st1", SampleGroupCode = "sg1", TreeCount = 103, SpeciesCode = "sp1", LiveDead = "L"},
            };
            foreach (var tl in tallyLedgers)
            {
                tl.TallyLedgerID = Guid.NewGuid().ToString();
                fromdb.Insert(tl);
            }

            var tallyPops = fromdb.From<TallyPopulation>().Query().ToArray();
            tallyPops.Should().NotBeEmpty();

            var downMigrator = new DownMigrator();
            downMigrator.MigrateFromV3ToV2(init.CruiseID, fromdb, todb);

            var tls = fromdb.From<TallyLedger>().Query().ToArray();
            tls.Should().HaveCount(2);

            var countTrees = todb.From<V2.Models.CountTree>().Query().ToArray();
            countTrees.Should().HaveCount(expectedCountTreeCount);

            var u1Counts = countTrees.First(x => x.CuttingUnit_CN == 1);
            u1Counts.TreeCount.Should().Be(204);

            var u2Counts = countTrees.First(x => x.CuttingUnit_CN == 2);
            u2Counts.TreeCount.Should().Be(0);
        }

        [Fact]
        public void CountTree_PlotBased()
        {
            var fromPath = GetTempFilePath("CountTree_PlotBased_MigrateFromV3ToV2.crz3");
            var toPath = GetTempFilePath("CountTree_PlotBased_MigrateFromV3ToV2.cruise");

            var init = new DatabaseInitializer()
            {
                Units = new[] { "u1", "u2" },
                Strata = new[] { new Stratum { StratumCode = "st1", Method = "PCM" } },
                UnitStrata = new[] {
                    new CuttingUnit_Stratum { StratumCode = "st1", CuttingUnitCode = "u1" },
                    new CuttingUnit_Stratum { StratumCode = "st1", CuttingUnitCode = "u2" }
                },
                SampleGroups = new[] { new SampleGroup { StratumCode = "st1", SampleGroupCode = "sg1", SamplingFrequency = 101, TallyBySubPop = false } },
                Subpops = new[] { new SubPopulation { StratumCode = "st1", SampleGroupCode = "sg1", SpeciesCode = "sp1", LiveDead = "L" } },
            };

            var expectedCountTreeCount = 2;

            using var fromdb = init.CreateDatabaseFile(fromPath);
            using var todb = new DAL(toPath, true);

            var unit = "u1";
            var stratum = "st1";
            var sg = "sg1";

            var tallyLedgers = new TallyLedger[]
            {
                new TallyLedger {CruiseID = init.CruiseID, CuttingUnitCode = "u1", StratumCode = "st1", SampleGroupCode = "sg1", TreeCount = 101, SpeciesCode = "sp1", LiveDead = "L"},
                new TallyLedger {CruiseID = init.CruiseID, CuttingUnitCode = "u1", StratumCode = "st1", SampleGroupCode = "sg1", TreeCount = 103, SpeciesCode = "sp1", LiveDead = "L"},
            };
            foreach (var tl in tallyLedgers)
            {
                tl.TallyLedgerID = Guid.NewGuid().ToString();
                fromdb.Insert(tl);
            }

            var tallyPops = fromdb.From<TallyPopulation>().Query().ToArray();
            tallyPops.Should().NotBeEmpty();

            var downMigrator = new DownMigrator();
            downMigrator.MigrateFromV3ToV2(init.CruiseID, fromdb, todb);

            var tls = fromdb.From<TallyLedger>().Query().ToArray();
            tls.Should().HaveCount(2);

            var countTrees = todb.From<V2.Models.CountTree>().Query().ToArray();
            countTrees.Should().HaveCount(expectedCountTreeCount);

            var u1Counts = countTrees.First(x => x.CuttingUnit_CN == 1);
            u1Counts.TreeCount.Should().Be(0);

            var u2Counts = countTrees.First(x => x.CuttingUnit_CN == 2);
            u2Counts.TreeCount.Should().Be(0);
        }

        private (string, string, string) SetUpTestFile(string fileName, [CallerMemberName] string caller = null)
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
            crz3File = new Migrator().MigrateFromV2ToV3(orgFile, true);


            var v2againPath = Path.Combine(TestTempPath, caller + "_again_" + fileName);
            using (var v2again = new DAL(v2againPath, true))
            using (var v3db = new CruiseDatastore_V3(crz3File))
            {
                var cruiseID = v3db.ExecuteScalar<string>("SELECT CruiseID FROM Cruise;");
                var downMigrator = new DownMigrator();
                downMigrator.MigrateFromV3ToV2(cruiseID, v3db, v2again);

            }

            return (orgFile, crz3File, v2againPath);
        }
    }
}
