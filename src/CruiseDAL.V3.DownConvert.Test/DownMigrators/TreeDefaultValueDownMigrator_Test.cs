using CruiseDAL.DownMigrators;
using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test.DownMigrators
{
    public class TreeDefaultValueDownMigrator_Test : TestBase
    {
        public TreeDefaultValueDownMigrator_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void MigrateFromV3ToV2()
        {
            var fromPath = GetTempFilePath(" TreeDefaultValueDownMigrator_MigrateFromV3ToV2.crz3");
            var toPath = GetTempFilePath(" TreeDefaultValueDownMigrator_MigrateFromV3ToV2.cruise");

            var initializer = new DatabaseInitializer()
            {
                Species = new[] { "sp1", "sp2", "sp3" },
                TreeDefaults = new[]
                {
                    new TreeDefaultValue {SpeciesCode = "sp1", PrimaryProduct = "01", Recoverable = 1.1,},
                    new TreeDefaultValue {SpeciesCode = null, PrimaryProduct = "01", Recoverable = 1.2,},
                    new TreeDefaultValue {SpeciesCode = "sp1", PrimaryProduct = null, Recoverable = 1.3,},
                    new TreeDefaultValue {SpeciesCode = null, PrimaryProduct = null, Recoverable = 1.4,},
                },
                Subpops = new SubPopulation[] { },
            };

            using var fromDb = initializer.CreateDatabaseFile(fromPath);
            using var toDb = new DAL(toPath, true);

            var downMigrator = new DownMigrator(new[] { new TreeDefaultValueDownMigrate(), });
            downMigrator.MigrateFromV3ToV2(initializer.CruiseID, fromDb, toDb);

            var prodCount = fromDb.From<LK_Product>().Count();
            var spCount = initializer.Species.Count();

            // from the one TDV with explicitly set species and prod we should get two TDVs (one live one dead
            var tdvs = toDb.From<V2.Models.TreeDefaultValue>().Where("Species = @p1 AND PrimaryProduct = @p2").Query("sp1", "01").ToArray();
            tdvs.Should().HaveCount(2);
            tdvs.Should().OnlyContain(x => x.Recoverable == 1.1);

            // from the TDV with explicitly set product, we should have 2 TDVs for each species minus the one that was created with explicit sp and prod
            var tdvFromProd = toDb.From<V2.Models.TreeDefaultValue>().Where("PrimaryProduct = @p1 AND Species != @p2 ").Query("01", "sp1").ToArray();
            tdvFromProd.Should().HaveCount((int)((spCount - 1) * 2));
            tdvFromProd.Should().OnlyContain(x => x.Recoverable == 1.2);

            // from the TDV with explicitly set species, we should have 2 TDVs for each product minus the one that was created with explicit sp and prod
            var tdvFromSp = toDb.From<V2.Models.TreeDefaultValue>().Where("Species = @p1 AND PrimaryProduct != @p2").Query("sp1", "01").ToArray();
            tdvFromSp.Should().HaveCount((int)((prodCount - 1) * 2));
            tdvFromSp.Should().OnlyContain(x => x.Recoverable == 1.3);

            // from the TDV with neither species or product defined, we should get (sp -1 * prod - 1) * 2 TDVs
            var tdvFromAnyAny = toDb.From<V2.Models.TreeDefaultValue>().Where("Species != @p1 AND PrimaryProduct != @p2").Query("sp1", "01").ToArray();
            tdvFromAnyAny.Should().HaveCount((int)((spCount - 1) * (prodCount - 1) * 2));
            tdvFromAnyAny.Should().OnlyContain(x => x.Recoverable == 1.4);

            var expectedTDVcount = (1 + (spCount - 1) + (prodCount - 1) + ((spCount - 1) * (prodCount - 1))) * 2;
            var alltdv = toDb.From<V2.Models.TreeDefaultValue>().Query().ToArray();
            alltdv.Should().HaveCount((int)expectedTDVcount);
        }
    }
}