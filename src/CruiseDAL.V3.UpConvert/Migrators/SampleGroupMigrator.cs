using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class SampleGroupMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
            return
$@"INSERT INTO {toDbName}.SampleGroup (
    SampleGroupID,
    SampleGroup_CN,
    CruiseID,
    SampleGroupCode,
    StratumCode,
    CutLeave,
    UOM,
    PrimaryProduct,
    SecondaryProduct,
    BiomassProduct,
    DefaultLiveDead,
    SamplingFrequency,
    InsuranceFrequency,
    KZ,
    BigBAF,
    TallyBySubPop,
    UseExternalSampler,
    SampleSelectorType,
    Description,
    MinKPI,
    MaxKPI,
    SmallFPS,
    CreatedBy
)
SELECT
    (hex( randomblob(4)) || '-' || hex( randomblob(2)) 
            || '-' || '4' || substr(hex(randomblob(2)), 2) || '-'
            || substr('AB89', 1 + (abs(random()) % 4), 1) ||
            substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6))) AS SampleGroupID,
    sg.SampleGroup_CN,
    '{cruiseID}',
    sg.Code AS SampleGroupCode,
    st.Code AS StratumCode,
    sg.CutLeave,
    trim(sg.UOM) AS UOM,
    sg.PrimaryProduct,
    sg.SecondaryProduct,
    sg.BiomassProduct,
    sg.DefaultLiveDead,
    sg.SamplingFrequency,
    sg.InsuranceFrequency,
    sg.KZ,
    sg.BigBAF,
    (EXISTS (
            SELECT * FROM {fromDbName}.CountTree AS ct
            WHERE ct.SampleGroup_CN = sg.SampleGroup_CN AND ct.TreeDefaultValue_CN NOT NULL
        )) AS TallyBySubPop,
    (CASE WHEN sg.SampleSelectorType = 'ClickerSelecter' THEN 1 ELSE 0 END) AS UseExternalSampler,
    CASE WHEN sg.SampleSelectorType IN ('SystematicSelecter', 'BlockSelecter', 'ClickerSelecter') THEN sg.SampleSelectorType ELSE NULL END,
    sg.Description,
    sg.MinKPI,
    sg.MaxKPI,
    sg.SmallFPS,
    '{deviceID}' AS CreatedBy
FROM {fromDbName}.SampleGroup AS sg
JOIN {fromDbName}.Stratum AS st USING (Stratum_CN);";
        }
    }
}
