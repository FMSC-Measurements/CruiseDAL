using CruiseDAL.TestCommon;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync.Test.UnitTests.FileTests
{
    public class SyncStevesFiles : TestBase
    {
        public ConflictChecker ConflictChecker { get; protected set; }
        public ConflictResolver ConflictResolver { get; }
        public CruiseDatabaseSyncer Syncer { get; protected set; }
        public DeleteSysncer DeleteSyncer { get; }
        public TableSyncOptions Options { get; protected set; }

        public SyncStevesFiles(ITestOutputHelper output) : base(output)
        {
            Syncer = new CruiseDatabaseSyncer();
            DeleteSyncer = new DeleteSysncer();
            ConflictChecker = new ConflictChecker();
            ConflictResolver = new ConflictResolver();
            Options = new TableSyncOptions();
        }


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

        [Fact]
        public void SyncOneAndThree()
        {
            var destPath = GetTestFile("17901_Light_TS_202208040942_GalaxyTabS3-15CRZ Component 1.crz3");
            var sourcePath = GetTestFile("17901_Light_TS_202208040240_GalaxyTabS3-15CRZ Component 3.crz3");


            using var dest = new CruiseDatastore_V3(destPath);
            using var source = new CruiseDatastore_V3(sourcePath);

            Output.WriteLine(dest.DatabaseVersion);
            Output.WriteLine(source.DatabaseVersion);

            var cruiseID = dest.QueryScalar<string>("SELECT CruiseID FROM Cruise;").Single();

            PrepareFile(dest, source, cruiseID);


            using (var sourceConn = source.OpenConnection())
            using (var destConn = dest.OpenConnection())
            {
                var conflicts = ConflictChecker.CheckConflicts(sourceConn, destConn, cruiseID, new ConflictCheckOptions());

                conflicts.CuttingUnit.Should().HaveCount(1);
                conflicts.Tree.Should().HaveCount(1);

                var unitConflict = conflicts.CuttingUnit.Single();
                unitConflict.ConflictResolution = ConflictResolutionType.ChoseSource;

                var treeConflict = conflicts.Tree.Single();
                treeConflict.ConflictResolution = ConflictResolutionType.ChoseSource;

                ConflictResolver.ResolveConflicts(sourceConn, destConn, conflicts);


                conflicts = ConflictChecker.CheckConflicts(sourceConn, destConn, cruiseID, new ConflictCheckOptions());
                conflicts.HasConflicts.Should().BeFalse();

                Syncer.Sync(cruiseID, sourceConn, destConn, Options);

                conflicts = ConflictChecker.CheckConflicts(sourceConn, destConn, cruiseID, new ConflictCheckOptions());
                conflicts.HasConflicts.Should().BeFalse();
            }
        }
    }
}
