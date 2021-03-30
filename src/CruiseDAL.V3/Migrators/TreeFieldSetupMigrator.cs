using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class TreeFieldSetupMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
            return
$@"INSERT INTO {toDbName}.TreeFieldSetup (
    CruiseID,
    StratumCode,
    Field,
    FieldOrder
)
SELECT
    '{cruiseID}',
    st.Code AS StratumCode,
    tfs.Field,
    tfs.FieldOrder
FROM {fromDbName}.TreeFieldSetup AS tfs
JOIN {fromDbName}.Stratum AS st USING (Stratum_CN)
JOIN {toDbName}.TreeField AS tf USING (Field); -- join with TreeField so that we only import v3 tree fields

INSERT INTO {toDbName}.TreeFieldHeading (
    CruiseID,
    Field,
    Heading
)
SELECT
    '{cruiseID}',
    Field,
    min(Heading)
FROM { fromDbName}.TreeFieldSetup AS tfs
JOIN {toDbName}.TreeField AS tf USING (Field)
WHERE length(Heading) > 1
GROUP BY Field; ";
        }
    }
}
