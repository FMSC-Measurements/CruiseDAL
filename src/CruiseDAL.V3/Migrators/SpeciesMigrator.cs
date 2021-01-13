using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class SpeciesMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
            return
$@"INSERT INTO {toDbName}.Species ( 
    SpeciesCode,
    CruiseID,
    ContractSpecies
) 
SELECT Species,  '{cruiseID}', ContractSpecies FROM {fromDbName}.TreeDefaultValue GROUP BY Species;";
        }
    }
}
