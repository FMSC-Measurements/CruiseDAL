﻿using System;
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
    Purpose,
    Region,
    Forest,
    District,
    MeasurementYear,
    CalendarYear,
    LogGradingEnabled,
    Remarks,
    DefaultUOM,
    CreatedBy,
    CreatedDate,
    ModifiedBy,
    ModifiedDate,
    RowVersion
)
SELECT
    '{saleID}',
    Sale_CN,
    SaleNumber,
    Name,
    Purpose,
    Region,
    Forest,
    District,
    MeasurementYear,
    CalendarYear,
    LogGradingEnabled,
    Remarks,
    DefaultUOM,
    CreatedBy,
    CreatedDate,
    ModifiedBy,
    ModifiedDate,
    RowVersion
FROM {fromDbName}.Sale;";
        }
    }
}