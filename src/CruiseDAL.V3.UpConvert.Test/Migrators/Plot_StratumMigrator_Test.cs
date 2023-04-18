using CruiseDAL.TestCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.UpConvert.Test.Migrators
{
    public class Plot_StratumMigrator_Test : TestBase
    {
        public Plot_StratumMigrator_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Test_IsEmpty()
        {
            var v2Init = new TestCommon.V2.DatabaseInitializer_V2();


            var v2Path = GetTempFilePathWithExt(".cruise");
            using var v2db = v2Init.CreateDatabaseFile(v2Path);

            var plots = new[]
            {
                new V2.Models.Plot()
                {
                    CuttingUnit_CN = 1,
                    Stratum_CN = 1,
                    PlotNumber = 1,
                    IsEmpty = "True",
                },
                new V2.Models.Plot()
                {
                    CuttingUnit_CN = 1,
                    Stratum_CN = 1,
                    PlotNumber = 2,
                    IsEmpty = "False",
                },
                new V2.Models.Plot()
                {
                    CuttingUnit_CN = 1,
                    Stratum_CN = 1,
                    PlotNumber = 3,
                    IsEmpty = null,
                },
                new V2.Models.Plot()
                {
                    CuttingUnit_CN = 1,
                    Stratum_CN = 1,
                    PlotNumber = 4,
                    IsEmpty = "",
                },
            };

            foreach(var p in plots)
            {
                v2db.Insert(p);
            }


            var v3Path = GetTempFilePathWithExt(".crz3");
            using var v3db = new CruiseDatastore_V3(v3Path, true);

            var migrator = new CruiseDAL.UpConvert.Migrator();
            migrator.MigrateFromV2ToV3(v2db, v3db);



        }
    }
}
