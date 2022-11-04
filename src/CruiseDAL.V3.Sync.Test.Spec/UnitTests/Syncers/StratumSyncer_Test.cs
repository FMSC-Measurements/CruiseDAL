using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using CruiseDAL.V3.Sync.Syncers;
using FMSC.ORM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync.Test.Spec.UnitTests.Syncers
{
    public class StratumSyncer_Test : TestBase
    {
        public StratumSyncer_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void SyncRecords_With_CruiseMethodChanges_Without_CruiseData()
        {
            var newMethod = "FIX";

            var fromPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Add_fromFile");
            var toPath = GetTempFilePathWithExt(".crz3", "Sync_Plot_Add_toFile");

            var syncOptions = new TableSyncOptions();

            var init = new DatabaseInitializer();

            using var fromDb = init.CreateDatabaseFile(fromPath);

            fromDb.CopyTo(toPath, true);
            using var toDb = new CruiseDatastore_V3(toPath);

            var st = fromDb.From<Stratum>().Where("StratumCode = 'st1'").Query().First();
            st.Method.Should().NotBe(newMethod);
            st.Method = newMethod;
            fromDb.Update(st);

            var syncer = new StratumSyncer();
            syncer.Invoking(x => x.SyncRecords(init.CruiseID, fromDb.OpenConnection(), toDb.OpenConnection(), syncOptions, (IExceptionProcessor)null))
                .Should().NotThrow<StratumSettingMismatchException>();
        }

        [Fact]
        public void SyncRecords_With_CruiseMethodChanges_With_CruiseData()
        {
            var newMethod = "FIX";

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

            var st = fromDb.From<Stratum>().Where("StratumCode = 'st1'").Query().First();
            st.Method.Should().NotBe(newMethod);
            st.Method = newMethod;
            fromDb.Update(st);

            var syncer = new StratumSyncer();
            syncer.Invoking(x => x.SyncRecords(init.CruiseID, fromDb.OpenConnection(), toDb.OpenConnection(), syncOptions, (IExceptionProcessor)null))
                .Should().Throw<StratumSettingMismatchException>();
        }
    }
}
