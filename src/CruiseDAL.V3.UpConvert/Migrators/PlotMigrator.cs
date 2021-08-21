using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class PlotMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
            return
$@"INSERT INTO {toDbName}.Plot ( 
    Plot_CN, 
    PlotID,
    PlotNumber,
    CruiseID,
    CuttingUnitCode,
    Slope,
    Aspect,
    Remarks,
    CreatedBy
)
SELECT
    p.Plot_CN,
    coalesce( -- we are going to check the if PlotID is valid otherwise generate a new guid
        (CASE typeof(Plot_GUID) COLLATE NOCASE -- ckeck the type of Plot_GUID
            WHEN 'TEXT' THEN -- if text
                (CASE WHEN Plot_GUID LIKE '________-____-____-____-____________' -- check to see if it is a properly formated guid
                    THEN nullif(Plot_GUID, '00000000-0000-0000-0000-000000000000') -- if not a empty guid return that value otherwise return null for now
                    ELSE NULL END) -- if it is not a properly formatted guid return Tree_GUID
            ELSE NULL END) -- if value is not a string return null
        , (hex( randomblob(4)) || '-' || hex( randomblob(2)) 
            || '-' || '4' || substr(hex(randomblob(2)), 2) || '-'
            || substr('AB89', 1 + (abs(random()) % 4), 1) ||
            substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6)))) AS PlotID,
    p.PlotNumber,
    '{cruiseID}',
    cu.Code AS CuttingUnitCode,
    p.Slope,
    p.Aspect,
    group_concat(p.Remarks) AS Remarks,
    '{deviceID}' AS CreatedBy
FROM {fromDbName}.Plot AS p
JOIN {fromDbName}.CuttingUnit AS cu USING (CuttingUnit_CN)
GROUP BY cu.Code, PlotNumber;";
        }
    }
}
