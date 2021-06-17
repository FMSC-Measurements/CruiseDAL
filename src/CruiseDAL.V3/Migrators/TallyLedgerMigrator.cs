using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class TallyLedgerMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
            return
                // migrate counts from CountTree table
$@"INSERT INTO {toDbName}.TallyLedger (
    TallyLedgerID,
    CruiseID,
    CuttingUnitCode,
    StratumCode,
    SampleGroupCode,
    SpeciesCode,
    LiveDead,
    TreeCount,
    KPI,
    EntryType,
    CreatedBy
)
SELECT
    'initFromCountTree-' || cu.Code || ',' || st.Code || ',' || sg.Code || ',' || ifnull(tdv.Species, 'null') || ',' || ifnull(tdv.LiveDead, 'null') || ',' || ifnull(Component_CN, 'master'),
    '{cruiseID}',
    cu.Code AS CuttingUnitCode,
    st.Code AS StratumCode,
    sg.Code AS SampleGroupCode,
    tdv.Species AS SpeciesCode,
    tdv.LiveDead AS LiveDead,
    Sum(ct.TreeCount) AS TreeCount,
    Sum(ct.SumKPI) AS SumKPI,
    'utility' AS EntryType,
    '{deviceID}' AS CreatedBy
FROM {fromDbName}.CountTree AS ct
JOIN {fromDbName}.CuttingUnit AS cu USING (CuttingUnit_CN)
JOIN {fromDbName}.SampleGroup AS sg USING (SampleGroup_CN)
JOIN {fromDbName}.Stratum AS st USING (Stratum_CN)
LEFT JOIN {fromDbName}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN)
GROUP BY
    cu.Code,
    st.Code,
    sg.Code,
    ifnull(tdv.Species, ''),
    ifnull(tdv.LiveDead, ''),
    ifnull(ct.Component_CN, 0);
" +

// migrate counts from the Tree table
$@"INSERT INTO {toDbName}.TallyLedger(

   TallyLedgerID,
   CruiseID,
   TreeID,
   CuttingUnitCode,
   StratumCode,
   SampleGroupCode,
   PlotNumber,
   SpeciesCode,
   LiveDead,
   TreeCount,
   KPI,
   STM,
   EntryType,
   CreatedBy
)
SELECT
    'migrateFromTree-' || t.Tree_CN,
    '{cruiseID}',
    t3.TreeID,
    cu.Code AS CuttingUnitCode,
    st.Code AS StratumCode,
    sg.Code AS SampleGroupCode,
    ps.PlotNumber AS PlotNumber,
    tdv.Species AS SpeciesCode,
    tdv.LiveDead AS LiveDead,
    t.TreeCount,
    t.KPI AS KPI,
    CASE t.STM COLLATE NOCASE WHEN 'Y' THEN 1 WHEN 'N' THEN 0 ELSE 0 END AS STM,
    'utility' AS EntryType,
    '{deviceID}' AS CreatedBy
FROM {fromDbName}.Tree AS t
JOIN {fromDbName}.CuttingUnit AS cu USING (CuttingUnit_CN)
JOIN {fromDbName}.Stratum AS st USING (Stratum_CN)
JOIN {fromDbName}.SampleGroup AS sg USING (SampleGroup_CN)
JOIN {toDbName}.Tree AS t3 USING (Tree_CN)
LEFT JOIN {toDbName}.Plot_Stratum AS ps ON t.Plot_CN = ps.Plot_Stratum_CN
LEFT JOIN {fromDbName}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN)
WHERE t.TreeCount > 0 OR t.KPI > 0 OR t.CountOrMeasure = 'M' OR t.CountOrMeasure = 'I';";
        }
    }
}
