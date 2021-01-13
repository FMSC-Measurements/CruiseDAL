using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class CuttingUnit_StratumMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
            return
$@"INSERT INTO {toDbName}.CuttingUnit_Stratum ( 
    CruiseID,
    CuttingUnitCode,
    StratumCode,
    StratumArea
)
SELECT
    '{cruiseID}',
    cu.Code,
    st.Code,
    cust.StratumArea 
FROM {fromDbName}.CuttingUnitStratum AS cust
JOIN {fromDbName}.CuttingUnit AS cu USING (CuttingUnit_CN) 
JOIN {fromDbName}.Stratum AS st USING (Stratum_CN);";
        }
    }
}
