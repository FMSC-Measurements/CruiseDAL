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
    StratumID,
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
    CreatedBy,
    Created_TS,
    ModifiedBy,
    Modified_TS
)
SELECT
    (hex( randomblob(4)) || '-' || hex( randomblob(2)) 
            || '-' || '4' || substr(hex(randomblob(2)), 2) || '-'
            || substr('AB89', 1 + (abs(random()) % 4), 1) ||
            substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6))) AS StratumID,
    st.Code,
    '{cruiseID}',
    st.Description,
    CASE WHEN st.Method = 'PCMTRE' THEN 'PCM' ELSE st.Method END,
    st.BasalAreaFactor,
    st.FixedPlotSize,
    st.KZ3PPNT,
    st.SamplingFrequency,
    st.HotKey,
    st.FBSCode,
    st.YieldComponent,
    tc.FieldName,
    st.CreatedBy,
    st.CreatedDate,
    st.ModifiedBy,
    st.ModifiedDate
FROM {fromDbName}.Stratum AS st
LEFT JOIN {fromDbName}.FixCNTTallyClass AS tc USING (Stratum_CN);";
        }
    }
}
