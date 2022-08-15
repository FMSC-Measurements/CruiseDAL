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
    public class UpdateTo_3_5_5_Test : TestBase
    {
        public UpdateTo_3_5_5_Test(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("something")]
        [InlineData(null)]
        public void UpdateFrom354_To_355_SaleDistrict(string districtValue)
        {
            var filePath = InitializeTestFile("3.5.4.crz3");

            using var ds = new CruiseDatastore(filePath);

            ds.Execute("UPDATE Sale SET District = @p1;", districtValue);

            var updateTo355 = new UpdateTo_3_5_5();

            using var conn = ds.OpenConnection();
            updateTo355.Invoking(x => x.Update(conn)).Should().NotThrow();

            var saleAfter = ds.From<Sale>().Query().Single();
            if (districtValue != null)
            {
                var expectedValue = districtValue.Substring(0, 2);
                saleAfter.District.Should().Be(expectedValue);
            }
            else
            {
                saleAfter.District.Should().BeNull();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("something", 0)]
        [InlineData("01", 1)]
        [InlineData(1, 1)]
        public void UpdateFrom354_To_355_SamplerState_InsuranceCounter(object icValue, int? expectedValue)
        {
            var filePath = InitializeTestFile("3.5.4.crz3");

            using var ds = new CruiseDatastore(filePath);

            var cruiseID = ds.ExecuteScalar<string>("SELECT CruiseID FROM Cruise LIMIT 1;");

            var device = new Device
            {
                CruiseID = cruiseID,
                DeviceID = Guid.NewGuid().ToString(),
                Name = "something",
            };
            ds.Insert(device);

            var ss = new SamplerState
            {
                CruiseID = cruiseID,
                DeviceID = device.DeviceID,
                StratumCode = "st1",
                SampleGroupCode = "sg1",
            };
            ds.Insert(ss);

            ds.Execute("UPDATE SamplerState SET InsuranceCounter = @p1;", icValue);

            var updateTo355 = new UpdateTo_3_5_5();

            using var conn = ds.OpenConnection();
            updateTo355.Invoking(x => x.Update(conn)).Should().NotThrow();

            var icAgain = ds.ExecuteScalar("SELECT InsuranceCounter FROM SamplerState;");
            
            if (expectedValue != null)
            {
                icAgain.Should().BeOfType<long>();
                icAgain.Should().Be(expectedValue); 
            }
            else
            {  icAgain.Should().Be(DBNull.Value);}
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("something", 0)]
        [InlineData("01", 1)]
        [InlineData(1, 1)]
        public void UpdateFrom354_To_355_SamplerState_InsuranceIndex(object iiValue, int? expectedValue)
        {
            var filePath = InitializeTestFile("3.5.4.crz3");

            using var ds = new CruiseDatastore(filePath);

            var cruiseID = ds.ExecuteScalar<string>("SELECT CruiseID FROM Cruise LIMIT 1;");

            var device = new Device
            {
                CruiseID = cruiseID,
                DeviceID = Guid.NewGuid().ToString(),
                Name = "something",
            };
            ds.Insert(device);

            var ss = new SamplerState
            {
                CruiseID = cruiseID,
                DeviceID = device.DeviceID,
                StratumCode = "st1",
                SampleGroupCode = "sg1",
            };
            ds.Insert(ss);

            ds.Execute("UPDATE SamplerState SET InsuranceIndex = @p1;", iiValue);

            var updateTo355 = new UpdateTo_3_5_5();

            using var conn = ds.OpenConnection();
            updateTo355.Invoking(x => x.Update(conn)).Should().NotThrow();

            var iiAgain = ds.ExecuteScalar("SELECT InsuranceIndex FROM SamplerState;");

            if (expectedValue != null)
            {
                iiAgain.Should().BeOfType<long>();
                iiAgain.Should().Be(expectedValue);
            }
            else
            { iiAgain.Should().Be(DBNull.Value); }
        }
    }
}
