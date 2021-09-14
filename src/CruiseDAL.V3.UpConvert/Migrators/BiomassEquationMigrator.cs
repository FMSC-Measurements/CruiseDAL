namespace CruiseDAL.Migrators
{
    public class BiomassEquationMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
            return
$@"INSERT INTO {toDbName}.BiomassEquation (
    CruiseID,
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
	WeightFactorSecondary,
    CreatedBy
)
SELECT
    '{cruiseID}',
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
	WeightFactorSecondary,
    '{deviceID}'
FROM {fromDbName}.BiomassEquation;
";
        }
    }
}