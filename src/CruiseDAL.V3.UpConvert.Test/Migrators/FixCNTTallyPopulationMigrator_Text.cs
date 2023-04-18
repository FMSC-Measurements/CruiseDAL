using CruiseDAL.TestCommon;
using CruiseDAL.UpConvert;
using FluentAssertions;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.UpConvert.Test.Migrators
{
    public class FixCNTTallyPopulationMigrator_Text : TestBase
    {
        public FixCNTTallyPopulationMigrator_Text(ITestOutputHelper output) : base(output)
        {
        }

        // in Cruise Manager When a subpopulation is removed
        // the FixCNTTallyPopulation record if there is one
        // is not deleted. This can create a ForeignKey error
        // when converting FixCNTTallyPopulation records
        // because in V3 a FixCNTTallyPopulation must have
        // a subpop record.
        [Fact]
        public void Issue_Missing_Subpop()
        {
            var v2Path = base.GetTestFile("08041 Marshall HFR.cruise");

            var v3Path = GetTempFilePath("08041 Marshall HFR.crz3");

            using var v2Db = new DAL(v2Path);

            var fixCNTSubPops = v2Db.From<V2.Models.SampleGroupTreeDefaultValue>()
                .Join("FixCNTTallyPopulation", "USING (SampleGroup_CN, TreeDefaultValue_CN)")
                .Query().ToArray();

            fixCNTSubPops.Should().HaveCount(2);

            using var v3Db = new CruiseDatastore_V3(v3Path, true);

            new Migrator().MigrateFromV2ToV3(v2Db, v3Db);

            var fixCNTTallyPops = v3Db.From<V3.Models.FixCNTTallyPopulation>().Query().ToArray();
            fixCNTTallyPops.Should().HaveCount(2);
        }
    }
}