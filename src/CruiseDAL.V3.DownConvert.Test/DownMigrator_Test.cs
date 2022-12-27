using CruiseDAL.TestCommon;
using CruiseDAL.UpConvert;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.DownConvert.Test
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
                SampleGroupCode = sg.SampleGroupCode,
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

            var tm = new TreeMeasurment
            {
                TreeID = tree.TreeID,
            };
            fromDb.Insert(tm);

            using var toDb = new DAL(toPath, true);

            var downMigrator = new DownMigrator();
            downMigrator.MigrateFromV3ToV2(initializer.CruiseID, fromDb, toDb);

            var treeAgain = toDb.From<V2.Models.Tree>().Where("TreeNumber = 101").Query().Single();
            treeAgain.Should().NotBeNull();
            treeAgain.DBH.Should().Be(12.0);
            treeAgain.DRC.Should().Be(13.0);

            ValidateMigration(fromDb, toDb);
        }

        private void ValidateMigration(CruiseDatastore v3db, CruiseDatastore v2db)
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

            var v3Path = new Migrator().MigrateFromV2ToV3(tempPath, true);

            using (var v2db = new DAL(tempPath))
            using (var v3Database = new CruiseDatastore_V3(v3Path))
            {
                var units = v2db.From<V2.Models.CuttingUnit>().Query();
                var strata = v2db.From<V2.Models.Stratum>().Query().ToArray();
                var samplegroups = v2db.From<V2.Models.SampleGroup>().Query();
                var plots = v2db.From<V2.Models.Plot>().Query();
                var trees = v2db.From<V2.Models.Tree>().Query();
                //                var countTrees = v2db.Query<TreeCNTTotals>(
                //@"SELECT CuttingUnit_CN, SampleGroup_CN, ifnull(TreeDefaultValue_CN, 0) AS TreeDefaultValue_CN, Sum(TreeCount) AS TreeCount, sum(SumKPI) AS SumKPI FROM CountTree
                //GROUP BY CuttingUnit_CN, SampleGroup_CN, ifnull(TreeDefaultValue_CN, 0);").ToArray();

                var treeCounts = v2db.Query<TreeCNTTotals>(
@"SELECT cnt.CuttingUnit_CN, cnt.SampleGroup_CN, cnt.TreeDefaultValue_CN, cnt.TreeCount + sum(TreeCNTTotal) AS TreeCNT, cnt.SumKPI
 FROM CountTree AS cnt
 JOIN (SELECT CuttingUnit_CN, SampleGroup_CN, TreeDefaultValue_CN, Sum(TreeCount) as TreeCNTTotal
    FROM Tree
    GROUP BY CuttingUnit_CN, SampleGroup_CN, ifnull(TreeDefaultValue_CN, 0)) AS tcnt on tcnt.CuttingUnit_CN = cnt.CuttingUnit_CN
                        AND tcnt.SampleGroup_CN = cnt.SampleGroup_CN
                        AND (cnt.TreeDefaultValue_CN IS NULL OR ifnull(tcnt.TreeDefaultValue_CN, 0) = ifnull(cnt.TreeDefaultValue_CN, 0))

GROUP BY cnt.CuttingUnit_CN, cnt.SampleGroup_CN, cnt.TreeDefaultValue_CN
ORDER BY cnt.CuttingUnit_CN, cnt.SampleGroup_CN, cnt.TreeDefaultValue_CN;").ToArray();

                var cruise = v3Database.From<Cruise>().Query().Single();
                var cruiseID = cruise.CruiseID;

                using (var v2again = new DAL(reconvertPath, true))
                {
                    var tableInfo = v2again.GetTableInfo("Sale");

                    var downMigrator = new DownMigrator();
                    downMigrator.MigrateFromV3ToV2(cruiseID, v3Database, v2again, "test");

                    //                    var countTreesAgain = v2again.Query<TreeCNTTotals>(
                    //@"SELECT CuttingUnit_CN, SampleGroup_CN, ifnull(TreeDefaultValue_CN, 0) AS TreeDefaultValue_CN, Sum(TreeCount) AS TreeCount, sum(SumKPI) AS SumKPI FROM CountTree
                    //GROUP BY CuttingUnit_CN, SampleGroup_CN, ifnull(TreeDefaultValue_CN, 0);").ToArray();

                    //                    var countTreeDiff = countTreesAgain.Except(countTrees).ToArray();
                    //                    countTreesAgain.Should().BeEquivalentTo(countTrees);

                    var treeCountsAgain = v2again.Query<TreeCNTTotals>(
@"SELECT cnt.CuttingUnit_CN, cnt.SampleGroup_CN, cnt.TreeDefaultValue_CN, cnt.TreeCount + sum(TreeCNTTotal) AS TreeCNT, cnt.SumKPI
 FROM CountTree AS cnt
 JOIN (SELECT CuttingUnit_CN, SampleGroup_CN, TreeDefaultValue_CN, Sum(TreeCount) as TreeCNTTotal
    FROM Tree
    GROUP BY CuttingUnit_CN, SampleGroup_CN, ifnull(TreeDefaultValue_CN, 0)) AS tcnt on tcnt.CuttingUnit_CN = cnt.CuttingUnit_CN
                        AND tcnt.SampleGroup_CN = cnt.SampleGroup_CN
                        AND (cnt.TreeDefaultValue_CN IS NULL OR ifnull(tcnt.TreeDefaultValue_CN, 0) = ifnull(cnt.TreeDefaultValue_CN, 0))
GROUP BY cnt.CuttingUnit_CN, cnt.SampleGroup_CN, cnt.TreeDefaultValue_CN
ORDER BY cnt.CuttingUnit_CN, cnt.SampleGroup_CN, cnt.TreeDefaultValue_CN;").ToArray();

                    var treeCountDiff = treeCountsAgain.Except(treeCounts).ToArray();
                    treeCountsAgain.Should().BeEquivalentTo(treeCounts);

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
                    plotsAgain.Should().BeEquivalentTo(plots,
                        config => config
                        .Excluding(x => x.Plot_GUID) // Plot_Stratum doesn't have a guid
                        .Using<string>(ctx => ctx.Subject.Should().Be(ctx.Expectation ?? "False")) // v3 will auto populate IsEmpty with False if null
                        .When(info => info.SelectedMemberPath.Equals(nameof(V2.Models.Plot.IsEmpty))));

                    var treesAgain = v2again.From<V2.Models.Tree>().Query();
                    treesAgain.Should().BeEquivalentTo(trees, congig => congig
                        .Excluding(x => x.Tree_GUID)
                        .Excluding(x => x.TreeDefaultValue_CN)
                        .Excluding(x => x.ExpansionFactor)
                        .Excluding(x => x.TreeFactor)
                        .Excluding(x => x.PointFactor)
                        .Excluding(x => x.TreeCount) // tree count may get combined into the count tree table
                    );
                }
            }
        }

        [InlineData("R2_Test.cruise")]
        [InlineData("testRoundTripConvert.cruise")]
        [Theory]
        public void RoundTripV2Migration_ignoreCountTreeRecordCount(string fileName)
        {
            var filePath = Path.Combine(TestFilesDirectory, fileName);
            var reconvertPath = Path.Combine(TestTempPath, "again_" + fileName);
            Output.WriteLine(reconvertPath);

            // copy file to test temp dir
            var tempPath = Path.Combine(TestTempPath, fileName);
            File.Copy(filePath, tempPath, true);

            var v3Path = new Migrator().MigrateFromV2ToV3(tempPath, true);

            using (var v2db = new DAL(tempPath))
            using (var v3Database = new CruiseDatastore_V3(v3Path))
            {
                var units = v2db.From<V2.Models.CuttingUnit>().Query();
                var strata = v2db.From<V2.Models.Stratum>().Query().ToArray();
                var samplegroups = v2db.From<V2.Models.SampleGroup>().Query();
                var plots = v2db.From<V2.Models.Plot>().Query();
                var trees = v2db.From<V2.Models.Tree>().Query();
                var reports = v2db.From<V2.Models.Reports>().Query();

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
                    plotsAgain.Should().BeEquivalentTo(plots,
                        config => config
                        .Excluding(x => x.Plot_GUID) // Plot_Stratum doesn't have a guid
                        .Using<string>(ctx => ctx.Subject.Should().Be(ctx.Expectation ?? "False")) // v3 will auto populate IsEmpty with False if null
                        .When(info => info.SelectedMemberPath.Equals(nameof(V2.Models.Plot.IsEmpty))));

                    var treesAgain = v2again.From<V2.Models.Tree>().Query();
                    treesAgain.Should().BeEquivalentTo(trees, congig => congig
                        .Excluding(x => x.Tree_GUID)
                        .Excluding(x => x.TreeDefaultValue_CN)
                        .Excluding(x => x.ExpansionFactor)
                        .Excluding(x => x.TreeFactor)
                        .Excluding(x => x.PointFactor)
                        .Excluding(x => x.TreeCount) // tree count may get combined into the count tree table
                        .Excluding(x => x.HiddenPrimary)
                    );

                    var reportsAgain = v2again.From<V2.Models.Reports>().Query();
                    reportsAgain.Should().BeEquivalentTo(reports);
                }
            }
        }

        [Fact]
        public void EnsureCanMigrate_CheckAllSubPopsHaveTDV_HasTDVs()
        {
            var v3Path = GetTempFilePathWithExt(".crz3");

            var init = new DatabaseInitializer
            {
                TreeDefaults = new[]
                {
                    new TreeDefaultValue {SpeciesCode = "sp1", PrimaryProduct = "01"},
                    new TreeDefaultValue {SpeciesCode = "sp2", PrimaryProduct = "01"},
                    new TreeDefaultValue {SpeciesCode = "sp3", PrimaryProduct = "01"},
                }
            };
            using var v3db = init.CreateDatabase();

            var result = DownMigrator.CheckAllSubPopsHaveTDV(init.CruiseID, v3db);
            result.Should().BeEmpty();
        }

        [Fact]
        public void EnsureCanMigrate_CheckAllSubPopsHaveTDV_MissingTDVs()
        {
            var v3Path = GetTestFile("Fails_Subpop_DesignCheck.crz3");

            using var v3db = new CruiseDatastore_V3(v3Path);
            var cruiseID = v3db.ExecuteScalar<string>("SELECT CruiseID FROM Cruise LIMIT 1");

            var result = DownMigrator.CheckAllSubPopsHaveTDV(cruiseID, v3db);
            result.Should().NotBeEmpty();

            var downmigrator = new DownMigrator();

            // demonstrate that this file has trees with out TDVs
            v3db.From<CruiseDAL.V3.Models.Tree>().Where("SpeciesCode IS NULL OR LiveDead IS NULL").Query().Should().BeEmpty();

            var v2Path = GetTempFilePathWithExt(".cruise");
            using var v2db = new DAL(v2Path, true);
            downmigrator.MigrateFromV3ToV2(cruiseID, v3db, v2db);
            v2db.From<CruiseDAL.V2.Models.Tree>().Where("TreeDefaultValue_CN IS NULL").Query().Should().NotBeEmpty();
        }

        [Fact]
        public void EnsureCanMigrate_CheckAllSubPopsHaveTDV_DoesntHasTDVs()
        {
            var v3Path = GetTempFilePathWithExt(".crz3");

            var init = new DatabaseInitializer
            {
                TreeDefaults = new TreeDefaultValue[] { },
            };
            using var v3db = init.CreateDatabase();

            var result = DownMigrator.CheckAllSubPopsHaveTDV(init.CruiseID, v3db);
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void DOWN_MIGRATORS_Contains_All_Migrators()
        {
            var assembly = Assembly.GetAssembly(typeof(DownMigrator));
            var foundMigratorTypes = assembly.GetTypes()
                .Where(x => typeof(IDownMigrator).IsAssignableFrom(x) && x.IsClass)
                .ToHashSet();

            var migrators = DownMigrator.DOWN_MIGRATORS
                .Select(x => x.GetType()).ToHashSet();

            foundMigratorTypes.Should().HaveCount(migrators.Count);
            foundMigratorTypes.IsSubsetOf(migrators).Should().BeTrue();
        }

        protected record TreeCNTTotals
        {
            public int CuttingUnit_CN { get; set; }
            public int SampleGroup_CN { get; set; }
            public int? TreeDefaultValue_CN { get; set; }
            public int TreeCNT { get; set; }
            public int TreeCount { get; set; }
            public int SumKPI { get; set; }
        }
    }
}