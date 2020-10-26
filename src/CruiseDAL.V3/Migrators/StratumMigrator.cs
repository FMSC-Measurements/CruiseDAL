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
    Created_TS,
    ModifiedBy,
    Modified_TS
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
    ModifiedDate
FROM {fromDbName}.Stratum;";
        }
    }
}
