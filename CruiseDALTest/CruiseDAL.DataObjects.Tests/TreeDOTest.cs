using CruiseDAL.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CruiseDAL;
using System.ComponentModel;
using System.Collections.Generic;

namespace CruiseDAL.DataObjects.Tests
{
    
    
    /// <summary>
    ///This is a test class for TreeDOTest and is intended
    ///to contain all TreeDOTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TreeDOTest
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


        ///// <summary>
        /////A test for SetValues
        /////</summary>
        //[TestMethod()]
        //public void SetValuesTest1()
        //{
        //    TreeDO target = new TreeDO(); // TODO: Initialize to an appropriate value
        //    TreeDO obj = null; // TODO: Initialize to an appropriate value
        //    target.SetValues(obj);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for SetValues
        /////</summary>
        //[TestMethod()]
        //public void SetValuesTest()
        //{
        //    TreeDO target = new TreeDO(); // TODO: Initialize to an appropriate value
        //    DataObject obj = null; // TODO: Initialize to an appropriate value
        //    target.SetValues(obj);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for DoValidate
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("CruiseDAL.dll")]
        //public void DoValidateTest()
        //{
        //    TreeDO_Accessor target = new TreeDO_Accessor(); // TODO: Initialize to an appropriate value
            
        //    bool expected = false; // TODO: Initialize to an appropriate value
        //    bool actual;
        //    actual = target.DoValidate();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        //[TestMethod()]
        //public void TestCustomTreeDORead()
        //{
        //    DAL db = new DAL(@"C:\Users\benjaminjcampbell\Desktop\Cruise File Stash\TWIN - Copy.cruise");
        //    List<CustomTreeDO> tList = db.Read<CustomTreeDO>("Tree", null);


        //}


        /// <summary>
        ///A test for TreeDO Constructor
        ///</summary>
        [TestMethod()]
        public void TreeDOConstructorTest()
        {
            DAL DAL = null; 
            TreeDO target = new TreeDO(DAL);

            TestContext.WriteLine("\r\nTesting countMeasure == NULL");
            TestContext.BeginTimer("validate1");
            Assert.IsFalse(target.Validate());
            TestContext.EndTimer("validate1");

            IDataErrorInfo tError = target;
            //display all errors
            TestContext.WriteLine("Error: " + tError.Error);
            //foreach (string colName in CruiseDAL.Schema.TREE._ALL)
            //{
            //    string error = tError[colName];
            //    if (string.IsNullOrEmpty(error)) { continue; }
            //    TestContext.WriteLine(colName + " error = " + error);
            //}
            
            

            string sgError = tError["SampleGroup"];
            Assert.IsFalse(string.IsNullOrEmpty(sgError), "SGError = " + sgError);

            //test countMeasure == C and revalidate 
            TestContext.WriteLine("\r\nTesting countMeasure == C");
            target.CountOrMeasure = "C";
            TestContext.BeginTimer("validate2");
            Assert.IsFalse(target.Validate());
            TestContext.EndTimer("validate2");
            

            //display all errors
            TestContext.WriteLine("Error: " + tError.Error);
            //foreach (string colName in CruiseDAL.Schema.TREE._ALL)
            //{
            //    string error = tError[colName];
            //    if (string.IsNullOrEmpty(error)) { continue; }
            //    TestContext.WriteLine(colName + " error = " + error);
            //}



            sgError = tError["SampleGroup"];
            Assert.IsFalse(string.IsNullOrEmpty(sgError), "SGError = " + sgError);

            TestContext.WriteLine("\r\nTesting countMeasure == M");
            target.CountOrMeasure = "M";
            TestContext.BeginTimer("validate3");
            Assert.IsFalse(target.Validate());
            TestContext.EndTimer("validate3");

            //display all errors
            TestContext.WriteLine("Error: " + tError.Error);
            //foreach (string colName in CruiseDAL.Schema.TREE._ALL)
            //{
            //    string error = tError[colName];
            //    if (string.IsNullOrEmpty(error)) { continue; }
            //    TestContext.WriteLine(colName + " error = " + error);
            //}

            TestContext.WriteLine("\r\nTesting countMeasure == M, DBH > 0");
            target.DBH = 1;
            target.Validate();
            TestContext.WriteLine("Error: " + tError.Error);

            TestContext.WriteLine("\r\nTesting countMeasure == M, Ht > 0");
            target.TotalHeight = 1;
            target.Validate();
            TestContext.WriteLine("Error: " + tError.Error);



            sgError = tError["SampleGroup"];
            Assert.IsFalse(string.IsNullOrEmpty(sgError), "SGError = " + sgError);
        }
    }
}
