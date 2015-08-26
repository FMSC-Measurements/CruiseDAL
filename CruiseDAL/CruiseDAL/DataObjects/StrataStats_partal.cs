using System;
using System.Collections.Generic;
using System.Text;
using CruiseDAL.MappingCollections;

namespace CruiseDAL.DataObjects
{
    public partial class StratumStatsDO
    {

        protected override void OnDALChanged(DatastoreBase newDAL)
        {
            base.OnDALChanged(newDAL);
        }

        public override void Delete()
        {
            base.Delete();
        }

        public int DeleteStratumStats(DAL dal, long? StratumStats_CN)
        {
           //Delete sample group stats for stratum
           List<SampleGroupStatsDO> allSGStatsInStratumStats = dal.Read<SampleGroupStatsDO>("SampleGroupStats",
               "WHERE StratumStats_CN = ?", StratumStats_CN.ToString());
           foreach (SampleGroupStatsDO SgStats in allSGStatsInStratumStats)
           {
              SgStats.Delete();
           }

           Delete();
           
           return (0);
        }
    }
}
