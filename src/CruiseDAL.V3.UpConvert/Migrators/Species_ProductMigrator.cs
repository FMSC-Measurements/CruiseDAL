using CruiseDAL.Migrators;

namespace CruiseDAL.Migrators
{
    public class Species_ProductMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
            return
$@"

-- indicates how many contract species per species code


INSERT INTO {toDbName}.Species_Product (
    CruiseID,
    SpeciesCode,
    ContractSpecies
)  
SELECT '{cruiseID}',
    tdv.Species AS SpeciesCode,
    tdv.ContractSpecies
FROM {fromDbName}.TreeDefaultValue AS tdv
WHERE trim(ifnull(ContractSpecies, '')) != ''
GROUP BY Species 
HAVING count(DISTINCT ContractSpecies) = 1;

WITH ctrspPerSp AS (
    SELECT Species, count(*) AS cnt
    FROM (SELECT DISTINCT Species, ContractSpecies FROM {fromDbName}.TreeDefaultValue 
            WHERE trim(ifnull(ContractSpecies, '')) != '')
    GROUP BY Species
)

INSERT INTO {toDbName}.Species_Product (
    CruiseID,
    SpeciesCode,
    PrimaryProduct, 
    ContractSpecies
) 
SELECT '{cruiseID}',
    tdv.Species AS SpeciesCode,
    tdv.PrimaryProduct,
    tdv.ContractSpecies
FROM {fromDbName}.TreeDefaultValue AS tdv
JOIN ctrspPerSp USING (Species)
WHERE trim(ifnull(tdv.ContractSpecies, '')) != ''
 AND ctrspPerSp.cnt > 1 
GROUP BY Species, PrimaryProduct
HAVING count(DISTINCT ContractSpecies) = 1; -- ignore prods with multiple ctrSp
";
        }
    }
}
