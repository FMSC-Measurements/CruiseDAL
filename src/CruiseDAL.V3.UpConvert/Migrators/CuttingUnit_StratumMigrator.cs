using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class CuttingUnit_StratumMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
            return
$@"INSERT INTO {toDbName}.CuttingUnit_Stratum ( 
    CruiseID,
    CuttingUnitCode,
    StratumCode,
    StratumArea,
    CreatedBy
)
SELECT
    '{cruiseID}',
    cu.Code,
    st.Code,
    cust.StratumArea ,
    '{deviceID}' AS CreatedBy
FROM {fromDbName}.CuttingUnitStratum AS cust
JOIN {fromDbName}.CuttingUnit AS cu USING (CuttingUnit_CN) 
JOIN {fromDbName}.Stratum AS st USING (Stratum_CN);";
        }
    }
}
