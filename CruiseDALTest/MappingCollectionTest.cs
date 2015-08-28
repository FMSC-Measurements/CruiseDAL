using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CruiseDAL;
using CruiseDAL.DataObjects;

namespace CruiseDAL.Tests
{
    /// <summary>
    /// Summary description for MappingCollectionTest
    /// </summary>
    [TestClass]
    public class MappingCollectionTest
    {
        public MappingCollectionTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private DAL testDB;

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
         //Use TestInitialize to run code before running each test 
         [TestInitialize()]
         public void MyTestInitialize() 
         {
             testDB = new DAL(TestContext.TestName + ".cruise", true);
             Random rand = new Random(0);

             List<CuttingUnitDO> units = new List<CuttingUnitDO>();
             for (int i = 1; i <= 10; i++)
             {
                 CuttingUnitDO cu = new CuttingUnitDO(testDB);
                 cu.Code = i.ToString();
                 cu.Area = rand.Next(9) + 1.0f;
                 cu.Save();
                 units.Add(cu);
             }

             List<StratumDO> strata = new List<StratumDO>();
             for (int i = 0; i <= 10; i++)
             {
                 StratumDO st = new StratumDO(testDB);
                 st.Code = i.ToString();
                 st.Method = "something";
                 st.Save();
                 strata.Add(st);
             }

             foreach (CuttingUnitDO cu in units)
             {
                 for (int i = 0; i < cu.Area; i++)
                 {
                     CuttingUnitStratumDO cust = new CuttingUnitStratumDO(testDB);
                     cust.CuttingUnit_CN = cu.CuttingUnit_CN;
                     cust.Stratum_CN = strata[i].Stratum_CN;
                     cust.Save();
                 }
             }

             testDB.FlushCache();


         }
         //
         // Use TestCleanup to run code after each test has run
         // [TestCleanup()]
         // public void MyTestCleanup() { }
         //
        #endregion

         [TestMethod]
        public void MappingCollectionReadTest()
        {
            List<CuttingUnitDO> units = testDB.Read<CuttingUnitDO>(null, null);
            foreach (CuttingUnitDO u in units)
            {
                u.Strata.Populate();
                Assert.IsTrue(u.Strata.Count == (int)u.Area);
            }
        }

         [TestMethod]
         public void MappingCollectionIndexTest()
         {             

             List<CuttingUnitDO> units = testDB.Read<CuttingUnitDO>(null, null);
             foreach (CuttingUnitDO u in units)
             {
                 u.Strata.Populate();
                 for (int i = 0; i < u.Strata.Count; i++)
                 {
                     Assert.IsTrue(u.Strata[i] != null);
                 }
             }

             int expectedCount = units[0].Strata.Count + 10;
             for (int i = 0; i < 10; i++)
             {
                 units[0].Strata.Add(new StratumDO());
             }
             Assert.IsTrue(units[0].Strata.Count == expectedCount);
         }

         [TestMethod]
         public void MappingCollectionPopulateTest()
         {
             List<CuttingUnitDO> units = testDB.Read<CuttingUnitDO>(null, null);
             List<StratumDO> strata = testDB.Read<StratumDO>(null, null);

             foreach (CuttingUnitDO c in units)
             {
                 c.Strata.Populate();
                 foreach (StratumDO s in c.Strata)
                 {
                     Assert.IsTrue(strata.Contains(s));
                 }
             }

             foreach (StratumDO s in strata)
             {
                 s.CuttingUnits.Populate();
                 foreach (CuttingUnitDO c in s.CuttingUnits)
                 {
                     Assert.IsTrue(units.Contains(c));
                 }
             }

             CuttingUnitDO cu = units[0];
             StratumDO st = strata[0];
             cu.Strata.Populate();
             cu.Strata.Add(st);
             cu.Strata.Populate();
             Assert.IsTrue(cu.Strata.Contains(st));

         }
    }
}
