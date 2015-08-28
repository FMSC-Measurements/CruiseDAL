using CruiseDAL.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CruiseDAL.MappingCollections;
using CruiseDAL;

namespace CruiseDAL.DataObjects.Tests
{
    
    
    /// <summary>
    ///This is a test class for CuttingUnitDOTest and is intended
    ///to contain all CuttingUnitDOTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CuttingUnitDOTest
    {
        public readonly string TEST_FILE_PATH = @"C:\Documents and Settings\benjaminjcampbell\My Documents\Visual Studio 2008\Projects\CruiseDAL\Test.small.cruise";


        private static DAL _DAL { get; set; }
        private static int CurrentCode = 0;
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            _DAL = new DAL("CuttingUnitDOTest.cruise", true);
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
        ///A test for Strata
        ///</summary>
        [TestMethod()]
        public void StrataTest()
        {
            CuttingUnitDO cutUnit = getDummyCuttingUnit();
            cutUnit.Save();
            var stratum = getDummyStratum();
            stratum.Save();
            cutUnit.Strata.Add(stratum);
            cutUnit.Strata.Save();
            stratum.CuttingUnits.Populate();
            Assert.AreEqual(1, stratum.CuttingUnits.Count);

            cutUnit = getDummyCuttingUnit();
            cutUnit.Save();
            stratum.CuttingUnits.Add(cutUnit);
            stratum.CuttingUnits.Save();

            var cust = _DAL.Read<CuttingUnitStratumDO>("CuttingUnitStratum", null);

            cutUnit.Strata.Populate();
            Assert.AreEqual(1, cutUnit.Strata.Count);
            Assert.AreEqual(2, stratum.CuttingUnits.Count);

            

        }

        [TestMethod()]
        public void StrataTest2()
        {
            //here we are going to copy all the cutting units
            //and all the strata belonging to them from one database
            //to another, along with the many-to-many relationships between them. 

            //To do this we need to remember that the CuttingUnitStratum table 
            //needs to know the CN values of both the cutting unit and the stratum
            //before a mapping can be made. To do that we need to save the strata and
            //cutting units before we save the mappings.

            DAL originDAL = new DAL(TEST_FILE_PATH);
            DAL destDAL = new DAL("StrataTest2.cruise", true);

            var cuttingUnits = originDAL.Read<CuttingUnitDO>("CuttingUnit", null, null);

            //here we want to make sure all the strata
            //we will be coping are loaded into memory
            //because once we start coping we don't want
            //to reread any strata untill we are done 
            foreach (CuttingUnitDO cu in cuttingUnits)
            {
                cu.Strata.Populate();
            }

            //next we want to save all the strata into
            //our destination database so we know 
            //their CN values for the next step

            foreach (CuttingUnitDO cu in cuttingUnits)
            {
                foreach (StratumDO st in cu.Strata)
                {
                    if (st.DAL != destDAL)
                    {
                        st.DAL = destDAL;
                        st.Save();
                    }
                }
            }

            //now that we know that all the strata have 
            //their new CNs, we need to save the cuttingUnits
            //and get their new CNs too. 
            //We can also save the mapping collection during
            //this step, because the CN value on the CuttingUnit
            //is the last thing we need to create all the mappings
            //for a given cutting unit
            foreach (CuttingUnitDO cu in cuttingUnits)
            {
                cu.DAL = destDAL;
                cu.Save();
                cu.Strata.Save();
            }

        }

        public StratumDO getDummyStratum()
        {
            var st = _DAL.DOFactory.GetNew<StratumDO>() as StratumDO;
            st.Code = (CurrentCode++).ToString();
            st.Method = "TestMethod";
            return st;
        }

        public CuttingUnitDO getDummyCuttingUnit()
        {
            var cu = _DAL.DOFactory.GetNew<CuttingUnitDO>() as CuttingUnitDO;
            cu.Code = (CurrentCode++).ToString();
            cu.Area = 100L;
            return cu;
        }

        

    }
}
