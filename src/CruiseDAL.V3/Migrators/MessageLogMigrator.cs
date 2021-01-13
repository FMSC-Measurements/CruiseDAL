using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class MessageLogMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
            return
$@"INSERT INTO {toDbName}.MessageLog (
        Program,
        Date,
        Time,
        Level
    )
    SELECT
        Program,
        Date,
        Time,
        Level
    FROM {fromDbName}.MessageLog;";
        }
    }
}
