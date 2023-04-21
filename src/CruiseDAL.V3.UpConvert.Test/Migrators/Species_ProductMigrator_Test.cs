using CruiseDAL.TestCommon;
using CruiseDAL.TestCommon.V2;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.UpConvert.Test.Migrators
{
    public class Species_ProductMigrator_Test : TestBase
    {
        public Species_ProductMigrator_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ConvetContractSpecies_Multiple_Prods_SameCS()
        {
            var v2Path = GetTempFilePathWithExt(".cruise");
            var v3Path = GetTempFilePathWithExt(".crz3");

            var v2init = new DatabaseInitializer_V2();
            v2init.TreeDefaults = null;
            v2init.CreateDatabaseFile(v2Path);

            var v2Db = new DAL(v2Path);

            var tdvs = new[]
            {
                new V2.Models.TreeDefaultValue
                {
                    Species = "sp1",
                    PrimaryProduct = "01",
                    ContractSpecies = "1",
                    LiveDead = "L",
                },
                new V2.Models.TreeDefaultValue
                {
                    Species = "sp1",
                    PrimaryProduct = "02",
                    ContractSpecies = "1",
                    LiveDead = "L",
                },

            };

            foreach(var tdv in tdvs)
            {
                v2Db.Insert(tdv);
            }


            var v3Db = new CruiseDatastore_V3(v3Path, true);

            var upConverter = new CruiseDAL.UpConvert.Migrator();
            upConverter.MigrateFromV2ToV3(v2Db, v3Db);

            var sp_prods = v3Db.From<V3.Models.Species_Product>().Query().ToArray();
            sp_prods.Should().HaveCount(1);
            var sp_prod = sp_prods.Single();
            sp_prod.SpeciesCode.Should().Be("sp1");
            sp_prod.PrimaryProduct.Should().BeNull();
            sp_prod.ContractSpecies.Should().Be("1");
        }

        [Fact]
        public void ConvetContractSpecies_SingleProd_MultipleCS()
        {
            var v2Path = GetTempFilePathWithExt(".cruise");
            var v3Path = GetTempFilePathWithExt(".crz3");

            var v2init = new DatabaseInitializer_V2();
            v2init.TreeDefaults = null;
            v2init.CreateDatabaseFile(v2Path);

            var v2Db = new DAL(v2Path);

            var tdvs = new[]
            {
                //new V2.Models.TreeDefaultValue
                //{
                //    Species = "sp1",
                //    PrimaryProduct = "01",
                //    ContractSpecies = "1",
                //    LiveDead = "L",
                //},
                new V2.Models.TreeDefaultValue
                {
                    Species = "sp1",
                    PrimaryProduct = "02",
                    ContractSpecies = "1",
                    LiveDead = "L",
                },
                new V2.Models.TreeDefaultValue
                {
                    Species = "sp1",
                    PrimaryProduct = "02",
                    ContractSpecies = "2",
                    LiveDead = "D",
                },

            };

            foreach (var tdv in tdvs)
            {
                v2Db.Insert(tdv);
            }


            var v3Db = new CruiseDatastore_V3(v3Path, true);

            var upConverter = new CruiseDAL.UpConvert.Migrator();
            upConverter.MigrateFromV2ToV3(v2Db, v3Db);

            var sp_prods = v3Db.From<V3.Models.Species_Product>().Query().ToArray();
            sp_prods.Should().HaveCount(0);
        }

        [Fact]
        public void ConvetContractSpecies_MultipleProd_MultipleCS()
        {
            var v2Path = GetTempFilePathWithExt(".cruise");
            var v3Path = GetTempFilePathWithExt(".crz3");

            var v2init = new DatabaseInitializer_V2();
            v2init.TreeDefaults = null;
            v2init.CreateDatabaseFile(v2Path);

            var v2Db = new DAL(v2Path);

            var tdvs = new[]
            {
                new V2.Models.TreeDefaultValue
                {
                    Species = "sp1",
                    PrimaryProduct = "01",
                    ContractSpecies = "1",
                    LiveDead = "L",
                },
                new V2.Models.TreeDefaultValue
                {
                    Species = "sp1",
                    PrimaryProduct = "02",
                    ContractSpecies = "2",
                    LiveDead = "L",
                },
            };

            foreach (var tdv in tdvs)
            {
                v2Db.Insert(tdv);
            }


            var v3Db = new CruiseDatastore_V3(v3Path, true);

            var upConverter = new CruiseDAL.UpConvert.Migrator();
            upConverter.MigrateFromV2ToV3(v2Db, v3Db);

            var sp_prods = v3Db.From<V3.Models.Species_Product>().Query().ToArray();
            sp_prods.Should().HaveCount(2);

            sp_prods.All(x => x.SpeciesCode == "sp1").Should().BeTrue();
            var first = sp_prods[0];
            first.PrimaryProduct.Should().Be("01");
            first.ContractSpecies.Should().Be("1");

            var second = sp_prods[1];
            second.PrimaryProduct.Should().Be("02");
            second.ContractSpecies.Should().Be("2");
        }

        // multiple product codes map to multiple contract species, however
        // one product has multiple contract species so we cant be sure 
        // what contract species to give it. 
        [Fact]
        public void ConvetContractSpecies_MultipleProd_MultipleCS_UnclearSPProdMap()
        {
            var v2Path = GetTempFilePathWithExt(".cruise");
            var v3Path = GetTempFilePathWithExt(".crz3");

            var v2init = new DatabaseInitializer_V2();
            v2init.TreeDefaults = null;
            v2init.CreateDatabaseFile(v2Path);

            var v2Db = new DAL(v2Path);

            var tdvs = new[]
            {
                new V2.Models.TreeDefaultValue
                {
                    Species = "sp1",
                    PrimaryProduct = "01",
                    ContractSpecies = "1",
                    LiveDead = "L",
                },
                new V2.Models.TreeDefaultValue
                {
                    Species = "sp1",
                    PrimaryProduct = "02",
                    ContractSpecies = "2",
                    LiveDead = "L",
                },
                new V2.Models.TreeDefaultValue
                {
                    Species = "sp1",
                    PrimaryProduct = "02",
                    ContractSpecies = "3",
                    LiveDead = "D",
                },
            };

            foreach (var tdv in tdvs)
            {
                v2Db.Insert(tdv);
            }


            var v3Db = new CruiseDatastore_V3(v3Path, true);

            var upConverter = new CruiseDAL.UpConvert.Migrator();
            upConverter.MigrateFromV2ToV3(v2Db, v3Db);

            var sp_prods = v3Db.From<V3.Models.Species_Product>().Query().ToArray();
            sp_prods.Should().HaveCount(1);

            sp_prods.All(x => x.SpeciesCode == "sp1").Should().BeTrue();
            var first = sp_prods[0];
            first.PrimaryProduct.Should().Be("01");
            first.ContractSpecies.Should().Be("1");
        }

        [Fact]
        public void ConvertContractSpecies_MultiTest()
        {
            var v2Path = GetTempFilePathWithExt(".cruise");
            var v3Path = GetTempFilePathWithExt(".crz3");

            var v2init = new DatabaseInitializer_V2();
            v2init.TreeDefaults = null;
            v2init.CreateDatabaseFile(v2Path);

            var v2Db = new DAL(v2Path);

            var tdvs = new[]
            {
                // sp1
                new V2.Models.TreeDefaultValue
                {
                    Species = "sp1",
                    PrimaryProduct = "01",
                    ContractSpecies = "1",
                    LiveDead = "L",
                },
                new V2.Models.TreeDefaultValue
                {
                    Species = "sp1",
                    PrimaryProduct = "02",
                    ContractSpecies = "2",
                    LiveDead = "L",
                },

                // sp2
                new V2.Models.TreeDefaultValue
                {
                    Species = "sp2",
                    PrimaryProduct = "01",
                    ContractSpecies = "1",
                    LiveDead = "L",
                },
                new V2.Models.TreeDefaultValue
                {
                    Species = "sp2",
                    PrimaryProduct = "02",
                    ContractSpecies = "1",
                    LiveDead = "L",
                },
            };

            foreach (var tdv in tdvs)
            {
                v2Db.Insert(tdv);
            }


            var v3Db = new CruiseDatastore_V3(v3Path, true);

            var upConverter = new CruiseDAL.UpConvert.Migrator();
            upConverter.MigrateFromV2ToV3(v2Db, v3Db);

            var sp_prods = v3Db.From<V3.Models.Species_Product>().Query().ToArray();
            sp_prods.Should().HaveCount(3);

            var sp1 = v3Db.From<V3.Models.Species_Product>().Where("SpeciesCode = @p1").Query("sp1").ToArray();
            sp1.Should().HaveCount(2);


            var sp2 = v3Db.From<V3.Models.Species_Product>().Where("SpeciesCode = @p1").Query("sp2").ToArray();
            sp2.Should().HaveCount(1);


        }
    }
}
