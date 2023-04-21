using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class SpeciesMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {


            return
$@"INSERT INTO {toDbName}.Species ( 
    SpeciesCode,
    CruiseID
) 
SELECT Species,  '{cruiseID}' FROM {fromDbName}.TreeDefaultValue GROUP BY Species;


WITH sp_fia AS (
    SELECT sp.SpeciesCode, tdv.FIACode
    FROM {toDbName}.Species AS sp
    JOIN ( SELECT Species, FIACode 
                FROM (SELECT DISTINCT Species, FIACode FROM {fromDbName}.TreeDefaultValue)
                GROUP BY Species
                HAVING count(*) = 1 ) AS tdv
    ON sp.SpeciesCode = tdv.Species
)
                
UPDATE {toDbName}.Species as toSp SET 
    FIACode = (SELECT FIACode FROM sp_fia  WHERE sp_fia.SpeciesCode = toSp.SpeciesCode)
WHERE 
    EXISTS (SELECT SpeciesCode FROM sp_fia WHERE sp_fia.SpeciesCode = toSp.SpeciesCode);
";
        }
    }
}
