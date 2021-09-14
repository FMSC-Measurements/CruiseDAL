using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.DownMigrators
{
    public class BiomassEquationDownMigrator : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
			return
$@"INSERT INTO {toDbName}.BiomassEquation (
    Species,
	Product,
	Component,
	LiveDead,
	FIAcode,
	Equation,
	PercentMoisture,
	PercentRemoved,
	MetaData,
	WeightFactorPrimary,
	WeightFactorSecondary
)
SELECT
    Species,
	Product,
	Component,
	LiveDead,
	FIAcode,
	Equation,
	PercentMoisture,
	PercentRemoved,
	MetaData,
	WeightFactorPrimary,
	WeightFactorSecondary
FROM {fromDbName}.BiomassEquation 
WHERE CruiseID = '{cruiseID}';
";
        }
    }
}
