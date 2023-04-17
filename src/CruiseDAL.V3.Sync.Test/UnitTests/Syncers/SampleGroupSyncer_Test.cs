using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using CruiseDAL.V3.Sync.Syncers;
using FMSC.ORM.Core;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync.Test.UnitTests.Syncers
{
    public class SampleGroupSyncer_Test : TestBase
    {
        public SampleGroupSyncer_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void SyncRecords_With_CruiseMethodChanges_Without_CruiseData()
        {
            var newFrequency = 10;

            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Add_toFile");

            var syncOptions = new TableSyncOptions();

            var init = new DatabaseInitializer();

            using var fromDb = init.CreateDatabaseFile(fromPath);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var sg = fromDb.From<SampleGroup>().Where("StratumCode = 'st1' AND SampleGroupCode = 'sg1';").Query().First();
            sg.SamplingFrequency.Should().NotBe(newFrequency);
            sg.SamplingFrequency = newFrequency;
            fromDb.Update(sg);

            var syncer = new SampleGroupSyncer();
            syncer.Invoking(x => x.SyncRecords(init.CruiseID, fromDb.OpenConnection(), toDb.OpenConnection(), syncOptions, (IExceptionProcessor)null))
                .Should().NotThrow<SampleGroupSettingMismatchException>();
        }

        [Fact]
        public void SyncRecords_With_CruiseMethodChanges_With_CruiseData()
        {
            var newFrequency = 10;

            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Add_toFile");

            var syncOptions = new TableSyncOptions();

            var init = new DatabaseInitializer();

            using var fromDb = init.CreateDatabaseFile(fromPath);

            var tallyLedger = new TallyLedger
            {
                CruiseID = init.CruiseID,
                TallyLedgerID = Guid.NewGuid().ToString(),
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",
                TreeCount = 0,
            };
            fromDb.Insert(tallyLedger);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var sg = fromDb.From<SampleGroup>().Where("StratumCode = 'st1' AND SampleGroupCode = 'sg1';").Query().First();
            sg.SamplingFrequency.Should().NotBe(newFrequency);
            sg.SamplingFrequency = newFrequency;
            fromDb.Update(sg);

            var syncer = new SampleGroupSyncer();
            syncer.Invoking(x => x.SyncRecords(init.CruiseID, fromDb.OpenConnection(), toDb.OpenConnection(), syncOptions, (IExceptionProcessor)null))
                .Should().Throw<SampleGroupSettingMismatchException>().And.Message.Should().Contain("Sample Group");
        }
    }
}