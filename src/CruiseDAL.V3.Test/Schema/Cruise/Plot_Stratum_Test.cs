using CruiseDAL.TestCommon;
using CruiseDAL.V3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CruiseDAL.V3.Test.Schema.Cruise
{
    public class Plot_Stratum_Test
    {
        [Fact]
        public void ThreePPNT_FieldCheck()
        {
            var init = new DatabaseInitializer();

            using var db = init.CreateDatabase();

            var stratum = init.PlotStrata.First();
            var unit = init.Units.First();

            var plot = new Plot()
            {
                CruiseID = init.CruiseID,
                PlotID = Guid.NewGuid().ToString(),
                PlotNumber = 1,
                CuttingUnitCode = unit
            };
            db.Insert(plot);

            var plotStratum = new Plot_Stratum()
            {
                CruiseID = init.CruiseID,
                PlotNumber = plot.PlotNumber,
                CuttingUnitCode = unit,
                StratumCode = stratum.StratumCode,
  
            };
            db.Insert(plotStratum);

            plotStratum.AverageHeight = 1;
            plotStratum.KPI = 1;
            plotStratum.TreeCount = 1;
            db.Update(plotStratum);
        }
    }
}
