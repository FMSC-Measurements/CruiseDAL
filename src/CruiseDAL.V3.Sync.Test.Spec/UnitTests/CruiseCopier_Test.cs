using CruiseDAL.TestCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;

namespace CruiseDAL.V3.Sync.Test.UnitTests
{
    public class CruiseCopier_Test : TestBase
    {
        public CruiseCopier_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Copy()
        {
            var initializer = new DatabaseInitializer();
            using (var srcDb = initializer.CreateDatabaseWithAllTables())
            using (var destDb = new CruiseDatastore_V3())
            {
                var copier = new CruiseCopier();
                copier.Copy(srcDb, destDb, initializer.CruiseID);
            }
        }

        [Fact]
        public void Issue_CantExportIfCruiseSaleIDDoesntMatch()
        {
            var fileName = "Issue_CantExportIfCruiseSaleIDDoesntMatch.crz3db";

            var filePath = GetTestFile(fileName);
            var outputPath = GetTempFilePath("Issue_CantExportIfCruiseSaleIDDoesntMatch_Output.crz3");

            var cruiseID = "feb09511-3b82-4dc6-a34d-eafc929bf8df";

            using var db = new CruiseDatastore_V3(filePath);
            using var destDb = new CruiseDatastore_V3(outputPath, true);

            var copier = new CruiseCopier();
            copier.Copy(db, destDb, cruiseID);

            //copier.Invoking(x => x.Copy(db, destDb, cruiseID)).Should().NotThrow();
        }

        
    }
}
