using System;
using System.Collections.Generic;
using System.Text;
using FMSC.ORM.Core;

namespace CruiseDAL.DataObjects
{
    public partial class StratumStatsDO
    {

        protected override void OnDALChanged(DatastoreRedux newDAL)
        {
            base.OnDALChanged(newDAL);
        }


        public int DeleteStratumStats(DatastoreRedux dal, long? StratumStats_CN)
        {
           //Delete sample group stats for stratum
           List<SampleGroupStatsDO> allSGStatsInStratumStats = dal.Read<SampleGroupStatsDO>(
               "WHERE StratumStats_CN = ?", StratumStats_CN);
           foreach (SampleGroupStatsDO SgStats in allSGStatsInStratumStats)
           {
              SgStats.Delete();
           }

           Delete();
           
           return (0);
        }
    }
}
