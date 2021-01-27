namespace CruiseDAL.DownMigrators
{
    public class CuttingUnitDownMigrate : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
            return
$@"INSERT INTO CuttingUnit (
    CuttingUnit_CN,
    Code,
    Area,
    Description,
    LoggingMethod,
    PaymentUnit,
    Rx,
    CreatedBy
)
SELECT
    CuttingUnit_CN,
    CuttingUnitCode,
    Area,
    Description,
    LoggingMethod,
    PaymentUnit,
    Rx,
    '{createdBy}'
FROM {fromDbName}.CuttingUnit
WHERE CruiseID = '{cruiseID}';
";
        }
    }
}