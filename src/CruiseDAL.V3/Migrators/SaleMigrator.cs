using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class SaleMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
            return
$@"INSERT INTO {toDbName}.Sale (
    SaleID,
    SaleNumber,
    Name,
    Region,
    Forest,
    District,
    Remarks,
    DefaultUOM,
    CreatedBy
)
SELECT
    '{saleID}',
    SaleNumber,
    Name,
    trim(Region) AS Region,
    trim(Forest) AS Forest,
    District,
    Remarks,
    DefaultUOM,
    '{deviceID}'
FROM {fromDbName}.Sale

UNION ALL

SELECT 
    '{saleID}',
    '{saleID}' AS SaleNumber,
    NULL AS Name,
    NULL AS Region,
    NULL AS Forest,
    NULL AS District,
    NULL AS Remarks,
    NULL AS DefaultUOM,
    '{deviceID}'
WHERE NOT EXISTS (SELECT * FROM {fromDbName}.Sale);
;";
        }
    }
}
