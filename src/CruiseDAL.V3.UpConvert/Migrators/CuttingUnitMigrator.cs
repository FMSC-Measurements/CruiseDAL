using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class CuttingUnitMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
return $@"INSERT INTO {toDbName}.CuttingUnit (
        CuttingUnitID,
        CuttingUnit_CN,
        CruiseID,
        CuttingUnitCode,
        Area,
        Description,
        LoggingMethod,
        PaymentUnit,
        Rx, 
        CreatedBy
    )
    SELECT
        (hex( randomblob(4)) || '-' || hex( randomblob(2)) 
            || '-' || '4' || substr(hex(randomblob(2)), 2) || '-'
            || substr('AB89', 1 + (abs(random()) % 4), 1) ||
            substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6))) AS CuttringUnitID,
        CuttingUnit_CN,
        '{cruiseID}',
        Code,
        Area,
        Description,
        trim(LoggingMethod) AS LoggingMethod,
        PaymentUnit,
        Rx,
        '{deviceID}' AS CreatedBy
    FROM {fromDbName}.CuttingUnit;";
        }
    }
}
