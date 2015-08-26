using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using CruiseDAL;

namespace CruiseDALTest
{
    /// <summary>
    /// Summary description for DataObjectDiscriptionTest
    /// </summary>
    [TestClass]
    public class DataObjectDiscriptionTest
    {
        public DataObjectDiscriptionTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void LoadDataObjects()
        {
            var types = (from t in Assembly.GetAssembly(typeof(CruiseDAL.DataObject)).GetTypes()
                        where t.IsClass && t.Namespace == "CruiseDAL.DataObjects"
                        select t).ToList();

            foreach (Type t in types)
            {
                TestContext.WriteLine(t.FullName);
                DataObjectInfo doi = new DataObjectInfo(t);
                Assert.IsTrue(doi.ReadSource != null);
            }
        }
    }
}
