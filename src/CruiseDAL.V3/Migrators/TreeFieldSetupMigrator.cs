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
        FieldOrder,
        Heading,
        Width
    )
    SELECT
        '{cruiseID}',
        st.Code AS StratumCode,
        tfs.Field,
        tfs.FieldOrder,
        tfs.Heading,
        tfs.Width
    FROM {fromDbName}.TreeFieldSetup AS tfs
    JOIN {fromDbName}.Stratum AS st USING (Stratum_CN)
    JOIN {toDbName}.TreeField AS tf USING (Field);";
        }
    }
}
