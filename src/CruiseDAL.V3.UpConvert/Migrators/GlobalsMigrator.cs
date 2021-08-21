using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class GlobalsMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
            return
$@"INSERT OR IGNORE INTO {toDbName}.Globals ( -- dont overwrite existing global valus
        Block,
        Key,
        Value
    )
    SELECT
        Block,
        Key,
        Value
    FROM {fromDbName}.Globals;";
        }
    }
}
