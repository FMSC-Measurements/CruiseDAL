using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using CruiseDAL.V3.Sync.Syncers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync.Test.Issues
{
    public class Issues_Test : TestBase
    {
        public Issues_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Issue_20240220_AuditRulesNotImporting()
        {
            CleanUpTestFiles = false;

            var cruisePath = GetTestFile("2024.02.20_AutitRulesNotImporting\\33 33 tuesday Recon - Recon.crz3");
            var crzdbPath = GetTestFile("2024.02.20_AutitRulesNotImporting\\CruiseDatabaseBackup_18032024.crz3db");


            using var cruiseDb = new CruiseDatastore_V3(cruisePath);
            var cruiseID = cruiseDb.From<Cruise>().Query().Single().CruiseID;

            using var fsCruiserDb = new CruiseDatastore_V3(crzdbPath);

            var syncOptions = new TableSyncOptions();
            var syncer = new CruiseDatabaseSyncer();
            syncer.Sync(cruiseID, cruiseDb, fsCruiserDb, syncOptions);


            var orgTARs = cruiseDb.From<TreeAuditRule>().Query().ToArray();

            var destTars = fsCruiserDb.From<TreeAuditRule>().Query().ToArray();

            var missingTars = orgTARs.Where(x => !destTars.Any(y => y.TreeAuditRuleID == x.TreeAuditRuleID && y.CruiseID == x.CruiseID))
                .ToArray();
            missingTars.Should().BeEmpty();
        }
    }
}
