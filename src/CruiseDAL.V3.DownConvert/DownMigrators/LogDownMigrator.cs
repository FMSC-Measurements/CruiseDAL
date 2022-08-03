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
	ifnull(SeenDefect, 0.0) AS SeenDefect,
	ifnull(PercentRecoverable, 0.0) AS PercentRecoverable,
	ifnull(Length, 0) AS Length,
	ExportGrade,
	ifnull(SmallEndDiameter, 0.0) AS SmallEndDiameter,
	ifnull(LargeEndDiameter, 0.0) AS LargeEndDiameter,
	ifnull(GrossBoardFoot, 0.0) AS GrossBoardFoot,
	ifnull(NetBoardFoot, 0.0) AS NetBoardFoot,
	ifnull(GrossCubicFoot, 0.0) AS GrossCubicFoot,
	ifnull(NetCubicFoot, 0.0) AS NetCubicFoot,
	ifnull(BoardFootRemoved, 0.0) AS BoardFootRemoved,
	ifnull(CubicFootRemoved, 0.0) AS CubicFootRemoved,
	ifnull(DIBClass, 0.0) AS DIBClass,
	ifnull(BarkThickness, 0.0) AS BarkThickness,
	'{createdBy}'
FROM {fromDbName}.Log
JOIN {fromDbName}.Tree AS t USING (TreeID)
WHERE t.CruiseID = '{cruiseID}'
;";
        }
    }
}