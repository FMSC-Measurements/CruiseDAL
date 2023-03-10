using CruiseDAL.Migrators;
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
    public class SpeciesMigrator_Test : TestBase
    {
        public SpeciesMigrator_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void ConvertFIACodes_withTestFile()
        {
            var fromPath = GetTestFile("testUpConvert.cruise");
            var toPath = GetTempFilePathWithExt(".crz3");

            using var fromDb = new DAL(fromPath);
            using var toDb = new CruiseDatastore_V3(toPath, true);

            var upConverter = new CruiseDAL.UpConvert.Migrator();
            upConverter.MigrateFromV2ToV3(fromDb, toDb);


            var species = toDb.From<Models.Species>().Query().ToArray();

            species.All(x => string.IsNullOrEmpty(x.FIACode) is false).Should().BeTrue();
        }

        [Fact]
        public void ConvertFIACodes()
        {
            var v2Path = GetTempFilePathWithExt(".cruise");
            var v3Path = GetTempFilePathWithExt(".crz3");

            var v2init = new DatabaseInitializer_V2();
            v2init.TreeDefaults = null;
            v2init.CreateDatabaseFile(v2Path);

            var v2Db = new DAL(v2Path);

            var tdv = new CruiseDAL.V2.Models.TreeDefaultValue()
            {
                Species = "sp1",
                FIAcode = 101,
                PrimaryProduct = "01",
                LiveDead = "L",
            };
            v2Db.Insert(tdv);

            var tdv2 = new CruiseDAL.V2.Models.TreeDefaultValue()
            {
                Species = "sp1",
                FIAcode = 101,
                PrimaryProduct = "02",
                LiveDead = "L",
            };
            v2Db.Insert(tdv2);

            var tdv3 = new CruiseDAL.V2.Models.TreeDefaultValue()
            {
                Species = "sp2",
                FIAcode = 102,
                PrimaryProduct = "02",
                LiveDead = "L",
            };
            v2Db.Insert(tdv3);


            var v3Db = new CruiseDatastore_V3(v3Path, true);

            var upConverter = new CruiseDAL.UpConvert.Migrator();
            upConverter.MigrateFromV2ToV3(v2Db, v3Db);


            var species = v3Db.From<Models.Species>().Query().ToArray();
            species.Should().HaveCount(2);
            species.Single(x => x.SpeciesCode == "sp1").FIACode.Should().Be("101");
            species.Single(x => x.SpeciesCode == "sp2").FIACode.Should().Be("102");

        }


        [Fact]
        public void ConvertFIACodes_dontConvertIfMultiple()
        {
            var v2Path = GetTempFilePathWithExt(".cruise");
            var v3Path = GetTempFilePathWithExt(".crz3");

            var v2init = new DatabaseInitializer_V2();
            v2init.TreeDefaults = null;
            v2init.CreateDatabaseFile(v2Path);

            var v2Db = new DAL(v2Path);

            var tdv = new CruiseDAL.V2.Models.TreeDefaultValue()
            {
                Species = "sp1",
                FIAcode = 101,
                PrimaryProduct = "01",
                LiveDead = "L",
            };
            v2Db.Insert(tdv);

            var tdv2 = new CruiseDAL.V2.Models.TreeDefaultValue()
            {
                Species = "sp1",
                FIAcode = 102,
                PrimaryProduct = "02",
                LiveDead = "L",
            };
            v2Db.Insert(tdv2);


            var v3Db = new CruiseDatastore_V3(v3Path, true);

            var upConverter = new CruiseDAL.UpConvert.Migrator();
            upConverter.MigrateFromV2ToV3(v2Db, v3Db);


            var species = v3Db.From<Models.Species>().Query().ToArray();
            species.Should().HaveCount(1);
            species.Single().FIACode.Should().BeNull();

            species.All(x => string.IsNullOrEmpty(x.FIACode) is true).Should().BeTrue();
        }
    }
}
