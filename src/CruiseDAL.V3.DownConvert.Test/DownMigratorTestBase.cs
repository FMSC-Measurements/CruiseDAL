using CruiseDAL.TestCommon;
using CruiseDAL.UpConvert;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CruiseDAL.V3.DownConvert.Test
{
    public class DownMigratorTestBase : TestBase
    {
        public DownMigratorTestBase(ITestOutputHelper output) : base(output)
        {
        }

        protected (string, string, string) SetUpTestFile(string fileName, [CallerMemberName] string caller = null)
        {
            var filePath = Path.Combine(TestFilesDirectory, fileName);

            var baseFileName = Path.GetFileName(fileName);
            var orgFile = Path.Combine(TestTempPath, fileName);
            var crz3File = (string)null;

            // create copy of base file
            if (File.Exists(orgFile) == false)
            {
                File.Copy(filePath, orgFile);
            }
            crz3File = new Migrator().MigrateFromV2ToV3(orgFile, true);


            var v2againPath = Path.Combine(TestTempPath, caller + "_again_" + fileName);
            using (var v2again = new DAL(v2againPath, true))
            using (var v3db = new CruiseDatastore_V3(crz3File))
            {
                var cruiseID = v3db.ExecuteScalar<string>("SELECT CruiseID FROM Cruise;");
                var downMigrator = new DownMigrator();
                downMigrator.MigrateFromV3ToV2(cruiseID, v3db, v2again);

            }

            return (orgFile, crz3File, v2againPath);
        }
    }
}
