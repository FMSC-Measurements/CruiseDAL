namespace CruiseDAL.DownMigrators
{
    public class LogDownMigrator : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
            return
$@"INSERT INTO {toDbName}.Log (
	Log_CN,
	Log_GUID,
	Tree_CN,
	LogNumber,
	Grade,
	SeenDefect,
	PercentRecoverable,
	Length,
	ExportGrade,
	SmallEndDiameter,
	LargeEndDiameter,
	GrossBoardFoot,
	NetBoardFoot,
	GrossCubicFoot,
	NetCubicFoot,
	BoardFootRemoved,
	CubicFootRemoved,
	DIBClass,
	BarkThickness,
	CreatedBy
)
SELECT
	Log_CN,
	LogID AS Log_GUID,
	t.Tree_CN,
	LogNumber,
	Grade,
	SeenDefect,
	PercentRecoverable,
	Length,
	ExportGrade,
	SmallEndDiameter,
	LargeEndDiameter,
	GrossBoardFoot,
	NetBoardFoot,
	GrossCubicFoot,
	NetCubicFoot,
	BoardFootRemoved,
	CubicFootRemoved,
	DIBClass,
	BarkThickness,
	'{createdBy}'
FROM {fromDbName}.Log
JOIN {fromDbName}.Tree AS t USING (TreeID)
WHERE t.CruiseID = '{cruiseID}'
;";
        }
    }
}