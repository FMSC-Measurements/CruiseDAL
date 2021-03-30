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
    CruiseNumber,
    Purpose,
    DefaultUOM,
    LogGradingEnabled,
    UseCrossStrataPlotTreeNumbering,
    Remarks,
    CreatedBy,
    Created_TS,
    ModifiedBy,
    Modified_TS
)
SELECT
    '{cruiseID}',
    '{saleID}',
    SaleNumber,
    Purpose,
    DefaultUOM,
    LogGradingEnabled,
    CASE WHEN Purpose = 'Recon' THEN 0 ELSE 1 END,
    Remarks,
    CreatedBy,
    CreatedDate,
    ModifiedBy,
    ModifiedDate
FROM {fromDbName}.Sale;";
        }
    }
}
