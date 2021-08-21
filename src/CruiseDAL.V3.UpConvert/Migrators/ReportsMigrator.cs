using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class ReportsMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
            // todo
            return
$@"INSERT INTO {toDbName}.Reports (
    ReportID,
    CruiseID,
    Selected,
    Title,
    CreatedBy
)
SELECT 
    ReportID,
    '{cruiseID}',
    Selected,
    Title,
    '{deviceID}' AS CreatedBy
FROM {fromDbName}.Reports;";
        }
    }
}
