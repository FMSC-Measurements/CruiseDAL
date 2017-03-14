using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using CruiseDAL;
using CruiseDAL.DataObjects;
using CruiseDAL.Schema;

namespace FMSCORM.Tests
{
    public class DALTest
    {
        private readonly ITestOutputHelper _output;

        public static DAL _testDALInstance;

        public DALTest(ITestOutputHelper output)
        {
            _output = output;

            _testDALInstance = new DAL(".\\TestResources\\Test.cruise");
            Assert.True(_testDALInstance.Exists);
        }

        [Fact]
        public void TestAllowMultDALOnSameThread()
        {
            try
            {
                var dal1 = new DAL(".\\TestResources\\Test.cruise");
                var dal2 = new DAL(".\\TestResources\\Test.cruise");
            }
            catch
            {
                Assert.True(false);
            }
        }

        void DoWorkBlockMultiThreadDALFileAcess()
        {
            DAL newDAL = new DAL(".\\TestResources\\Test.cruise");
            _output.WriteLine("waiting");
            Thread.Sleep(2000);
        }

        [Fact]
        public void BlockMultiThreadDALFileAcess()
        {
            Thread thread = new Thread(DoWorkBlockMultiThreadDALFileAcess);
            thread.Start();
            _output.WriteLine("start");
            try
            {
                var dal = new DAL(".\\TestResources\\Test.cruise");
                Assert.True(false);
            }
            catch (Exception e)
            {
                _output.WriteLine(e.ToString());
                return;
            }
        }

        public void DatabaseUpdateTest()
        {
            throw new NotImplementedException();
        }

        public void TestCopyTo()
        {
            throw new NotImplementedException();
        }
    }
}