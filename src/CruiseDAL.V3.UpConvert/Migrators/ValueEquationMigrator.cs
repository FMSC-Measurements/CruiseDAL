namespace CruiseDAL.Migrators
{
    public class ValueEquationMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID, string deviceID)
        {
            return
$@"INSERT INTO {toDbName}.ValueEquation (
    CruiseID,
    Species,
    PrimaryProduct,
    ValueEquationNumber,
    Grade,
    Coefficient1,
    Coefficient2,
    Coefficient3,
    Coefficient4,
    Coefficient5,
    Coefficient6,
    CreatedBy
)
SELECT
    '{cruiseID}',
    Species,
    PrimaryProduct,
    ValueEquationNumber,
    Grade,
    Coefficient1,
    Coefficient2,
    Coefficient3,
    Coefficient4,
    Coefficient5,
    Coefficient6,
    '{deviceID}'
FROM {fromDbName}.ValueEquation;
";
        }
    }
}