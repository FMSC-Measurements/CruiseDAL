namespace CruiseDAL.DownMigrators
{
    public class ReportsDownMigrator : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
            return
$@"INSERT INTO {toDbName}.Reports (
    ReportID,
	Selected,
    Title
)
SELECT
    ReportID,
	Selected,
	Title
FROM {fromDbName}.Reports
WHERE CruiseID = '{cruiseID}';";
        }
    }
}