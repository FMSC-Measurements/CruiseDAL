using CruiseDAL.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Migrators
{
    public class TreeFieldSetupDefaultMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
            return
$@"INSERT INTO {toDbName}.StratumDefault (
    StratumDefaultID,
    Method
) 
SELECT
    (hex( randomblob(4)) || '-' || hex( randomblob(2)) 
        || '-' || '4' || substr(hex(randomblob(2)), 2) || '-'
        || substr('AB89', 1 + (abs(random()) % 4), 1) ||
        substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6))) AS StratumDefaultID,
    Method
FROM {fromDbName}.TreeFieldSetupDefault
GROUP BY Method;

INSERT INTO { toDbName}.TreeFieldSetupDefault (
    StratumDefaultID,
    Field,
    FieldOrder
)
SELECT
    (SELECT StratumDefaultID FROM { toDbName}.StratumDefault WHERE Method = tfsd.Method) AS StratumDefaultID,
    tfsd.Field,
    tfsd.FieldOrder
FROM { fromDbName}.TreeFieldSetupDefault AS tfsd
JOIN {toDbName}.TreeField AS tf USING (Field); -- join with TreeField so that we only import v3 supported tree fields

            ";
        }
    }
}
