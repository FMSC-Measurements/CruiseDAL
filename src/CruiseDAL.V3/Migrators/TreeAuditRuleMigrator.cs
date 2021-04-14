using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class TreeAuditRuleMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
            return
$@"INSERT INTO {toDbName}.TreeAuditRule (
    TreeAuditRule_CN,
    TreeAuditRuleID,
    CruiseID, 
    Field,
    Min,
    Max
)
SELECT
    TreeAuditValue_CN,
    (hex( randomblob(4)) || '-' || hex( randomblob(2)) 
        || '-' || '4' || substr(hex(randomblob(2)), 2) || '-' 
        || substr('AB89', 1 + (abs(random()) % 4), 1) || 
        substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6))),
    '{cruiseID}',
    Field,
    nullif(Min,0) AS Min,
    nullif(Max,0) AS Max
FROM {fromDbName}.TreeAuditValue;";
        }
    }
}
