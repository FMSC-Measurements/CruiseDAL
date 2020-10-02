using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class StratumMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
            return
$@"INSERT INTO {toDbName}.Stratum (
    StratumCode,
    CruiseID,
    Description,
    Method,
    BasalAreaFactor,
    FixedPlotSize,
    KZ3PPNT,
    SamplingFrequency,
    HotKey,
    FBSCode,
    YieldComponent,
    VolumeFactor,
    Month,
    Year,
    CreatedBy,
    CreatedDate,
    ModifiedBy,
    ModifiedDate,
    RowVersion
)
SELECT
    Code,
    '{cruiseID}',
    Description,
    Method,
    BasalAreaFactor,
    FixedPlotSize,
    KZ3PPNT,
    SamplingFrequency,
    HotKey,
    FBSCode,
    YieldComponent,
    VolumeFactor,
    Month,
    Year,
    CreatedBy,
    CreatedDate,
    ModifiedBy,
    ModifiedDate,
    RowVersion
FROM {fromDbName}.Stratum;";
        }
    }
}
