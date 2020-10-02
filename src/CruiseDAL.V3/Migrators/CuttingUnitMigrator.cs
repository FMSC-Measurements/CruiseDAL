using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class CuttingUnitMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
return $@"INSERT INTO {toDbName}.CuttingUnit (
        CuttingUnit_CN,
        CruiseID,
        Code,
        Area,
        Description,
        LoggingMethod,
        PaymentUnit,
        Rx
    )
    SELECT
        CuttingUnit_CN,
        '{cruiseID}',
        Code,
        Area,
        Description,
        LoggingMethod,
        PaymentUnit,
        Rx
    FROM {fromDbName}.CuttingUnit;";
        }
    }
}
