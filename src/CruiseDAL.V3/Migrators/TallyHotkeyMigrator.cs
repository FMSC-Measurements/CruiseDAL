using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class TallyHotkeyMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
            return
$@"WITH ctFlattened AS ( 
SELECT SampleGroup_CN, TreeDefaultValue_CN, max(Tally_CN) AS Tally_CN 
FROM {fromDbName}.CountTree 
GROUP BY SampleGroup_CN, ifnull(TreeDefaultValue_CN, '')) 

INSERT OR REPLACE INTO {toDbName}.TallyHotKey ( 
    CruiseID,
    StratumCode, 
    SampleGroupCode, 
    SpeciesCode, 
    LiveDead, 
    HotKey 
)
SELECT 
    '{cruiseID}',
    st.Code AS StratumCode, 
    sg.Code AS SampleGroupCode, 
    tdv.Species, 
    tdv.LiveDead, 
    t.HotKey 
FROM ctFlattened 
JOIN {fromDbName}.SampleGroup AS sg USING (SampleGroup_CN) 
JOIN {fromDbName}.Stratum AS st USING (Stratum_CN) 
JOIN {fromDbName}.Tally AS t USING (Tally_CN) 
LEFT JOIN {fromDbName}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN)
; ";
        }
    }
}
