using CruiseDAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CruiseDAL.DataObjects;

namespace CruiseDALTest
{
    
    
    /// <summary>
    ///This is a test class for ObjectCacheTest and is intended
    ///to contain all ObjectCacheTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ObjectCacheTest
    {

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

        private static DAL _DAL { get; set; }
        private static DataObjectFactory _DOFactory { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            _DAL = new DAL("ObjectCacheTest.cruise", true);
            _DOFactory = new DataObjectFactory(_DAL);
        }
        
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
        ///A test for Count
        ///</summary>
        [TestMethod()]
        public void CountTest()
        {
            DataObjectFactory DOFactory = new DataObjectFactory(_DAL); 
            ObjectCache target = new ObjectCache(DOFactory);

            target.GetByID<SaleDO>(1);

            Assert.AreEqual(1, target.Count);

            var tempSale = new SaleDO { rowID = 2 };
            target.Add( tempSale, ObjectCache.AddBehavior.THROWEXCEPTION);

            Assert.AreEqual(2, target.Count);

            target.Remove(tempSale);

            Assert.AreEqual(1, target.Count);

            //target.Clear();

            //Assert.AreEqual(0, target.Count);

        }





        /// <summary>
        ///A test for GetByID
        ///</summary>
        [TestMethod()]
        public void GetByIDTest()
        {
            ObjectCache target = new ObjectCache(_DOFactory);

            Assert.IsNull( target.GetByID(1));

            //GetByID with a type argument should retrun an object and added it to the cache
            SaleDO tempSale = target.GetByID<SaleDO>(1) as SaleDO;
            Assert.IsNotNull(tempSale);


            Assert.IsNotNull(target.GetByID(1));
        }



        /// <summary>
        ///A test for Add
        ///</summary>
        [TestMethod()]
        public void AddTest()
        {
            ObjectCache target = new ObjectCache(_DOFactory);

            var tempSale = new SaleDO { rowID = 1 };

            Assert.AreEqual(true, target.Add(tempSale, ObjectCache.AddBehavior.DONT_OVERWRITE));
            tempSale = new SaleDO { rowID = 1 };
            Assert.AreEqual(false, target.Add(tempSale, ObjectCache.AddBehavior.DONT_OVERWRITE));
            tempSale = new SaleDO { rowID = 1 };
            Assert.AreEqual(true, target.Add(tempSale, ObjectCache.AddBehavior.OVERWRITE));
            tempSale = new SaleDO { rowID = 1 };
            try
            {
                target.Add(tempSale, ObjectCache.AddBehavior.THROWEXCEPTION);
                Assert.Fail();
            }
            catch (Exception e)
            {
                //TODO proper exception type not defined, for AddBehavior.THROW_EXCEPTION 
            }
        }

        /// <summary>
        ///A test for ObjectCache Constructor
        ///</summary>
        [TestMethod()]
        public void ObjectCacheConstructorTest()
        {
            ObjectCache target = new ObjectCache(_DOFactory);
            Assert.IsNotNull(target);
        }
    }
}
