using CruiseDAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using CruiseDAL.DataObjects;
using System.Threading;

namespace CruiseDALTest
{
    
    
    /// <summary>
    ///This is a test class for DALTest and is intended
    ///to contain all DALTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DALTest
    {
        public static string TEST_FILE_PATH = "Test.cruise";
        public static string TEST_COPY_FILE_NAME = "\\TestCopy.cruise";
        public static string TESTCRUISEDAL_PATH = "TestCruiseDAL.db";

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class

        public static DAL _testDALInstance;
        //public static Random _rand;

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            _testDALInstance = GetTestDAL();
            Assert.IsTrue(_testDALInstance.Exists);
            //_rand = new Random(0);

            //_testDALInstance = MakeDB("Test.cruise");

            //for (int i = 0; i < 10; i++)
            //{
            //    CuttingUnitDO newCU = new CuttingUnitDO(_testDALInstance);
            //    newCU.Code = RandomString(10);
            //    newCU.Area = (float)(1000 * _rand.NextDouble());
            //    newCU.Save();
            //}
        }

        public static DAL GetTestDAL()
        {
            return new DAL(TEST_FILE_PATH);
        }

        //private static string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        //private static string RandomString(int size)
        //{
        //    char[] buffer = new char[size];

        //    for (int i = 0; i < size; i++)
        //    {
        //        buffer[i] = _chars[_rand.Next(_chars.Length)];
        //    }
        //    return new string(buffer);
        //}


        //private static DAL MakeDB(string fileName)
        //{
        //    string path = fileName;
        //    DAL target = new DAL(path);
        //    IAsyncResult result;
        //    result = target.BeginCreate(null);
        //    Assert.IsNotNull(result);
        //    target.EndCreate(result);
        //    return target;
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod()]
        public void TestAllowMultDALOnSameThread()
        {
            try
            {
                var dal1 = new DAL("FileAccessTest.cruise");
                var dal2 = new DAL("FileAccessTest.cruise");
            }
            catch
            {
                Assert.Fail();
            }
        }

        void DoWorkBlockMultiThreadDALFileAcess()
        {
            DAL newDAL = new DAL("FileAccessTest.cruise");
            TestContext.WriteLine("waiting");
            Thread.Sleep(2000);
        }

        [TestMethod()]
        public void BlockMultiThreadDALFileAcess()
        {

            Thread thread = new Thread(DoWorkBlockMultiThreadDALFileAcess);
            thread.Start();
            TestContext.WriteLine("start");
            try
            {
                var dal = new DAL("FileAccessTest.cruise");
                Assert.Fail();
            }
            catch( Exception e)
            {

                TestContext.WriteLine(e.ToString());
                return;
            }

            
            
        }

        [TestMethod()]
        public void DatabaseUpdateTest()
        {
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void DataObjectToStringTest()
        {
            DataObject obj;
            obj = _testDALInstance.ReadSingleRow<SaleDO>(CruiseDAL.Schema.SALE._NAME, 1);
            CheckRow(obj);
            string s = obj.ToString("[District] hi mom ://\\ [rowID]", null);
            s = obj.ToString("[district] [District] [SaleNumber]", null);

            try
            {
                s = obj.ToString(" [badPropName] ", null);
                Assert.Fail("toString didn't fail with a bad Property name");
            }
            catch
            {
            }


        }



        [TestMethod()]
        public void ReadSingleRowTest()
        {
            DataObject obj;
            obj = _testDALInstance.ReadSingleRow<SaleDO>(CruiseDAL.Schema.SALE._NAME, 1);
            CheckRow(obj);

            SaleDO sale = obj as SaleDO;
            Assert.IsNotNull(sale);
            obj = _testDALInstance.ReadSingleRow<SaleDO>("Sale", "WHERE SaleNumber = ?", sale.SaleNumber);
            Assert.IsTrue(Assert.ReferenceEquals(sale, obj));


            obj = _testDALInstance.ReadSingleRow<CuttingUnitDO>(CruiseDAL.Schema.CUTTINGUNIT._NAME, 1);
            CheckRow(obj);

            obj = _testDALInstance.ReadSingleRow<StratumDO>(CruiseDAL.Schema.STRATUM._NAME, 1);
            CheckRow(obj);

            obj = _testDALInstance.ReadSingleRow<SampleGroupDO>(CruiseDAL.Schema.SAMPLEGROUP._NAME, 1);
            CheckRow(obj);

            obj = _testDALInstance.ReadSingleRow<TreeDefaultValueDO>(CruiseDAL.Schema.TREEDEFAULTVALUE._NAME, 1);
            //Assert.IsNotNull(((TreeDefaultValueDO)obj).);
            CheckRow(obj);

            obj = _testDALInstance.ReadSingleRow<TreeDO>(CruiseDAL.Schema.TREE._NAME, 1);
            CheckRow(obj);
        }

        private void CheckRow(DataObject obj)
        {
            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.DAL);
            Assert.IsNotNull(obj.rowID);
            Assert.IsTrue(obj.IsPersisted);
        }


        [TestMethod()]
        public void WriteTest()
        {
            DAL dal = new DAL("WriteTest.cruise");
            dal.Create();


            SaleDO sale = new SaleDO(dal);
            sale.SaleNumber = "12345";
            sale.Region = "01";
            sale.Forest = "02";
            sale.Save();

            SaleDO saleRead = dal.ReadSingleRow<SaleDO>("Sale", "WHERE SaleNumber = '12345'");
            //Assert.IsTrue(object.ReferenceEquals(sale, saleRead));
            long saleID = sale.GetID();
            long saleReadID = saleRead.GetID();
            Assert.IsTrue(saleReadID == saleID);
            //Assert.IsNotNull(saleRead.CreatedDate);
            //DateTime time;
            //DateTime.TryParse(saleRead.CreatedDate, out time);
            //Assert.IsTrue(DateTime.Compare(time.Date, DateTime.Today) == 0);

            sale.SaleNumber = "54321";
            sale.Save();

            saleRead = dal.ReadSingleRow<SaleDO>("Sale", "WHERE SaleNumber = '54321'");
            //Assert.IsTrue(object.ReferenceEquals(sale, saleRead));
            //Assert.IsNotNull(saleRead.ModifiedDate);
            //DateTime.TryParse(saleRead.ModifiedDate, out time);
            //Assert.IsTrue(DateTime.Compare(time.Date, DateTime.Today) == 0);
        }


        /// <summary>
        ///A test for Read
        [TestMethod()]
        public void TestRead()
        {
            DAL db = new DAL(TESTCRUISEDAL_PATH);

            List<TestReadDO> list = db.Read<TestReadDO>("TestRead", null);

        }


        /// <summary>
        ///A test for GetRowCount
        ///</summary>
        [TestMethod()]
        public void GetRowCountTest()
        {
            DAL target = _testDALInstance;
            string tableName = "CuttingUnit";
            string selection = null;
            string[] selectionArgs = null;
            Int64 expected = 10;
            Int64 actual;
            actual = target.GetRowCount(tableName, selection, selectionArgs);
            Assert.AreEqual(expected, actual);
        }



        /// <summary>
        ///A test for BeginCreate
        ///</summary>
        [TestMethod()]
        public void BeginCreateTest()
        {
            string path = "TestFile.cruise";
            DAL target = new DAL(path, true);
            AsyncCallback callbackFunct = new AsyncCallback(EndBeginCreateTimer);
            IAsyncResult result;
            TestContext.BeginTimer("BeginCreateTest");
            result = target.BeginCreate(callbackFunct);
            Assert.IsNotNull(result);
            target.EndCreate(result);
            TestContext.EndTimer("BeginCreateTest");
        }

        [TestMethod()]
        public void FailWhenNoConnection()
        {

            string path = "something.cruise";
            DAL target = new DAL(path);
            TreeDO t = new TreeDO(target);
            Exception ex = null;
            try
            {
                t.Save();
            }
            catch (FileAccessException e)
            {
                ex = e;
            }
            Assert.IsTrue(ex != null, "Save should fail when dal trys to access a file that hasn't been created");
        }

        [TestMethod()]
        public void TestCopyTo()
        {
            DAL target = GetTestDAL();
            string copyPath = System.IO.Path.GetDirectoryName(target.Path) + TEST_COPY_FILE_NAME;
            DAL copy = target.CopyTo(copyPath);

            string alias = "copy";
            target.AttachDB(copy, alias);
            try
            {
                //insure that schema of two files match 
                int rowCnt = (int)target.Execute("SELECT * FROM main.sqlite_master EXCEPT SELECT * FROM copy.sqlite_master;");
                Assert.IsTrue(rowCnt == 0);
            }
            finally
            {
                target.DetachDB(alias);
            }



        }


        private void EndBeginCreateTimer(IAsyncResult result)
        {
            this.TestContext.EndTimer("BeginCreateTest");
        }

        /// <summary>
        ///A test for DAL Constructor
        ///</summary>
        //[TestMethod()]
        //public void DALConstructorTest()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path);
        //    Assert.Inconclusive("TODO: Implement code to verify target");
        //}
    }
}
