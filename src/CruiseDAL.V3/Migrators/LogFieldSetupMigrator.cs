using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class LogFieldSetupMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
            return
$@"INSERT INTO {toDbName}.LogFieldSetup (
        StratumCode,
        CruiseID,
        Field,
        FieldOrder,
        Heading,
        Width
    )
    SELECT
        st.Code AS StratumCode,
        '{cruiseID}',
        lfs.Field,
        lfs.FieldOrder,
        lfs.Heading,
        lfs.Width
    FROM {fromDbName}.LogFieldSetup AS lfs
    JOIN {fromDbName}.Stratum AS st USING (Stratum_CN)
    JOIN {toDbName}.LogField USING (Field);"; // join with LogField so we only get valid fields
        }
    }
}
