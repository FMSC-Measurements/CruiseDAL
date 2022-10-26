namespace CruiseDAL.DownMigrators
{
    public class SaleDownMigrate : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
            return
$@"INSERT INTO {toDbName}.Sale (
    SaleNumber,
    Name,
    Purpose,
    Region,
    Forest,
    District,
    MeasurementYear,
    CalendarYear,
    LogGradingEnabled,
    Remarks,
    DefaultUOM,
    CreatedBy
) 
SELECT 
    s.SaleNumber,
    s.Name,
    c.Purpose,
    ifnull(s.Region, '00'),
    ifnull(s.Forest, '00'), 
    ifnull(s.District, '00'),
    c.MeasurementYear,
    CalendarYear,
    c.LogGradingEnabled,
    s.Remarks || ' ' || c.Remarks,
    c.DefaultUOM,
    '{createdBy}'
FROM {fromDbName}.Sale AS s
JOIN Cruise AS c USING (SaleNumber)
WHERE c.CruiseID = '{cruiseID}';
";
        }
    }
}
