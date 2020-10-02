using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class FixCNTTallyClassMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
            return
$@"INSERT INTO {toDbName}.FixCNTTallyClass (
    FixCNTTallyClass_CN,
    CruiseID,
    StratumCode,
    Field
)
SELECT
    FixCNTTallyClass_CN,
    '{cruiseID}',
    st.Code AS StratumCode,
    FieldName
FROM {fromDbName}.FixCNTTallyClass
JOIN {fromDbName}.Stratum AS st USING (Stratum_CN);";
        }
    }
}
