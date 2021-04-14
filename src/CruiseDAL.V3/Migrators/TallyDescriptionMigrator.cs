using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class TallyDescriptionMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
            return
$@"WITH ctFlattened AS ( 
SELECT SampleGroup_CN, TreeDefaultValue_CN, max(Tally_CN) AS Tally_CN 
FROM {fromDbName}.CountTree AS ct
-- we need to join with SGTDV to prefent fKey fails with SubPopulation, 
JOIN {fromDbName}.SampleGroupTreeDefaultValue AS sgtdv USING (SampleGroup_CN, TreeDefaultValue_CN)
WHERE ct.TreeDefaultValue_CN NOTNULL
GROUP BY SampleGroup_CN, TreeDefaultValue_CN

UNION ALL 

SELECT SampleGroup_CN, NULL AS TreeDefaultValue_CN, max(Tally_CN) AS Tally_CN 
FROM {fromDbName}.CountTree AS ct
WHERE ct.TreeDefaultValue_CN IS NULL
GROUP BY SampleGroup_CN

) 

INSERT OR REPLACE INTO {toDbName}.TallyDescription ( 
    CruiseID, 
    StratumCode, 
    SampleGroupCode, 
    SpeciesCode, 
    LiveDead, 
    Description 
)
SELECT 
    '{cruiseID}', 
    st.Code AS StratumCode, 
    sg.Code AS SampleGroupCode,
    tdv.Species,
    tdv.LiveDead,
    t.Description
FROM ctFlattened
JOIN {fromDbName}.SampleGroup AS sg USING (SampleGroup_CN)
JOIN {fromDbName}.Stratum AS st USING (Stratum_CN)
JOIN {fromDbName}.Tally AS t USING (Tally_CN)
LEFT JOIN {fromDbName}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN); ";
        }
    }
}
