using Gherkin.CucumberMessages.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync.Test.UnitTests.FileTests
{
    public class Lost_Mule_Recon_Recon : SyncTestBase
    {
        public Lost_Mule_Recon_Recon(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void CheckFiles()
        {
            var destPath = GetTestFile("1_Lost Mule Recon_Recon_202306060338_GalaxyTabActive3-5J1C.crz3");
            var srcPath = GetTestFile("1_Lost Mule Recon_Recon_202306060339_GalaxyTabActive3-AAGP.crz3");

            using var dest = new CruiseDatastore_V3(destPath);
            using var source = new CruiseDatastore_V3(srcPath);

            Output.WriteLine(dest.DatabaseVersion);
            Output.WriteLine(source.DatabaseVersion);

            var cruiseID = dest.QueryScalar<string>("SELECT CruiseID FROM Cruise;").Single();

            PrepareFile(dest, source, cruiseID);

            using var sourceConn = source.OpenConnection();
            using var destConn = dest.OpenConnection();

            var conflicts = ConflictChecker.CheckConflicts(sourceConn, destConn, cruiseID, new ConflictCheckOptions());

            conflicts.Should().NotBeNull();
            conflicts.HasConflicts.Should().BeFalse();
        }
    }
}
