using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CruiseDAL;
using System.Collections;

namespace CruiseDALTest
{
    /// <summary>
    /// Summary description for ReadTests
    /// </summary>
    [TestClass]
    public class ReadTests
    {
        public ReadTests()
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

        public void ReadOneAllSetup(DAL db)
        {
            StringBuilder sb = new StringBuilder();
            foreach (String tableName in CruiseDAL.Schema.Schema.TABLE_NAMES)
            {
                List<ColumnInfo> columnInfos = db.GetTableInfo(tableName);
                String[] columnNames = (from ci in columnInfos
                                       where ci.IsRequired
                                       select ci.Name).ToArray();
                String[] values = (from ci in columnInfos
                                   where ci.IsRequired
                                   select (ci.DBType == "TEXT") ? "'TestStr'" : 
                                   (ci.DBType == "INTEGER") ? "101010" : 
                                   (ci.DBType == "REAL") ? "202.02" : "'Something'").ToArray();

                if (columnNames.Length > 0)
                {
                    sb.AppendFormat("INSERT INTO {0} ({1}) VALUES ({2}); ", tableName, string.Join(",", columnNames), string.Join(",", values));
                }
                else
                {
                    sb.AppendFormat("INSERT INTO {0} DEFAULT VALUES; ", tableName);                    
                }
            }

            db.Execute(sb.ToString());

        }

        public void ReadOneAllTablesCore(DAL db)
        {
            ArrayList results = new ArrayList();


            results.Add(db.Read<CruiseDAL.DataObjects.BiomassEquationDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.ComponentDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.CountTreeDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.CruiseMethodsDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.CuttingUnitDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.CuttingUnitStratumDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.ErrorLogDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.ForestsDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.GlobalsDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.LCDDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.LogDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.LogFieldSetupDefaultDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.LogFieldSetupDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.LoggingMethodsDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.LogMatrixDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.LogStockDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.MessageLogDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.PlotDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.POPDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.PRODO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.ProductCodesDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.QualityAdjEquationDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.RegionsDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.RegressionDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.ReportsDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.SaleDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.SampleGroupDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.SampleGroupStatsDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.SampleGroupStatsTreeDefaultValueDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.SampleGroupTreeDefaultValueDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.StemDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.StratumDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.StratumStatsDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.TallyDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.TreeAuditValueDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.TreeCalculatedValuesDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.TreeDefaultValueDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.TreeDefaultValueTreeAuditValueDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.TreeDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.TreeEstimateDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.TreeFieldSetupDefaultDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.TreeFieldSetupDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.UOMCodesDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.ValueEquationDO>(null, null));
            results.Add(db.Read<CruiseDAL.DataObjects.VolumeEquationDO>(null, null));
            //results.Add(db.Read<CruiseDAL.DataObjects.StratumAcres_ViewDO>(null, null));

            //List<CruiseDAL.DataObjects.CountTree_ViewDO> ctv = db.Read<CruiseDAL.DataObjects.CountTree_ViewDO>(null, null);
            //foreach (CruiseDAL.DataObjects.CountTree_ViewDO ct in ctv)
            //{
            //    TestContext.WriteLine(ct.ToString());
            //}
            //results.Add(ctv);

            foreach (object obj in results)
            {
                IList inst = obj as IList;
           
                Assert.IsTrue(obj != null);
                Assert.IsTrue(inst != null);
                Assert.IsTrue(inst.Count > 0, "Expected " + inst.ToString() + " to contain items");
            }

        }

        [TestMethod]
        public void ReadOneAllTables()
        {
            DAL db = new DAL("TestReadOneAllTables.cruise", true);
            ReadOneAllSetup(db);

            ReadOneAllTablesCore(db);
        }

        [TestMethod]
        public void ReadOneAllTablesWithExisting()
        {
            DAL db = new DAL("7Wolf.cruise");
            ReadOneAllSetup(db);

            ReadOneAllTablesCore(db);

        }
    }
}
