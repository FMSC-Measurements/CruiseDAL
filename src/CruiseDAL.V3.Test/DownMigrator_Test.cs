using CruiseDAL.DataObjects;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test
{
    public class DownMigrator_Test : TestBase
    {
        public DownMigrator_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void MigrateFromV3ToV2()
        {
            var initializer = new DatabaseInitializer();
            var fromPath = GetTempFilePath("MigrateFromV3ToV2.crz3");
            var toPath = GetTempFilePath("MigrateFromV3ToV2.cruise");

            using var fromDb = initializer.CreateDatabaseFile(fromPath);
            using var toDb = new DAL(toPath, true);

            var downMigrator = new DownMigrator();
            downMigrator.MigrateFromV3ToV2(initializer.CruiseID, fromDb, toDb);

            ValidateMigration(fromDb, toDb);

        }

        [Fact]
        public void MigrateFromV3ToV2_With_FieldDefaults()
        {
            var initializer = new DatabaseInitializer();
            var fromPath = GetTempFilePath("MigrateFromV3ToV2.crz3");
            var toPath = GetTempFilePath("MigrateFromV3ToV2.cruise");

            using var fromDb = initializer.CreateDatabaseFile(fromPath);

            var sg = initializer.SampleGroups.First();
            fromDb.Insert(new TreeFieldSetup()
            {
                CruiseID = initializer.CruiseID,
                SampleGroupCode  = sg.SampleGroupCode,
                StratumCode = sg.StratumCode,
                Field = "DBH",
                DefaultValueReal = 12.0,
            });

            fromDb.Insert(new TreeFieldSetup()
            {
                CruiseID = initializer.CruiseID,
                StratumCode = sg.StratumCode,
                Field = "DBH",
                DefaultValueReal = 11.0,
            });

            fromDb.Insert(new TreeFieldSetup()
            {
                CruiseID = initializer.CruiseID,
                StratumCode = sg.StratumCode,
                Field = "DRC",
                DefaultValueReal = 13.0,
            });

            var tree = new Tree()
            {
                CruiseID = initializer.CruiseID,
                TreeID = Guid.NewGuid().ToString(),
                TreeNumber = 101,
                CuttingUnitCode = initializer.Units.First(),
                StratumCode = sg.StratumCode,
                SampleGroupCode = sg.SampleGroupCode,
            };
            fromDb.Insert(tree);


            using var toDb = new DAL(toPath, true);

            var downMigrator = new DownMigrator();
            downMigrator.MigrateFromV3ToV2(initializer.CruiseID, fromDb, toDb);

            var treeAgain = toDb.From<V2.Models.Tree>().Where("TreeNumber = 101").Query().Single();
            treeAgain.Should().NotBeNull();
            treeAgain.DBH.Should().Be(12.0);
            treeAgain.DRC.Should().Be(13.0);

            ValidateMigration(fromDb, toDb);

        }

        void ValidateMigration(CruiseDatastore v3db, CruiseDatastore v2db)
        {
            var units = v3db.From<V3.Models.CuttingUnit>().Query();
            var strata = v3db.From<V3.Models.Stratum>().Query().ToArray();
            var samplegroups = v3db.From<V3.Models.SampleGroup>().Query();
            var plots = v3db.From<V3.Models.Plot>().Query();
            var trees = v3db.From<V3.Models.Tree>().Query();

            var units2 = v2db.From<V2.Models.CuttingUnit>().Query();
            var strata2 = v2db.From<V2.Models.Stratum>().Query().ToArray();
            var samplegroups2 = v2db.From<V2.Models.SampleGroup>().Query();
            var plots2 = v2db.From<V2.Models.Plot>().Query();
            var trees2 = v2db.From<V2.Models.Tree>().Query();

            units.Should().HaveSameCount(units2);
            strata.Should().HaveSameCount(strata2);
            samplegroups.Should().HaveSameCount(samplegroups2);
            plots.Should().HaveSameCount(plots2);
            trees.Should().HaveSameCount(trees2);
        }

        [Theory]
        [InlineData("7Wolf.cruise")]
        [InlineData("MultiTest.2014.10.31.cruise")]
        public void RoundTripV2Migration(string fileName)
        {
            var filePath = Path.Combine(TestFilesDirectory, fileName);
            var reconvertPath = Path.Combine(TestTempPath, "again_" + fileName);
            Output.WriteLine(reconvertPath);

            // copy file to test temp dir
            var tempPath = Path.Combine(TestTempPath, fileName);
            File.Copy(filePath, tempPath, true);

            var v3Path = Migrator.MigrateFromV2ToV3(tempPath, true);

            using (var v2db = new DAL(tempPath))
            using (var v3Database = new CruiseDatastore_V3(v3Path))
            {
                var units = v2db.From<V2.Models.CuttingUnit>().Query();
                var strata = v2db.From<V2.Models.Stratum>().Query().ToArray();
                var samplegroups = v2db.From<V2.Models.SampleGroup>().Query();
                var plots = v2db.From<V2.Models.Plot>().Query();
                var trees = v2db.From<V2.Models.Tree>().Query();

                var cruise = v3Database.From<Cruise>().Query().Single();
                var cruiseID = cruise.CruiseID;

                using (var v2again = new DAL(reconvertPath, true))
                {
                    var tableInfo = v2again.GetTableInfo("Sale");

                    var downMigrator = new DownMigrator();
                    downMigrator.MigrateFromV3ToV2(cruiseID, v3Database, v2again, "test");

                    var unitsAgain = v2again.From<V2.Models.CuttingUnit>().Query();
                    unitsAgain.Should().BeEquivalentTo(units, config => config
                        .Using<string>(x => x.Subject.Should().Be(x.Expectation?.Trim())).WhenTypeIs<string>()// ignore whitespace when type is string
                        .Excluding(x => x.TallyHistory)
                    );

                    //var strataAgain = v2again.From<V2.Models.Stratum>().Query();
                    //strataAgain.Should().Should().BeEquivalentTo(strata);

                    var samplegroupsAgain = v2again.From<V2.Models.SampleGroup>().Query();
                    samplegroupsAgain.Should().BeEquivalentTo(samplegroups, config => config
                        .Excluding(x => x.SampleSelectorState)
                        .Excluding(x => x.SampleSelectorType)
                        .Excluding(x => x.TallyMethod)
                    );

                    var plotsAgain = v2again.From<V2.Models.Plot>().Query();
                    plotsAgain.Should().BeEquivalentTo(plots);

                    var treesAgain = v2again.From<V2.Models.Tree>().Query();
                    treesAgain.Should().BeEquivalentTo(trees, congig => congig
                        .Excluding(x => x.Tree_GUID)
                        .Excluding(x => x.TreeDefaultValue_CN)
                        .Excluding(x => x.ExpansionFactor)
                        .Excluding(x => x.TreeFactor)
                        .Excluding(x => x.PointFactor)
                    );
                }
            }
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



        [Theory]
        [InlineData("7Wolf.cruise")]
        [InlineData("0432 C53East TS.cruise")]
        public void CountTree_Test_only_positiveTreeCounts(string fileName)
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

        [Theory]
        [InlineData("7Wolf.cruise")]
        [InlineData("0432 C53East TS.cruise")]
        public void CountTreeDO_Test_only_positiveTreeCounts(string fileName)
        {
            var (orgFile, crz3File, origAgain) = SetUpTestFile(fileName);

            using (var dbv2 = new DAL(orgFile))
            using (var dbv2Again = new DAL(origAgain))
            {
                var countTreeV2Again = dbv2Again.From<CountTreeDO>()
                    .Where("TreeCount > 0")
                    .Query();
                countTreeV2Again.Should().NotBeEmpty();

                var countTreeV2 = dbv2.From<CountTreeDO>()
                    .Where("TreeCount > 0")
                    .GroupBy("CuttingUnit_CN", "SampleGroup_CN", "ifnull(TreeDefaultValue_CN, '')")
                    .Query();
                countTreeV2.Should().NotBeEmpty();

                countTreeV2Again.Should().HaveSameCount(countTreeV2);
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
            crz3File = Migrator.MigrateFromV2ToV3(orgFile, true);


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