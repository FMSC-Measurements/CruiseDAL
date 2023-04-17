using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync.Test.UnitTests.FileTests
{
    public class Sync_12345_Girard : SyncTestBase
    {
        public Sync_12345_Girard(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public void SyncAll()
        {
            var destPath = GetTestFile("12345_Girard Combine Test_Check_202211081037_QualityControlGroupDxD-12KPE.crz3");
            using var dest = new CruiseDatastore_V3(destPath);
            Output.WriteLine("Dest: " + destPath);

            var cruiseID = dest.QueryScalar<string>("SELECT CruiseID FROM Cruise;").Single();

            using var destConn = dest.OpenConnection();

            // sync DxDQCG
            var sourcePath = GetTestFile("12345_Girard Combine Test_Check_202211100145_DxDQCG-MUQ.crz3");
            Output.WriteLine("Src: " + sourcePath);
            using var source = new CruiseDatastore_V3(sourcePath);

            PrepareFile(dest, source, cruiseID);
            using var sourceConn = source.OpenConnection();

            // check that file has conflict we expect
            var conflicts = ConflictChecker.CheckConflicts(sourceConn, destConn, cruiseID, new ConflictCheckOptions());
            conflicts.HasConflicts.Should().BeTrue();
            conflicts.Plot.Count().Should().Be(1);

            var plotConflict = conflicts.Plot.Single();
            var srcPlot = plotConflict.SourceRec.As<Plot>();
            srcPlot.PlotNumber.Should().Be(196);
            srcPlot.CuttingUnitCode.Should().Be("566");

            // resolve conflict and verify it has been resolved
            plotConflict.ConflictResolution = ConflictResolutionType.ChoseDest;
            ConflictResolver.ResolveConflicts(sourceConn, destConn, conflicts);

            var conflictsAgain = ConflictChecker.CheckConflicts(sourceConn, destConn, cruiseID, new ConflictCheckOptions());
            conflictsAgain.HasConflicts.Should().BeFalse();

            //DeleteSyncer.Sync(cruiseID, sourceConn, destConn, Options);
            Syncer.Sync(cruiseID, sourceConn, destConn, Options);

            // Ricksgalaxytablet
            var sourcePath2 = GetTestFile("12345_Girard Combine Test_Check_202211100222_Ricksgalaxytablet-E7ED.crz3");
            Output.WriteLine("Src2: " + sourcePath2);
            using var source2 = new CruiseDatastore_V3(sourcePath2);

            PrepareFile(dest, source2, cruiseID);
            using var sourceConn2 = source2.OpenConnection();

            // check that file has conflict we expect
            var conflicts2 = ConflictChecker.CheckConflicts(sourceConn2, destConn, cruiseID, new ConflictCheckOptions());
            conflicts2.HasConflicts.Should().BeTrue();

            var plotConflict2 = conflicts2.Plot.Single();
            plotConflict2.ConflictResolution = ConflictResolutionType.ChoseDest;
            ConflictResolver.ResolveConflicts(sourceConn2, destConn, conflicts2);

            //DeleteSyncer.Sync(cruiseID, sourceConn2, destConn, Options);
            Syncer.Sync(cruiseID, sourceConn2, destConn, Options);
        }

        [Fact]
        public void Sync_QualityControlGroup_And_DxDQCG()
        {
            var destPath = GetTestFile("12345_Girard Combine Test_Check_202211081037_QualityControlGroupDxD-12KPE.crz3");
            var sourcePath = GetTestFile("12345_Girard Combine Test_Check_202211100145_DxDQCG-MUQ.crz3");

            using var dest = new CruiseDatastore_V3(destPath);
            using var source = new CruiseDatastore_V3(sourcePath);

            Output.WriteLine("Dest: " + destPath);
            Output.WriteLine("Src: " + sourcePath);

            var cruiseID = dest.QueryScalar<string>("SELECT CruiseID FROM Cruise;").Single();

            PrepareFile(dest, source, cruiseID);

            using var sourceConn = source.OpenConnection();
            using var destConn = dest.OpenConnection();

            // check that file has conflict we expect
            var conflicts = ConflictChecker.CheckConflicts(sourceConn, destConn, cruiseID, new ConflictCheckOptions());
            conflicts.HasConflicts.Should().BeTrue();
            conflicts.Plot.Count().Should().Be(1);

            var plotConflict = conflicts.Plot.Single();
            var srcPlot = plotConflict.SourceRec.As<Plot>();
            srcPlot.PlotNumber.Should().Be(196);
            srcPlot.CuttingUnitCode.Should().Be("566");

            // resolve conflict and verify it has been resolved
            plotConflict.ConflictResolution = ConflictResolutionType.ChoseDest;
            ConflictResolver.ResolveConflicts(sourceConn, destConn, conflicts);

            var conflictsAgain = ConflictChecker.CheckConflicts(sourceConn, destConn, cruiseID, new ConflictCheckOptions());
            conflictsAgain.HasConflicts.Should().BeFalse();

            DeleteSyncer.Sync(cruiseID, sourceConn, destConn, Options);
            Syncer.Sync(cruiseID, sourceConn, destConn, Options);
        }

        [Fact]
        public void Check_QualityControlGroup_And_DxDQCG()
        {
            var destPath = GetTestFile("12345_Girard Combine Test_Check_202211081037_QualityControlGroupDxD-12KPE.crz3");
            var sourcePath = GetTestFile("12345_Girard Combine Test_Check_202211100145_DxDQCG-MUQ.crz3");

            using var dest = new CruiseDatastore_V3(destPath);
            using var source = new CruiseDatastore_V3(sourcePath);

            Output.WriteLine(dest.DatabaseVersion);
            Output.WriteLine(source.DatabaseVersion);

            var cruiseID = dest.QueryScalar<string>("SELECT CruiseID FROM Cruise;").Single();

            PrepareFile(dest, source, cruiseID);

            using var sourceConn = source.OpenConnection();
            using var destConn = dest.OpenConnection();

            var conflicts = ConflictChecker.CheckConflicts(sourceConn, destConn, cruiseID, new ConflictCheckOptions());

            conflicts.HasConflicts.Should().BeTrue();

            conflicts.Plot.Count().Should().Be(1);

            var plotConflict = conflicts.Plot.Single();
            var srcPlot = plotConflict.SourceRec.As<Plot>();
            srcPlot.PlotNumber.Should().Be(196);
            srcPlot.CuttingUnitCode.Should().Be("566");
        }

        [Fact]
        public void Check_QualityControlGroup_And_Ricksgalaxytablet()
        {
            var destPath = GetTestFile("12345_Girard Combine Test_Check_202211081037_QualityControlGroupDxD-12KPE.crz3");
            var sourcePath = GetTestFile("12345_Girard Combine Test_Check_202211100222_Ricksgalaxytablet-E7ED.crz3");

            using var dest = new CruiseDatastore_V3(destPath);
            using var source = new CruiseDatastore_V3(sourcePath);

            Output.WriteLine(dest.DatabaseVersion);
            Output.WriteLine(source.DatabaseVersion);

            var cruiseID = dest.QueryScalar<string>("SELECT CruiseID FROM Cruise;").Single();

            PrepareFile(dest, source, cruiseID);



            using var sourceConn = source.OpenConnection();
            using var destConn = dest.OpenConnection();

            var conflicts = ConflictChecker.CheckConflicts(sourceConn, destConn, cruiseID, new ConflictCheckOptions());

            conflicts.HasConflicts.Should().BeFalse();
        }

        [Fact]
        public void Check_Ricksgalaxytablet_And_DxDQCG()
        {
            var destPath = GetTestFile("12345_Girard Combine Test_Check_202211100222_Ricksgalaxytablet-E7ED.crz3");
            var sourcePath = GetTestFile("12345_Girard Combine Test_Check_202211100145_DxDQCG-MUQ.crz3");

            using var dest = new CruiseDatastore_V3(destPath);
            using var source = new CruiseDatastore_V3(sourcePath);

            Output.WriteLine(dest.DatabaseVersion);
            Output.WriteLine(source.DatabaseVersion);

            var cruiseID = dest.QueryScalar<string>("SELECT CruiseID FROM Cruise;").Single();

            PrepareFile(dest, source, cruiseID);

            using var sourceConn = source.OpenConnection();
            using var destConn = dest.OpenConnection();

            var conflicts = ConflictChecker.CheckConflicts(sourceConn, destConn, cruiseID, new ConflictCheckOptions());

            conflicts.Plot.Count().Should().Be(1);

            var plotConflict = conflicts.Plot.Single();
            var srcPlot = plotConflict.SourceRec.As<Plot>();
            srcPlot.PlotNumber.Should().Be(17);
            srcPlot.CuttingUnitCode.Should().Be("978");
        }

    }
}
