using FMSCORM;
using System;
using System.Collections.Generic;
using FMSCORM.DataObjects;
using System.Threading;
using System.IO;
using FMSCORM.Test.TestTypes;
using Xunit;
using Xunit.Abstractions;

namespace FMSCORM.Tests
{

    public class DALTest
    {
        public static string TEST_COPY_FILE_NAME = "\\TestCopy.cruise";
        public static string TESTCRUISEDAL_PATH = "TestCruiseDAL.db";

        private readonly ITestOutputHelper _output;

        public static DAL _testDALInstance;
        //public static Random _rand;

        public DALTest(ITestOutputHelper output)
        {
            _output = output;

            _testDALInstance = FMSCORM.Tests.CommonMethods.GetTestDAL();
            Assert.True(_testDALInstance.Exists);
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


        public void DataObjectToStringTest()
        {
            DataObject obj;
            obj = _testDALInstance.ReadSingleRow<SaleDO>(FMSCORM.Schema.SALE._NAME, 1);
            CheckRow(obj);
            string s = obj.ToString("[District] hi mom ://\\ [rowID]", null);
            s = obj.ToString("[district] [District] [SaleNumber]", null);

            try
            {
                s = obj.ToString(" [badPropName] ", null);
                Assert.True(false,"toString didn't fail with a bad Property name");
            }
            catch
            {
            }


        }


        public void GetTableUniquesTest()
        {
            String[] result = _testDALInstance.GetTableUniques("tree");
            Assert.NotNull(result);
        }


        public void ReadSingleRowTest()
        {
            DataObject obj;
            obj = _testDALInstance.ReadSingleRow<SaleDO>(FMSCORM.Schema.SALE._NAME, 1);
            CheckRow(obj);

            SaleDO sale = obj as SaleDO;
            Assert.NotNull(sale);
            obj = _testDALInstance.ReadSingleRow<SaleDO>("Sale", "WHERE SaleNumber = ?", sale.SaleNumber);
            Assert.True(Object.ReferenceEquals(sale, obj));


            obj = _testDALInstance.ReadSingleRow<CuttingUnitDO>(FMSCORM.Schema.CUTTINGUNIT._NAME, 1);
            CheckRow(obj);

            obj = _testDALInstance.ReadSingleRow<StratumDO>(FMSCORM.Schema.STRATUM._NAME, 1);
            CheckRow(obj);

            obj = _testDALInstance.ReadSingleRow<SampleGroupDO>(FMSCORM.Schema.SAMPLEGROUP._NAME, 1);
            CheckRow(obj);

            obj = _testDALInstance.ReadSingleRow<TreeDefaultValueDO>(FMSCORM.Schema.TREEDEFAULTVALUE._NAME, 1);
            //Assert.IsNotNull(((TreeDefaultValueDO)obj).);
            CheckRow(obj);

            obj = _testDALInstance.ReadSingleRow<TreeDO>(FMSCORM.Schema.TREE._NAME, 1);
            CheckRow(obj);
        }

        private void CheckRow(DataObject obj)
        {
            Assert.NotNull(obj);
            Assert.NotNull(obj.DAL);
            Assert.NotNull(obj.rowID);
            Assert.True(obj.IsPersisted);
        }


        public void WriteTest()
        {
            DAL dal = new DAL("WriteTest.cruise", true);
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
            Assert.True(saleReadID == saleID);
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
        public void TestRead()
        {
            DAL db = new DAL(TESTCRUISEDAL_PATH);

            List<TestReadDO> list = db.Read<TestReadDO>("TestRead", null);

        }


        /// <summary>
        ///A test for GetRowCount
        ///</summary>
        public void GetRowCountTest()
        {
            DAL target = _testDALInstance;
            string tableName = "CuttingUnit";
            string selection = null;
            string[] selectionArgs = null;
            Int64 expected = 9;
            Int64 actual;
            actual = target.GetRowCount(tableName, selection, selectionArgs);
            Assert.Equal(expected, actual);
        }

        public void FailWhenNoConnection()
        {

            string path = "doesNotExist.cruise";
            
            Exception ex = null;
            try
            {
                DAL target = new DAL(path);
                TreeDO t = new TreeDO(target);
                t.Save();
            }
            catch (FileNotFoundException e)
            {
                ex = e;
            }
            Assert.True(ex != null, "Save should fail when dal trys to access a file that hasn't been created");
        }

        public void TestCopyTo()
        {
            DAL target = FMSCORM.Tests.CommonMethods.GetTestDAL();
            string copyPath = System.IO.Path.GetDirectoryName(target.Path) + TEST_COPY_FILE_NAME;
            DAL copy = target.CopyTo(copyPath);

            string alias = "copy";
            target.AttachDB(copy, alias);
            try
            {
                //insure that schema of two files match 
                int rowCnt = (int)target.Execute("SELECT * FROM main.sqlite_master EXCEPT SELECT * FROM copy.sqlite_master;");
                Assert.True(rowCnt == 0);
            }
            finally
            {
                target.DetachDB(alias);
            }



        }


        //private void EndBeginCreateTimer(IAsyncResult result)
        //{
        //    this.TestContext.EndTimer("BeginCreateTest");
        //}

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

        ///// <summary>
        /////A test for Path
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void PathTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    string expected = string.Empty; // TODO: Initialize to an appropriate value
        //    string actual;
        //    target.Path = expected;
        //    actual = target.Path;
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for Extension
        /////</summary>
        //[TestMethod()]
        //public void ExtensionTest()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.Extension;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for Exists
        /////</summary>
        //[TestMethod()]
        //public void ExistsTest()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    bool actual;
        //    actual = target.Exists;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for WriteTableDumpRowValues
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void WriteTableDumpRowValuesTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    TextWriter writer = null; // TODO: Initialize to an appropriate value
        //    DbDataReader reader = null; // TODO: Initialize to an appropriate value
        //    target.WriteTableDumpRowValues(writer, reader);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for ThrowDatastoreExceptionHelper
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void ThrowDatastoreExceptionHelperTest1()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    DbConnection conn = null; // TODO: Initialize to an appropriate value
        //    DbCommand comm = null; // TODO: Initialize to an appropriate value
        //    Exception innerException = null; // TODO: Initialize to an appropriate value
        //    bool throwException = false; // TODO: Initialize to an appropriate value
        //    Exception expected = null; // TODO: Initialize to an appropriate value
        //    Exception actual;
        //    actual = target.ThrowDatastoreExceptionHelper(conn, comm, innerException, throwException);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for ThrowDatastoreExceptionHelper
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void ThrowDatastoreExceptionHelperTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    string message = string.Empty; // TODO: Initialize to an appropriate value
        //    Exception innerException = null; // TODO: Initialize to an appropriate value
        //    bool throwException = false; // TODO: Initialize to an appropriate value
        //    Exception expected = null; // TODO: Initialize to an appropriate value
        //    Exception actual;
        //    actual = target.ThrowDatastoreExceptionHelper(message, innerException, throwException);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for SetTableAutoIncrementStart
        /////</summary>
        //[TestMethod()]
        //public void SetTableAutoIncrementStartTest()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    string tableName = string.Empty; // TODO: Initialize to an appropriate value
        //    long start = 0; // TODO: Initialize to an appropriate value
        //    target.SetTableAutoIncrementStart(tableName, start);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for releaseAccessControl
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void releaseAccessControlTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    FileInfo file = null; // TODO: Initialize to an appropriate value
        //    target.releaseAccessControl(file);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for MoveTo
        /////</summary>
        //[TestMethod()]
        //public void MoveToTest()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    string path1 = string.Empty; // TODO: Initialize to an appropriate value
        //    bool expected = false; // TODO: Initialize to an appropriate value
        //    bool actual;
        //    actual = target.MoveTo(path1);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for MigrateTo
        /////</summary>
        //[TestMethod()]
        //public void MigrateToTest()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    string path1 = string.Empty; // TODO: Initialize to an appropriate value
        //    target.MigrateTo(path1);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for LogMessage
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void LogMessageTest1()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    string program = string.Empty; // TODO: Initialize to an appropriate value
        //    string message = string.Empty; // TODO: Initialize to an appropriate value
        //    string level = string.Empty; // TODO: Initialize to an appropriate value
        //    target.LogMessage(program, message, level);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for LogMessage
        /////</summary>
        //[TestMethod()]
        //public void LogMessageTest()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    string message = string.Empty; // TODO: Initialize to an appropriate value
        //    string level = string.Empty; // TODO: Initialize to an appropriate value
        //    target.LogMessage(message, level);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for InternalDetachDB
        /////</summary>
        //[TestMethod()]
        //public void InternalDetachDBTest()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    string externalDBAlias = string.Empty; // TODO: Initialize to an appropriate value
        //    target.InternalDetachDB(externalDBAlias);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for InternalAttachDB
        /////</summary>
        //[TestMethod()]
        //public void InternalAttachDBTest()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    DAL externalDB = null; // TODO: Initialize to an appropriate value
        //    string externalDBAlias = string.Empty; // TODO: Initialize to an appropriate value
        //    DbConnection expected = null; // TODO: Initialize to an appropriate value
        //    DbConnection actual;
        //    actual = target.InternalAttachDB(externalDB, externalDBAlias);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for Initialize
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void InitializeTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    target.Initialize();
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for HasStrataWithNoSampleGroups
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void HasStrataWithNoSampleGroupsTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    bool expected = false; // TODO: Initialize to an appropriate value
        //    bool actual;
        //    actual = target.HasStrataWithNoSampleGroups();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for HasSampleGroupUOMErrors
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void HasSampleGroupUOMErrorsTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    bool expected = false; // TODO: Initialize to an appropriate value
        //    bool actual;
        //    actual = target.HasSampleGroupUOMErrors();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for HasOrphanedStrata
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void HasOrphanedStrataTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    bool expected = false; // TODO: Initialize to an appropriate value
        //    bool actual;
        //    actual = target.HasOrphanedStrata();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for HasForeignKeyErrors
        /////</summary>
        //[TestMethod()]
        //public void HasForeignKeyErrorsTest()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    string table_name = string.Empty; // TODO: Initialize to an appropriate value
        //    bool expected = false; // TODO: Initialize to an appropriate value
        //    bool actual;
        //    actual = target.HasForeignKeyErrors(table_name);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for HasCruiseErrors
        /////</summary>
        //[TestMethod()]
        //public void HasCruiseErrorsTest1()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    bool expected = false; // TODO: Initialize to an appropriate value
        //    bool actual;
        //    actual = target.HasCruiseErrors();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for HasCruiseErrors
        /////</summary>
        //[TestMethod()]
        //public void HasCruiseErrorsTest()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    string[] errors = null; // TODO: Initialize to an appropriate value
        //    string[] errorsExpected = null; // TODO: Initialize to an appropriate value
        //    bool expected = false; // TODO: Initialize to an appropriate value
        //    bool actual;
        //    actual = target.HasCruiseErrors(out errors);
        //    Assert.AreEqual(errorsExpected, errors);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for HasBlankSpeciesCodes
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void HasBlankSpeciesCodesTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    bool expected = false; // TODO: Initialize to an appropriate value
        //    bool actual;
        //    actual = target.HasBlankSpeciesCodes();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for HasBlankLiveDead
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void HasBlankLiveDeadTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    bool expected = false; // TODO: Initialize to an appropriate value
        //    bool actual;
        //    actual = target.HasBlankLiveDead();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for HasBlankDefaultLiveDead
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void HasBlankDefaultLiveDeadTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    bool expected = false; // TODO: Initialize to an appropriate value
        //    bool actual;
        //    actual = target.HasBlankDefaultLiveDead();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for HasBlankCountOrMeasure
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void HasBlankCountOrMeasureTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    bool expected = false; // TODO: Initialize to an appropriate value
        //    bool actual;
        //    actual = target.HasBlankCountOrMeasure();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for GetUserInformation
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void GetUserInformationTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    string expected = string.Empty; // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.GetUserInformation();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for GetCreateTriggers
        /////</summary>
        //[TestMethod()]
        //public void GetCreateTriggersTest()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    string expected = string.Empty; // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.GetCreateTriggers();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for GetCreateSQL
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void GetCreateSQLTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    string expected = string.Empty; // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = target.GetCreateSQL();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for GetCallingProgram
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void GetCallingProgramTest()
        //{
        //    string expected = string.Empty; // TODO: Initialize to an appropriate value
        //    string actual;
        //    actual = DAL_Accessor.GetCallingProgram();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for Finalize
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void FinalizeTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    target.Finalize();
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for Dispose
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void DisposeTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    bool isDisposing = false; // TODO: Initialize to an appropriate value
        //    target.Dispose(isDisposing);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for DirectCopy
        /////</summary>
        //[TestMethod()]
        //public void DirectCopyTest2()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    string fileName = string.Empty; // TODO: Initialize to an appropriate value
        //    string table = string.Empty; // TODO: Initialize to an appropriate value
        //    string selection = string.Empty; // TODO: Initialize to an appropriate value
        //    OnConflictOption option = new OnConflictOption(); // TODO: Initialize to an appropriate value
        //    object[] selectionArgs = null; // TODO: Initialize to an appropriate value
        //    target.DirectCopy(fileName, table, selection, option, selectionArgs);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for DirectCopy
        /////</summary>
        //[TestMethod()]
        //public void DirectCopyTest1()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    DAL dataBase = null; // TODO: Initialize to an appropriate value
        //    string table = string.Empty; // TODO: Initialize to an appropriate value
        //    string selection = string.Empty; // TODO: Initialize to an appropriate value
        //    OnConflictOption option = new OnConflictOption(); // TODO: Initialize to an appropriate value
        //    object[] selectionArgs = null; // TODO: Initialize to an appropriate value
        //    target.DirectCopy(dataBase, table, selection, option, selectionArgs);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for DirectCopy
        /////</summary>
        //[TestMethod()]
        //public void DirectCopyTest()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    string fileName = string.Empty; // TODO: Initialize to an appropriate value
        //    string table = string.Empty; // TODO: Initialize to an appropriate value
        //    string selection = string.Empty; // TODO: Initialize to an appropriate value
        //    object[] selectionArgs = null; // TODO: Initialize to an appropriate value
        //    target.DirectCopy(fileName, table, selection, selectionArgs);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for DetachDB
        /////</summary>
        //[TestMethod()]
        //public void DetachDBTest()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    string externalDBAlias = string.Empty; // TODO: Initialize to an appropriate value
        //    target.DetachDB(externalDBAlias);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for CopyTo
        /////</summary>
        //[TestMethod()]
        //public void CopyToTest1()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    string path1 = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL expected = null; // TODO: Initialize to an appropriate value
        //    DAL actual;
        //    actual = target.CopyTo(path1);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for CopyTo
        /////</summary>
        //[TestMethod()]
        //public void CopyToTest()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    string path1 = string.Empty; // TODO: Initialize to an appropriate value
        //    bool overwrite = false; // TODO: Initialize to an appropriate value
        //    DAL expected = null; // TODO: Initialize to an appropriate value
        //    DAL actual;
        //    actual = target.CopyTo(path1, overwrite);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for CopyAs
        /////</summary>
        //[TestMethod()]
        //public void CopyAsTest()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    string path1 = string.Empty; // TODO: Initialize to an appropriate value
        //    bool expected = false; // TODO: Initialize to an appropriate value
        //    bool actual;
        //    actual = target.CopyAs(path1);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for BuildDBFile
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void BuildDBFileTest2()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    string createSQL = string.Empty; // TODO: Initialize to an appropriate value
        //    string createTriggers = string.Empty; // TODO: Initialize to an appropriate value
        //    target.BuildDBFile(path, createSQL, createTriggers);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for BuildDBFile
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void BuildDBFileTest1()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    target.BuildDBFile();
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for BuildDBFile
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void BuildDBFileTest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    DAL_Accessor target = new DAL_Accessor(param0); // TODO: Initialize to an appropriate value
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    target.BuildDBFile(path);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}




        ///// <summary>
        /////A test for AttachDB
        /////</summary>
        //[TestMethod()]
        //public void AttachDBTest()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path); // TODO: Initialize to an appropriate value
        //    DAL externalDB = null; // TODO: Initialize to an appropriate value
        //    string externalDBAlias = string.Empty; // TODO: Initialize to an appropriate value
        //    target.AttachDB(externalDB, externalDBAlias);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for DAL Constructor
        /////</summary>
        //[TestMethod()]
        //public void DALConstructorTest1()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path);
        //    Assert.Inconclusive("TODO: Implement code to verify target");
        //}

        ///// <summary>
        /////A test for DAL Constructor
        /////</summary>
        //[TestMethod()]
        //public void DALConstructorTest()
        //{
        //    string path = string.Empty; // TODO: Initialize to an appropriate value
        //    bool makeNew = false; // TODO: Initialize to an appropriate value
        //    DAL target = new DAL(path, makeNew);
        //    Assert.Inconclusive("TODO: Implement code to verify target");
        //}
    }
}
