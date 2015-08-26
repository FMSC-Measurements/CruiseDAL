using CruiseDAL.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CruiseDAL;
using System.Collections.Generic;

namespace CruiseDALTest
{
    
    
    /// <summary>
    ///This is a test class for LogMatrixDOTest and is intended
    ///to contain all LogMatrixDOTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LogMatrixDOTest
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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
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


        /// <summary>
        ///A test for GetByRowNumberGroupBySpecies
        ///</summary>
        [TestMethod()]
        public void GetByRowNumberGroupBySpeciesTest()
        {
            DAL cruiseDAL = DALTest.GetTestDAL();
            string reportNum = "008"; 


            Dictionary<string, List<LogMatrixDO>> groupedCollection;
            groupedCollection = LogMatrixDO.GetByRowNumberGroupBySpecies(cruiseDAL, reportNum);
            Assert.IsNotNull(groupedCollection);//actual should allways be not null, but may be empty

            foreach (string species in groupedCollection.Keys)
            {
                foreach (LogMatrixDO lm in groupedCollection[species])
                {
                    TestContext.WriteLine(lm.GradeDescription);
                }
            }


            
        }
    }
}
