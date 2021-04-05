using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class CruiseMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
return $@"INSERT INTO {toDbName}.Cruise (
    CruiseID,
    SaleID,
    CruiseNumber,
    Purpose,
    DefaultUOM,
    LogGradingEnabled,
    UseCrossStrataPlotTreeNumbering,
    Remarks,
    CreatedBy
)
SELECT
    '{cruiseID}',
    '{saleID}',
    SaleNumber,
    Purpose,
    DefaultUOM,
    LogGradingEnabled,
    CASE WHEN Purpose = 'Recon' THEN 0 ELSE 1 END AS UseCrossStrataPlotTreeNumbering,
    Remarks,
    '{deviceID}'  AS CreatedBy
FROM {fromDbName}.Sale

UNION ALL

SELECT 
    '{cruiseID}',
    '{saleID}',
    '{saleID}' AS SaleNumber,
    NULL AS Purpose,
    NULL AS DefaultUOM,
    0 AS LogGradingEnabled,
    0 AS UseCrossStrataPlotTreeNumbering,
    NULL AS Remarks,
    '{deviceID}'  AS CreatedBy
WHERE NOT EXISTS (SELECT * FROM {fromDbName}.Sale);";
        }
    }
}
