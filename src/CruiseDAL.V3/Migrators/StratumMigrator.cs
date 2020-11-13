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
    FixCNTField, 
    VolumeFactor,
    Month,
    Year,
    CreatedBy,
    Created_TS,
    ModifiedBy,
    Modified_TS
)
SELECT
    st.Code,
    st.'{cruiseID}',
    st.Description,
    st.Method,
    st.BasalAreaFactor,
    st.FixedPlotSize,
    st.KZ3PPNT,
    st.SamplingFrequency,
    st.HotKey,
    st.FBSCode,
    st.YieldComponent,
    tc.Field,
    st.VolumeFactor,
    st.Month,
    st.Year,
    st.CreatedBy,
    st.CreatedDate,
    st.ModifiedBy,
    st.ModifiedDate
FROM {fromDbName}.Stratum AS st
LEFT JOIN FixCNTTallyClass AS tc USING (Stratum_CN);";
        }
    }
}
