using CruiseDAL.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CruiseDAL;
using System;


namespace CruiseDALTest
{
    
    
    /// <summary>
    ///This is a test class for SaleDOTest and is intended
    ///to contain all SaleDOTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SaleDOTest
    {

        private static DAL _DAL { get; set; }
       

        private TestContext testContextInstance;
        private static int CurrentSaleNum = 0;

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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            _DAL = new DAL("SaleDOTest.cruise", true);
        }
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

        /// <summary>
        ///A test for Sale_CN
        ///</summary>
        [TestMethod()]
        public void Sale_CNTest()
        {
            SaleDO target = GetDumbySale(); 
            target.Save();
            Assert.IsNotNull(target.Sale_CN);
            Assert.AreEqual(target.rowID, target.Sale_CN);
        }



        /// <summary>
        ///A test for Name
        ///</summary>
        [TestMethod()]
        public void NameTest()
        {
            SaleDO target = new SaleDO(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.Name = expected;
            actual = target.Name;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ModifiedDate
        ///</summary>
        [TestMethod()]
        public void ModifiedDateTest()
        {
            SaleDO target = new SaleDO(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            //target.ModifiedDate = expected;
            //actual = target.ModifiedDate;
            //Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ModifiedBy
        ///</summary>
        [TestMethod()]
        public void ModifiedByTest()
        {
            var sale = GetDumbySale();
            sale.Save();
            sale = _DAL.ReadSingleRow<SaleDO>("Sale", sale.rowID);
            //Assert.IsNull(sale.ModifiedBy);
            sale.Name = "something";
            sale.Save();
            sale = _DAL.ReadSingleRow<SaleDO>("Sale", sale.rowID);
            //Assert.AreEqual(_DAL.User, sale.ModifiedBy);
        }





        /// <summary>
        ///A test for Not Null cols
        ///</summary>
        [TestMethod()]
        public void NotNullTest()
        {
            var sale = GetDumbySale();
            sale.Save();
            sale.District = null;
            try
            {
                sale.Save();
                Assert.Fail();
            }
            catch (System.Data.SQLite.SQLiteException e)
            { }

            sale = GetDumbySale();
            sale.Save();
            sale.Region = null;
            try
            {
                sale.Save();
                Assert.Fail();
            }
            catch (System.Data.SQLite.SQLiteException e)
            { }

            sale = GetDumbySale();
            sale.Save();
            sale.Forest = null;
            try
            {
                sale.Save();
                Assert.Fail();
            }
            catch (System.Data.SQLite.SQLiteException e)
            { }
        }

        /// <summary>
        ///A test for CreatedDate
        ///</summary>
        [TestMethod()]
        public void CreatedDateTest()
        {
            var toSaveSale = _DAL.DOFactory.GetNew<SaleDO>() as SaleDO;

            var saleNum = (++CurrentSaleNum).ToString();
            toSaveSale.SaleNumber = saleNum;
            toSaveSale.Region = "TestRegion";
            toSaveSale.Forest = "TestFortest";
            toSaveSale.District = "TestDistrict";
            toSaveSale.Save();

            var readSale = _DAL.ReadSingleRow<SaleDO>("Sale", toSaveSale.rowID);
            Assert.ReferenceEquals(toSaveSale, readSale);

            DateTime result;
            //DateTime.TryParse(readSale.CreatedDate, out result);
            //Assert.AreEqual(DateTime.Now.Day, result.Day);
            //Assert.AreEqual(DateTime.Now.Month, result.Month);
            //Assert.AreEqual(DateTime.Now.Year, result.Year);
        }




        /// <summary>
        ///A test for Unique propertys
        ///</summary>
        [TestMethod()]
        public void TestUniques()
        {
            var saleA = GetDumbySale();
            var saleB = GetDumbySale();
            saleB.SaleNumber = saleA.SaleNumber = "Duplicate";

            saleA.Save();

            try
            {
                saleB.Save();
                Assert.Fail();
            }
            catch (CruiseDAL.DatabaseExecutionException e)
            {
                //pass
            }
        }

        public SaleDO GetDumbySale()
        {
            var sale = _DAL.DOFactory.GetNew<SaleDO>() as SaleDO;
            var saleNum = (++CurrentSaleNum).ToString();
            sale.SaleNumber = saleNum;
            sale.Region = "TestRegion";
            sale.Forest = "TestFortest";
            sale.District = "TestDistrict";

            return sale;
        }

        /// <summary>
        ///A test for SetValues
        ///</summary>
        [TestMethod()]
        public void SetValuesTest()
        {
            SaleDO target = new SaleDO(); // TODO: Initialize to an appropriate value
            SaleDO Sale = null; 
            target.SetValues(Sale);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        ///// <summary>
        /////A test for GetValidator
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void GetValidatorTest()
        //{
        //    SaleDO_Accessor target = new SaleDO_Accessor(); // TODO: Initialize to an appropriate value
        //    RowValidator expected = null; // TODO: Initialize to an appropriate value
        //    RowValidator actual;
        //    actual = target.GetValidator();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}





        /// <summary>
        ///A test for SaleDO Constructor
        ///</summary>
        [TestMethod()]
        public void SaleDOConstructorTest()
        {
            var tempSale = new SaleDO(_DAL);
            Assert.IsNotNull(tempSale.DAL);
            Assert.AreEqual(false, tempSale.IsPersisted);

            var tempDO = _DAL.DOFactory.GetNew<SaleDO>();
            Assert.IsInstanceOfType(tempDO, typeof(SaleDO));
            Assert.IsNotNull(tempDO.DAL);
            Assert.AreEqual(false, tempDO.IsPersisted);
        }
    }
}
