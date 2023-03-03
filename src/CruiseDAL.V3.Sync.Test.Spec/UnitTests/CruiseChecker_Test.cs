using CruiseDAL.TestCommon;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync.Test.UnitTests
{
    public class CruiseChecker_Test : Datastore_TestBase
    {
        public CruiseChecker_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public bool AnalizeCruise()
        {
            var srcFile = GetTempFilePathWithExt(".crz3", "HasDesignKeyChanges_srcFile");
            var destFile = GetTempFilePathWithExt(".crz3", "HasDesignKeyChanges_destFile");

            var cruiseID = CruiseID;
            using (var srcDb = CreateDatabaseFile(srcFile))
            using (var destDb = CreateDatabaseFile(destFile))
            {
                var cruiseChecker = new CruiseChecker();

                var cruiseConflicts = cruiseChecker.GetCruiseConflicts(srcDb, destDb, cruiseID);
                var isCruiseInConflict = cruiseConflicts.Any();

                var saleConflicts = cruiseChecker.GetSaleConflicts(srcDb, destDb, cruiseID);
                var isSaleInConflict = saleConflicts.Any();


                var plotConflicts = cruiseChecker.GetPlotConflicts(srcDb, destDb, cruiseID);
                var treeConflicts = cruiseChecker.GetTreeConflicts(srcDb, destDb, cruiseID);
                var logConflicts = cruiseChecker.GetLogConflicts(srcDb, destDb, cruiseID);
                var hasDesignKeyChanges = cruiseChecker.HasDesignKeyChanges(srcDb, destDb, cruiseID);

                return plotConflicts.Count() == 0
                    && treeConflicts.Count() == 0
                    && logConflicts.Count() == 0
                    && !hasDesignKeyChanges
                    && !isCruiseInConflict
                    && !isSaleInConflict;

            }

        }


        [Fact]
        public void HasDesignKeyChanges()
        {
            var srcFile = GetTempFilePathWithExt(".crz3", "HasDesignKeyChanges_srcFile");
            var destFile = GetTempFilePathWithExt(".crz3", "HasDesignKeyChanges_destFile");

            using (var srcDb = CreateDatabaseFile(srcFile))
            using (var desDb = CreateDatabaseFile(destFile))
            {
                var cc = new CruiseChecker();

                var result = cc.HasDesignKeyChanges(srcDb, desDb, CruiseID);

            }
        }

        [Fact]
        public void DiffCuttingUnitKeys()
        {
            var srcFile = GetTempFilePathWithExt(".crz3", "DiffCuttingUnitKeys_srcFile");
            var destFile = GetTempFilePathWithExt(".crz3", "DiffCuttingUnitKeys_destFile");

            using (var srcDb = CreateDatabaseFile(srcFile))
            using (var desDb = CreateDatabaseFile(destFile))
            {
                var cc = new CruiseChecker();

                var result = cc.DiffCuttingUnitKeys(srcDb, desDb, CruiseID);

            }
        }

        [Fact]
        public void DiffStratumKeys()
        {
            var srcFile = GetTempFilePathWithExt(".crz3", "DiffStratumKeys_srcFile");
            var destFile = GetTempFilePathWithExt(".crz3", "DiffStratumKeys_destFile");

            using (var srcDb = CreateDatabaseFile(srcFile))
            using (var desDb = CreateDatabaseFile(destFile))
            {
                var cc = new CruiseChecker();

                var result = cc.DiffStratumKeys(srcDb, desDb, CruiseID);

            }
        }

        [Fact]
        public void DiffSampleGroupKeys()
        {
            var srcFile = GetTempFilePathWithExt(".crz3", "DiffSampleGroupKeys_srcFile");
            var destFile = GetTempFilePathWithExt(".crz3", "DiffSampleGroupKeys_destFile");

            using (var srcDb = CreateDatabaseFile(srcFile))
            using (var desDb = CreateDatabaseFile(destFile))
            {
                var cc = new CruiseChecker();

                var result = cc.DiffSampleGroupKeys(srcDb, desDb, CruiseID);

            }
        }

        [Fact]
        public void DiffSubPopulationKeys()
        {
            var srcFile = GetTempFilePathWithExt(".crz3", "DiffSubPopulationKeys_srcFile");
            var destFile = GetTempFilePathWithExt(".crz3", "DiffSubPopulationKeys_destFile");

            using (var srcDb = CreateDatabaseFile(srcFile))
            using (var desDb = CreateDatabaseFile(destFile))
            {
                var cc = new CruiseChecker();

                var result = cc.DiffSubPopulationKeys(srcDb, desDb, CruiseID);

            }
        }

        [Fact]
        public void GetCruiseConflicts()
        {
            var srcFile = GetTempFilePathWithExt(".crz3", "GetCruiseConflicts_srcFile");
            var destFile = GetTempFilePathWithExt(".crz3", "GetCruiseConflicts_destFile");

            using (var srcDb = CreateDatabaseFile(srcFile))
            using (var desDb = CreateDatabaseFile(destFile))
            {
                var cc = new CruiseChecker();

                var result = cc.GetCruiseConflicts(srcDb, desDb, CruiseID);
                result.Should().BeEmpty();

            }
        }

        [Fact]
        public void GetSaleConflicts()
        {
            var srcFile = GetTempFilePathWithExt(".crz3", "GetSaleConflicts_srcFile");
            var destFile = GetTempFilePathWithExt(".crz3", "GetSaleConflicts_destFile");

            using (var srcDb = CreateDatabaseFile(srcFile))
            using (var desDb = CreateDatabaseFile(destFile))
            {
                var cc = new CruiseChecker();

                var result = cc.GetSaleConflicts(srcDb, desDb, CruiseID);
                result.Should().BeEmpty();

            }
        }


        [Fact]
        public void GetSaleConflicts_dupSale()
        {
            var srcFile = GetTempFilePathWithExt(".crz3", "GetSaleConflicts_srcFile");
            var destFile = GetTempFilePathWithExt(".crz3", "GetSaleConflicts_destFile");

            var saleNumber = "12345";
            var srcInitialaizer = new DatabaseInitializer() { SaleNumber = saleNumber };
            var destInitializer = new DatabaseInitializer() { SaleNumber = saleNumber };
            using (var srcDb = srcInitialaizer.CreateDatabaseFile(srcFile))
            using (var desDb = destInitializer.CreateDatabaseFile(destFile))
            {
                var cc = new CruiseChecker();

                var result = cc.GetSaleConflicts(srcDb, desDb, srcInitialaizer.CruiseID);
                result.Should().NotBeEmpty();

            }
        }

        [Fact]
        public void GetLogConflicts()
        {
            var srcFile = GetTempFilePathWithExt(".crz3", "GetLogConflicts_srcFile");
            var destFile = GetTempFilePathWithExt(".crz3", "GetLogConflicts_destFile");

            using (var srcDb = CreateDatabaseFile(srcFile))
            using (var desDb = CreateDatabaseFile(destFile))
            {
                var cc = new CruiseChecker();

                var result = cc.GetLogConflicts(srcDb, desDb, CruiseID);
                result.Should().BeEmpty();

            }
        }

        [Fact]
        public void GetPlotConflicts()
        {
            var srcFile = GetTempFilePathWithExt(".crz3", "GetPlotConflicts_srcFile");
            var destFile = GetTempFilePathWithExt(".crz3", "GetPlotConflicts_destFile");

            using (var srcDb = CreateDatabaseFile(srcFile))
            using (var desDb = CreateDatabaseFile(destFile))
            {
                var cc = new CruiseChecker();

                var result = cc.GetPlotConflicts(srcDb, desDb, CruiseID);
                result.Should().BeEmpty();

            }
        }

        [Fact]
        public void GetTreeConflicts()
        {
            var srcFile = GetTempFilePathWithExt(".crz3", "GetTreeConflicts_srcFile");
            var destFile = GetTempFilePathWithExt(".crz3", "GetTreeConflicts_destFile");

            using (var srcDb = CreateDatabaseFile(srcFile))
            using (var desDb = CreateDatabaseFile(destFile))
            {
                var cc = new CruiseChecker();

                var result = cc.GetTreeConflicts(srcDb, desDb, CruiseID);
                result.Should().BeEmpty();

            }
        }


    }
}
