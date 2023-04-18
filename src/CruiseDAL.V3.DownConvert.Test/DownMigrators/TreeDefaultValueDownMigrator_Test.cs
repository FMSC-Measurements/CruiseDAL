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

            alltdv.Should().OnlyContain(x => x.TreeGrade == "0");
        }

        [Fact]
        public void ExpandContractSpecies_AllProd()
        {
            var fromPath = GetTempFilePath("ExpandContractSpecies_AllProd.crz3");
            var toPath = GetTempFilePath("ExpandContractSpecies_AllProd.cruise");

            var init = new DatabaseInitializer()
            {
                Species = new[] { "sp1", },
                TreeDefaults = new[]
                {
                    new TreeDefaultValue {SpeciesCode = "sp1", PrimaryProduct = "01", Recoverable = 1.1,},
                    new TreeDefaultValue {SpeciesCode = null, PrimaryProduct = "01", Recoverable = 1.2,},
                    new TreeDefaultValue {SpeciesCode = "sp1", PrimaryProduct = null, Recoverable = 1.3,},
                    new TreeDefaultValue {SpeciesCode = null, PrimaryProduct = null, Recoverable = 1.4,},
                },
                Subpops = new SubPopulation[] { },
                SpProds = null,
            };

            using var fromDb = init.CreateDatabaseFile(fromPath);
            using var toDb = new DAL(toPath, true);

            var spProd = new Species_Product
            {
                CruiseID = init.CruiseID,
                SpeciesCode = "sp1",
                ContractSpecies = "something",
            };
            fromDb.Insert(spProd);


            var downMigrator = new DownMigrator(new[] { new TreeDefaultValueDownMigrate(), });
            downMigrator.MigrateFromV3ToV2(init.CruiseID, fromDb, toDb);

            var tdvs = toDb.From<V2.Models.TreeDefaultValue>().Query().ToArray();
            tdvs.Should().OnlyContain(x => x.ContractSpecies != null);
        }

        [Fact]
        public void ExpandContractSpecies_SpecificProd()
        {
            var fromPath = GetTempFilePath("ExpandContractSpecies_SpecificProd.crz3");
            var toPath = GetTempFilePath("ExpandContractSpecies_SpecificProd.cruise");

            var specifProd = "01";

            var init = new DatabaseInitializer()
            {
                Species = new[] { "sp1", },
                TreeDefaults = new[]
                {
                    new TreeDefaultValue {SpeciesCode = "sp1", PrimaryProduct = specifProd, Recoverable = 1.1,},
                    new TreeDefaultValue {SpeciesCode = null, PrimaryProduct = specifProd, Recoverable = 1.2,},
                    new TreeDefaultValue {SpeciesCode = "sp1", PrimaryProduct = null, Recoverable = 1.3,},
                    new TreeDefaultValue {SpeciesCode = null, PrimaryProduct = null, Recoverable = 1.4,},
                },
                Subpops = new SubPopulation[] { },
                SpProds = null,
            };

            using var fromDb = init.CreateDatabaseFile(fromPath);
            using var toDb = new DAL(toPath, true);

            var spProd = new Species_Product
            {
                CruiseID = init.CruiseID,
                SpeciesCode = "sp1",
                ContractSpecies = "something",
                PrimaryProduct = specifProd,
            };
            fromDb.Insert(spProd);


            var downMigrator = new DownMigrator(new[] { new TreeDefaultValueDownMigrate(), });
            downMigrator.MigrateFromV3ToV2(init.CruiseID, fromDb, toDb);

            var tdvs = toDb.From<V2.Models.TreeDefaultValue>().Query().ToArray();
            // all TDVs with our specified product code should have a contract species
            tdvs.Where(x => x.PrimaryProduct == specifProd).Should().OnlyContain(x => x.ContractSpecies != null);
            // no TDVs without our specified product code should have a contract species
            tdvs.Where(x => x.PrimaryProduct != specifProd).Should().OnlyContain(x => x.ContractSpecies == null);
        }

        [Fact]
        public void NoContractSpecies()
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

            fromDb.Execute("DELETE FROM Species_Product;");

            using var toDb = new DAL(toPath, true);

            var downMigrator = new DownMigrator(new[] { new TreeDefaultValueDownMigrate(), });
            downMigrator.MigrateFromV3ToV2(initializer.CruiseID, fromDb, toDb);

            // from the one TDV with explicitly set species and prod we should get two TDVs (one live one dead
            var tdvs = toDb.From<V2.Models.TreeDefaultValue>().Query().ToArray();

            tdvs.Should().OnlyContain(x => x.ContractSpecies == null);
        }
    }
}