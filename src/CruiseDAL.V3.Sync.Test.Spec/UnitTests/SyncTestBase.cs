using CruiseDAL.TestCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync.Test.Spec.UnitTests
{
    public class SyncTestBase : TestBase
    {
        public SyncTestBase(ITestOutputHelper output) : base(output)
        {
            Syncer = new CruiseDatabaseSyncer();
            DeleteSyncer = new DeleteSysncer();
            ConflictChecker = new ConflictChecker();
            ConflictResolver = new ConflictResolver();
            Options = new TableSyncOptions();
        }

        public ConflictChecker ConflictChecker { get; protected set; }
        public ConflictResolver ConflictResolver { get; }
        public CruiseDatabaseSyncer Syncer { get; protected set; }
        public DeleteSysncer DeleteSyncer { get; }
        public TableSyncOptions Options { get; protected set; }

        protected void PrepareFile(CruiseDatastore_V3 dest, CruiseDatastore_V3 source, string cruiseID)
        {
            var preCheckSyncOptions = new TableSyncOptions(SyncOption.Update)
            {
                Processing = SyncOption.Lock,
                Template = SyncOption.Lock,
                SamplerState = SyncOption.Lock,
                TreeDataFlags = SyncOption.Lock,
                TreeDefaultValue = SyncOption.Lock,
                Validation = SyncOption.Lock,
            };
            Syncer.Sync(cruiseID, dest, source, preCheckSyncOptions);
        }
    }
}
