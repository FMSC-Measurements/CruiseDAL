using CruiseDAL.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CruiseDAL.MappingCollections;
using CruiseDAL;

namespace CruiseDALTest
{
    


    /// <summary>
    ///This is a test class for SampleGroupDOTest and is intended
    ///to contain all SampleGroupDOTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SampleGroupDOTest
    {
        public static DAL _DAL = null;
        public static int CurrentCode = 0;

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

        public SampleGroupDO getDummySampleGroup()
        {
            var sg = _DAL.DOFactory.GetNew<SampleGroupDO>() as SampleGroupDO;
            sg.rowID = 0;
            sg.Code = (CurrentCode++).ToString();
            sg.CutLeave = "hi"; ;
            sg.UOM = "TestUOM";
            sg.PrimaryProduct = "11";

            return sg;
        }

        public TreeDefaultValueDO getDummyTDV()
        {
            var tdv = _DAL.DOFactory.GetNew<TreeDefaultValueDO>() as TreeDefaultValueDO;
            tdv.PrimaryProduct = "12";
            tdv.Species = "testSpecies";
            tdv.LiveDead = "dead";
            return tdv;
        }

        public StratumDO getDummyStratum()
        {
            var st = _DAL.DOFactory.GetNew<StratumDO>() as StratumDO;
            st.Code = (CurrentCode++).ToString();
            st.Method = "TestMethod";
            return st;
        }

        /// <summary>
        ///A test for Stratum
        ///</summary>
        [TestMethod()]
        public void StratumTest()
        {
            var sg = getDummySampleGroup();
            var st = getDummyStratum();
            st.Save();
            sg.Stratum_CN = st.Stratum_CN;

            Assert.IsNotNull(sg.Stratum);
            Assert.ReferenceEquals(st, sg.Stratum);

            var st2 = getDummyStratum();
            st2.Save();

            sg.Stratum_CN = st2.Stratum_CN;

            Assert.ReferenceEquals(st2, sg.Stratum);

        }

        ///// <summary>
        /////A test for TreeDefaultValues
        /////</summary>
        //[TestMethod()]
        //public void TreeDefaultValuesTest()
        //{
        //    SampleGroupDO target = new SampleGroupDO(); // TODO: Initialize to an appropriate value
        //    TreeDefaultValueCollection actual;
        //    actual = target.TreeDefaultValues;
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}
    }
}
