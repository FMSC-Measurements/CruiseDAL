using CruiseDAL.TestCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync.Test.Spec.UnitTests
{
    public class SyncStevesFiles : TestBase
    {
        public ConflictChecker ConflictChecker { get; protected set; }
        public ConflictResolver ConflictResolver { get; }
        public CruiseSyncer Syncer { get; protected set; }
        public DeleteSysncer DeleteSyncer { get; }
        public CruiseSyncOptions Options { get; protected set; }

        public SyncStevesFiles(ITestOutputHelper output) : base(output)
        {
            Syncer = new CruiseSyncer();
            DeleteSyncer = new DeleteSysncer();
            ConflictChecker = new ConflictChecker();
            ConflictResolver = new ConflictResolver();
            Options = new CruiseSyncOptions();
        }


        protected void PrepareFile(CruiseDatastore_V3 dest, CruiseDatastore_V3 source, string cruiseID)
        {
            DeleteSyncer.Sync(cruiseID, source, dest, Options);
            DeleteSyncer.Sync(cruiseID, dest, source, Options);

            var preCheckSyncOptions = new CruiseSyncOptions
            {
                Design = SyncFlags.Update,
                FieldData = SyncFlags.Update,
                TreeFlags = SyncFlags.Update,
                Processing = SyncFlags.Lock,
                Template = SyncFlags.Lock,
                SamplerState = SyncFlags.Lock,
                TreeDataFlags = SyncFlags.Lock,
                TreeDefaultValue = SyncFlags.Lock,
                Validation = SyncFlags.Lock,
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

            var cruiseID = dest.QueryScalar<string>("SELECT CruiseID FROM Cruise;").Single();

            PrepareFile(dest, source, cruiseID);


            using (var sourceConn = source.OpenConnection())
            using (var destConn = dest.OpenConnection())
            {
                var conflicts = ConflictChecker.CheckConflicts(sourceConn, destConn, cruiseID);

                conflicts.CuttingUnit.Should().HaveCount(1);
                conflicts.Tree.Should().HaveCount(1);

                var unitConflict = conflicts.CuttingUnit.Single();
                unitConflict.ConflictResolution = ConflictResolutionType.ChoseSource;

                var treeConflict = conflicts.Tree.Single();
                treeConflict.ConflictResolution = ConflictResolutionType.ChoseSource;

                ConflictResolver.ResolveConflicts(sourceConn, destConn, conflicts);


                conflicts = ConflictChecker.CheckConflicts(sourceConn, destConn, cruiseID);
                conflicts.HasAny().Should().BeFalse();

                Syncer.Sync(cruiseID, sourceConn, destConn, Options);

                conflicts = ConflictChecker.CheckConflicts(sourceConn, destConn, cruiseID);
                conflicts.HasAny().Should().BeFalse();
            }
        }
    }
}
