using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class TreeMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
            return
$@"INSERT INTO {toDbName}.Tree (
        Tree_CN,
        CruiseID,
        TreeID,
        CuttingUnitCode,
        StratumCode,
        SampleGroupCode,
        SpeciesCode,
        LiveDead,
        PlotNumber,
        TreeNumber,
        CountOrMeasure,
        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS
    )
    SELECT
        t.Tree_CN,
        '{cruiseID}',
        ifnull(
            (CASE typeof(Tree_GUID) COLLATE NOCASE -- ckeck the type of Tree_GUID
                WHEN 'TEXT' THEN -- if text
                    (CASE WHEN Tree_GUID LIKE '________-____-____-____-____________' -- check to see if it is a properly formated guid
                        THEN nullif(Tree_GUID, '00000000-0000-0000-0000-000000000000') -- if not a empty guid return that value otherwise return null for now
                        ELSE NULL END) -- if it is not a properly formatted guid return Tree_GUID
                ELSE NULL END) -- if value is not a string return null
            , (hex( randomblob(4)) || '-' || hex( randomblob(2)) 
                    || '-' || '4' || substr(hex(randomblob(2)), 2) || '-' 
                    || substr('AB89', 1 + (abs(random()) % 4), 1) || 
                    substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6)))) AS TreeID, -- if value is null sofar generate guid
        cu.Code AS CuttingUnitCode,
        st.Code AS StratumCode,
        sg.Code AS SampleGroupCode,
        tdv.Species AS SpeciesCode,
        t.LiveDead, -- use livedead from tree instead of tdv, because that is the value used by cruise processing
        p.PlotNumber, 
        t.TreeNumber,
        t.CountOrMeasure,
        t.CreatedBy,
        t.CreatedDate,
        t.ModifiedBy,
        t.ModifiedDate
    FROM {fromDbName}.Tree as t
    JOIN {fromDbName}.CuttingUnit AS cu USING (CuttingUnit_CN)
    JOIN {fromDbName}.Stratum AS st USING (Stratum_CN)
    JOIN {fromDbName}.SampleGroup AS sg USING (SampleGroup_CN)
    LEFT JOIN {fromDbName}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN) 
    LEFT JOIN {fromDbName}.Plot AS p USING (Plot_CN);";
        }
    }
}
