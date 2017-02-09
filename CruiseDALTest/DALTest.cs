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
        public static string TEST_COPY_FILE_NAME = "\\TestCopy.cruise";
        public static string TESTCRUISEDAL_PATH = "TestCruiseDAL.db";

        private readonly ITestOutputHelper _output;

        public static DAL _testDALInstance;

        public DALTest(ITestOutputHelper output)
        {
            _output = output;

            _testDALInstance = new DAL("Test.cruise");
            Assert.True(_testDALInstance.Exists);
        }


        public void TestAllowMultDALOnSameThread()
        {
            try
            {
                var dal1 = new DAL("FileAccessTest.cruise");
                var dal2 = new DAL("FileAccessTest.cruise");
            }
            catch
            {
                Assert.True(false);
            }
        }

        void DoWorkBlockMultiThreadDALFileAcess()
        {
            DAL newDAL = new DAL("FileAccessTest.cruise");
            _output.WriteLine("waiting");
            Thread.Sleep(2000);
        }


        public void BlockMultiThreadDALFileAcess()
        {

            Thread thread = new Thread(DoWorkBlockMultiThreadDALFileAcess);
            thread.Start();
            _output.WriteLine("start");
            try
            {
                var dal = new DAL("FileAccessTest.cruise");
                Assert.True(false);
            }
            catch( Exception e)
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
