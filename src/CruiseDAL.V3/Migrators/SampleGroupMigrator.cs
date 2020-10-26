using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class SampleGroupMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
            return
$@"INSERT INTO {toDbName}.SampleGroup (
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
    TallyMethod,
    Description,
    MinKPI,
    MaxKPI,
    SmallFPS,
    CreatedBy,
    Created_TS,
    ModifiedBy,
    Modified_TS
)
SELECT
    sg.SampleGroup_CN,
    '{cruiseID}',
    sg.Code AS SampleGroupCode,
    st.Code AS StratumCode,
    sg.CutLeave,
    sg.UOM,
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
    sg.TallyMethod,
    sg.Description,
    sg.MinKPI,
    sg.MaxKPI,
    sg.SmallFPS,
    sg.CreatedBy,
    sg.CreatedDate,
    sg.ModifiedBy,
    sg.ModifiedDate
FROM {fromDbName}.SampleGroup AS sg
JOIN {fromDbName}.Stratum AS st USING (Stratum_CN);";
        }
    }
}
