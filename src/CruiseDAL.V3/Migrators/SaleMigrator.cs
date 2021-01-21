using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class SaleMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
            return
$@"INSERT INTO {toDbName}.Sale (
    SaleID,
    Sale_CN,
    SaleNumber,
    Name,
    Region,
    Forest,
    District,
    Remarks,
    DefaultUOM,
    CreatedBy,
    Created_TS,
    ModifiedBy,
    Modified_TS
)
SELECT
    '{saleID}',
    Sale_CN,
    SaleNumber,
    Name,
    trim(Region) AS Region,
    trim(Forest) AS Forest,
    District,
    Remarks,
    DefaultUOM,
    CreatedBy,
    CreatedDate,
    ModifiedBy,
    ModifiedDate
FROM {fromDbName}.Sale;";
        }
    }
}
