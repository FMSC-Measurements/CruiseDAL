using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class CruiseMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
return $@"INSERT INTO {toDbName}.Cruise (
    CruiseID,
    SaleID,
    Purpose,
    DefaultUOM,
    LogGradingEnabled,
    Remarks,
    CreatedBy,
    Created_TS,
    ModifiedBy,
    Modified_TS
)
SELECT
    '{cruiseID}',
    '{saleID}',
    Purpose,
    DefaultUOM,
    LogGradingEnabled,
    Remarks,
    CreatedBy,
    CreatedDate,
    ModifiedBy,
    ModifiedDate
FROM {fromDbName}.Sale;";
        }
    }
}
