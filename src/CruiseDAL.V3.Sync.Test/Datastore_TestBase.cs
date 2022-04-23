using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using CruiseDAL.V3.Sync.Util;
using System;
using System.Linq;
using Xunit.Abstractions;

namespace CruiseDAL.V3.Sync
{
    public class Datastore_TestBase : TestBase
    {
        protected DatabaseInitializer DbInitializer { get; }

        public string CruiseID => DbInitializer.CruiseID;
        public string SaleID => DbInitializer.SaleID;

        protected string[] Units => DbInitializer.Units;
        protected Stratum[] Strata => DbInitializer.Strata;
        protected CuttingUnit_Stratum[] UnitStrata => DbInitializer.UnitStrata;
        protected string[] Species => DbInitializer.Species;
        protected SampleGroup[] SampleGroups => DbInitializer.SampleGroups;
        protected TreeDefaultValue[] TreeDefaults => DbInitializer.TreeDefaults;
        protected SubPopulation[] Subpops => DbInitializer.Subpops;
        public Stratum[] PlotStrata => DbInitializer.PlotStrata;
        public Stratum[] NonPlotStrata => DbInitializer.NonPlotStrata;

        public Datastore_TestBase(ITestOutputHelper output) : base(output)
        {
            DbInitializer = new DatabaseInitializer();
        }

        protected CruiseDatastore_V3 CreateDatabase(string cruiseID = null, string saleID = null)
        {
            return DbInitializer.CreateDatabase(cruiseID, saleID);
        }

        protected CruiseDatastore_V3 CreateDatabaseFile(string path, string cruiseID = null, string saleID = null)
        {
            return DbInitializer.CreateDatabaseFile(path, cruiseID, saleID);
        }
    }
}