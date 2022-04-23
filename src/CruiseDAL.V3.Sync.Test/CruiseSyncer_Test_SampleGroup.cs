using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CruiseDAL.V3.Sync
{
    public partial class CruiseSyncer_Test
    {
        [Fact]
        public void Sync_SampleGroup_Add()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "SampleGroup_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "SampleGroup_Add_toFile");

            var syncOptions = new CruiseSyncOptions();

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var sampleGroupID = Guid.NewGuid().ToString();
            fromDb.Insert(new SampleGroup()
            {
                CruiseID = cruiseID,
                SampleGroupID = sampleGroupID,
                SampleGroupCode = "10",
                StratumCode = Strata[0].StratumCode,
            });
            var newSampleGroup = fromDb.From<SampleGroup>().Where("SampleGroupID = @p1").Query(sampleGroupID).Single();

            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var sampleGroupAgain = toDb.From<SampleGroup>().Where("SampleGroupID =  @p1")
                .Query(sampleGroupID).FirstOrDefault();
            sampleGroupAgain.Should().BeEquivalentTo(newSampleGroup);
        }

        [Fact]
        public void Sync_SampleGroup_Update()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "SampleGroup_Update_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "SampleGroup_Update_toFile");

            var syncOptions = new CruiseSyncOptions();

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            // initialize source database
            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);
            var sampleGroupID = Guid.NewGuid().ToString();
            var sampleGroup = new SampleGroup()
            {
                CruiseID = cruiseID,
                SampleGroupID = sampleGroupID,
                SampleGroupCode = "10",
                StratumCode = Strata[0].StratumCode,
            };
            fromDb.Insert(sampleGroup);

            // initialize dest database 
            // as exact copy of source database
            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            // modify sample group value on source database
            sampleGroup.Description = Rand.String();
            fromDb.Update(sampleGroup);

            // run sync
            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var sampleGroupAgain = toDb.From<SampleGroup>().Where("SampleGroupID =  @p1")
                .Query(sampleGroupID).FirstOrDefault();
            sampleGroupAgain.Should().BeEquivalentTo(sampleGroup, x => x.Excluding(y => y.Modified_TS));
        }

        [Fact]
        public void Sync_SampleGroup_Update_SampleGroupCode()
        {
            var fromPath = GetTempFilePathWithExt(".crz3", "SampleGroup_Update_SGCode_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "SampleGroup_Update_SGCode_toFile");

            var syncOptions = new CruiseSyncOptions();

            var cruiseID = Guid.NewGuid().ToString();
            var saleID = Guid.NewGuid().ToString();

            // initialize source database
            using var fromDb = CreateDatabaseFile(fromPath, cruiseID, saleID);
            var sampleGroupID = Guid.NewGuid().ToString();
            var sampleGroup = new SampleGroup()
            {
                CruiseID = cruiseID,
                SampleGroupID = sampleGroupID,
                SampleGroupCode = "10",
                StratumCode = Strata[0].StratumCode,
            };
            fromDb.Insert(sampleGroup);

            // initialize dest database 
            // as exact copy of source database
            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            // modify sample group code on source database
            sampleGroup.SampleGroupCode = "11";
            fromDb.Update(sampleGroup);

            // run sync
            var syncer = new CruiseSyncer();
            syncer.Sync(cruiseID, fromDb, toDb, syncOptions);

            var sampleGroupAgain = toDb.From<SampleGroup>().Where("SampleGroupID =  @p1")
                .Query(sampleGroupID).FirstOrDefault();
            sampleGroupAgain.SampleGroupCode.Should().BeEquivalentTo(sampleGroup.SampleGroupCode);
        }
    }
}
