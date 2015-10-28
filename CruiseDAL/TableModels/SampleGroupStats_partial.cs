using System;
using System.Collections.Generic;
using System.Text;
using CruiseDAL.MappingCollections;

namespace CruiseDAL.DataObjects
{
    public partial class SampleGroupStatsDO
    {
        protected TreeDefaultValueStatsCollection _TreeDefaultValueStats = null;
        public TreeDefaultValueStatsCollection TreeDefaultValueStats
        {
            get
            {
                if (_TreeDefaultValueStats == null)
                {
                    _TreeDefaultValueStats = new TreeDefaultValueStatsCollection(this);
                }
                return _TreeDefaultValueStats;
            }
        }
        protected override void OnDALChanged(DatastoreBase newDAL)
        {
            base.OnDALChanged(newDAL);
            if (_TreeDefaultValueStats != null)
            {
                _TreeDefaultValueStats.DAL = (DAL)newDAL;
            }
        }

        public void DeleteSgStat()
        {
           base.Delete();
        }

       public override void Delete()
        {
            this.TreeDefaultValueStats.Populate();
            
            this.TreeDefaultValueStats.Clear();
            this.TreeDefaultValueStats.Save();
            base.Delete();
        }

    }
}
