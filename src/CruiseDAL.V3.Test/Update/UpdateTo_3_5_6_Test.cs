using CruiseDAL.TestCommon;
using CruiseDAL.Update;
using CruiseDAL.V3.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Test.Update
{
    public class UpdateTo_3_5_6_Test : TestBase
    {
        public UpdateTo_3_5_6_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("3.5.4.crz3")]
        [InlineData("44149_Little Nolan_TS_202307101240_GalaxyTabActive3-KT8D723.crz3")]
        public void UpdateFrom354(string fileName)
        {
            var filePath = InitializeTestFile(fileName);

            using var ds = new CruiseDatastore(filePath);


            var updateTo356 = new UpdateTo_3_5_6();

            using var conn = ds.OpenConnection();
            updateTo356.Update(conn);
        }

        [Theory]
        [InlineData("3.5.5.crz3")]
        [InlineData("3.5.6.crz3")]
        public void UpdateFrom355(string fileName)
        {
            var filePath = InitializeTestFile("3.5.5.crz3");

            using var ds = new CruiseDatastore(filePath);

            var updateTo356 = new UpdateTo_3_5_6();

            using var conn = ds.OpenConnection();
            updateTo356.Update(conn);
        }

        [Theory]
        [InlineData(null, false, null)]
        [InlineData("", false, "")]
        [InlineData(" ", false, "")]
        //[InlineData("\t", false, "")]
        //[InlineData("\r\n", false, "")]
        [InlineData("0.0", false, "0")]
        [InlineData("00", false, "0")]
        [InlineData("09", false, "9")]
        [InlineData(" 09 ", false, "9")]
        public void UpdateFrom354_UpdateLogGrades(string gradeValue, bool fails, object expected)
        {
            var filePath = InitializeTestFile("3.5.4.crz3");
            var init = new DatabaseInitializer();

            using var ds = new CruiseDatastore(filePath);
            DatabaseInitializer.InitializeDatabase(ds,
                init.CruiseID,
                init.SaleID,
                init.SaleNumber,
                init.Units,
                init.Strata,
                init.UnitStrata,
                init.SampleGroups,
                init.Species,
                init.TreeDefaults,
                init.Subpops);

            var tree = new Tree()
            {
                CruiseID = init.CruiseID,
                TreeID = Guid.NewGuid().ToString(),
                TreeNumber = 1,
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",
            };
            ds.Insert(tree);

            var log = new Log()
            {
                CruiseID = init.CruiseID,
                TreeID = tree.TreeID,
                LogID = Guid.NewGuid().ToString(),
                LogNumber = "1",
            };
            ds.Insert(log);

            ds.Execute("UPDATE Log SET Grade = @p1 WHERE LogID = @p2;", gradeValue, log.LogID);

            var updateTo356 = new UpdateTo_3_5_6();

            using var conn = ds.OpenConnection();

            if (!fails)
            {
                updateTo356.Update(conn);

                expected ??= DBNull.Value;

                var result = ds.ExecuteScalar("SELECT Grade FROM Log WHERE LogID = @p1;", log.LogID);
                result.Should().Be(expected); 
            }
            else
            {
                updateTo356.Invoking(x => x.Update(conn)).Should().Throw<FMSC.ORM.UpdateException>();
            }
        }

        [Theory]
        //[InlineData(null, false, null)]
        [InlineData(0, false, 0)]
        [InlineData(-1, true, null)]
        [InlineData(100, false, 100)]
        [InlineData(999, false, 100)]
        public void UpdateFrom354_UpdateTreeSeenDefect(int orgValue, bool fails, object expected)
        {
            var filePath = InitializeTestFile("3.5.4.crz3");
            var init = new DatabaseInitializer();

            using var ds = new CruiseDatastore(filePath);
            DatabaseInitializer.InitializeDatabase(ds,
                init.CruiseID,
                init.SaleID,
                init.SaleNumber,
                init.Units,
                init.Strata,
                init.UnitStrata,
                init.SampleGroups,
                init.Species,
                init.TreeDefaults,
                init.Subpops);

            var tree = new Tree()
            {
                CruiseID = init.CruiseID,
                TreeID = Guid.NewGuid().ToString(),
                TreeNumber = 1,
                CuttingUnitCode = "u1",
                StratumCode = "st1",
                SampleGroupCode = "sg1",
            };
            ds.Insert(tree);

            var treeMeasurment = new TreeMeasurment()
            {
                TreeID = tree.TreeID,
            };
            ds.Insert(treeMeasurment);

            ds.Execute("UPDATE TreeMeasurment SET SeenDefectPrimary = @p1 WHERE TreeID = @p2;", orgValue, tree.TreeID);

            var updateTo356 = new UpdateTo_3_5_6();

            using var conn = ds.OpenConnection();

            if (!fails)
            {
                updateTo356.Update(conn);

                expected ??= DBNull.Value;

                var result = ds.ExecuteScalar("SELECT SeenDefectPrimary FROM TreeMeasurment WHERE TreeID = @p1;", tree.TreeID);
                result.Should().Be(expected);
            }
            else
            {
                updateTo356.Invoking(x => x.Update(conn)).Should().Throw<FMSC.ORM.UpdateException>();
            }
        }
    }
}
