namespace CruiseDAL.DownMigrators
{
    public class ValueEquationDownMigrator : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
            return
$@"INSERT INTO {toDbName}.ValueEquation (
    Species,
	PrimaryProduct,
	ValueEquationNumber,
	Grade,
	Coefficient1,
	Coefficient2,
	Coefficient3,
	Coefficient4,
	Coefficient5,
	Coefficient6
)
SELECT
	Species,
	PrimaryProduct,
	ValueEquationNumber,
	Grade,
	Coefficient1,
	Coefficient2,
	Coefficient3,
	Coefficient4,
	Coefficient5,
	Coefficient6
FROM {fromDbName}.ValueEquation
WHERE CruiseID = '{cruiseID}';
";
        }
    }
}