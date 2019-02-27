using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V2Backports
{
    public partial class DDL
    {
        public const string CREATE_VIEW_CUTTINGUNITSTRATUM =
            "CREATE VIEW CuttingUnitStratum " +
            "AS " +
                "SELECT cu.CuttingUnit_CN, " +
                "st.Stratum_CN, " +
                "StratumArea " +
                "FROM CuttingUnit_Stratum AS cust " +
                "JOIN CuttingUnit AS cu ON cust.CuttingUnitCode = cu.Code " +
                "JOIN Stratum AS st ON cust.StratumCode = st.Code;";
    }
}
