using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync.Test.E2E
{
    public class Sync_E2E : TestBase
    {
        public Sync_E2E(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void CreateFromTemplate_To_Sync()
        {
            var templateCopier = new TemplateCopier();

            // initialize template

            var templatePath = GetTestFile("TestTemplate.crz3t");

            using var templateDb = new CruiseDatastore_V3(templatePath);
            var templateCruiseID = templateDb.From<Cruise>().Query().Single().CruiseID;

            var tarCount = templateDb.From<TreeAuditRule>().Count();
            var tarSelCount = templateDb.From<TreeAuditRuleSelector>().Count();

            // initialize first cruise

            var cruisePath1 = GetTempFilePath("cruise1.crz3");
            var cruise1Init = new DatabaseInitializer(false);
            using var cruiseDb1 = cruise1Init.CreateDatabaseFile(cruisePath1);
            var cruiseID1 = cruise1Init.CruiseID;

            templateCopier.Copy(templateDb, cruiseDb1, templateCruiseID, cruiseID1);

            var cruise1Tars = cruiseDb1.From<TreeAuditRule>().Query().ToArray();

            // initialize second cruise

            var cruisePath2 = GetTempFilePath("cruise2.crz3");
            var cruise2Init = new DatabaseInitializer(false);
            using var cruiseDb2 = cruise2Init.CreateDatabaseFile(cruisePath2);
            var cruiseID2 = cruise2Init.CruiseID;

            templateCopier.Copy(templateDb, cruiseDb2, templateCruiseID, cruiseID2);

            // create destination file and sync 

            var destDbPath = GetTempFilePath("destDb.crz3db");
            using var destDb = new CruiseDatastore_V3(destDbPath, true);

            var syncer = new CruiseDatabaseSyncer();
            var syncOptions = new TableSyncOptions();

            syncer.Sync(cruiseID1, cruiseDb1, destDb, syncOptions);

            destDb.From<TreeAuditRule>().Count().Should().Be(tarCount);
            destDb.From<TreeAuditRuleSelector>().Count().Should().Be(tarSelCount);

            syncer.Sync(cruiseID2, cruiseDb2, destDb, syncOptions);

            destDb.From<TreeAuditRule>().Count().Should().Be(tarCount * 2);
            destDb.From<TreeAuditRuleSelector>().Count().Should().Be(tarSelCount * 2);

            // export first cruise back out

            var cruiseCopier = new CruiseCopier();
            var cruise1ExportPath = GetTempFilePath("cruise1Export.crz3");
            using var cruise1ExportDb = new CruiseDatastore_V3(cruise1ExportPath, true);

            cruiseCopier.Copy(destDb, cruise1ExportDb, cruiseID1);

            cruise1ExportDb.From<TreeAuditRule>().Count().Should().Be(tarCount);
            cruise1ExportDb.From<TreeAuditRuleSelector>().Count().Should().Be(tarSelCount);

            // verify that tars from first cruise are the same
            var tarAgain = cruise1ExportDb.From<TreeAuditRule>().Query().ToArray();
            tarAgain.Should().BeEquivalentTo(cruise1Tars);

        }
    }
}
