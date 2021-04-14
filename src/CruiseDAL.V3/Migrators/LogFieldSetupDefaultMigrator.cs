using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Migrators
{
    public class LogFieldSetupDefaultMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
            var stratumDefaultID = Guid.NewGuid().ToString();

            return
$@"
INSERT INTO {toDbName}.StratumDefault (
    StratumDefaultID,
    Description
) VALUES (
    '{stratumDefaultID}',
    'Default Log Field Setup Profile'
);


INSERT INTO {toDbName}.LogFieldSetupDefault (
    StratumDefaultID,
    Field,
    FieldOrder
)
SELECT 
    '{stratumDefaultID}' AS StratumDefaultID,
    Field,
    FieldOrder
FROM {fromDbName}.LogFieldSetupDefault
JOIN {toDbName}.LogField USING (Field) -- join with LogField so that we only import V3 supported log fields
;";
        }
    }
}
