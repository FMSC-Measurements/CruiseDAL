using System;
using System.Collections.Generic;
using System.Text;
using FMSC.ORM.Core;
using System.Linq;

namespace CruiseDAL.DataObjects
{
    public partial class StratumStatsDO
    {

        protected override void OnDALChanged(Datastore newDAL)
        {
            base.OnDALChanged(newDAL);
        }


        public int DeleteStratumStats(Datastore dal, long? StratumStats_CN)
        {
            //Delete sample group stats for stratum
            var allSGStatsInStratumStats = dal.From<SampleGroupStatsDO>()
                 .Where("StratumStats_CN = @p1")
                 .Read(StratumStats_CN).ToList();
                                
           foreach (SampleGroupStatsDO SgStats in allSGStatsInStratumStats)
           {
              SgStats.Delete();
           }

           Delete();
           
           return (0);
        }
    }
}
