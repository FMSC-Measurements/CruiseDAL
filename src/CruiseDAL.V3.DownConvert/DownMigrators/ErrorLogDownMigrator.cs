namespace CruiseDAL.DownMigrators
{
    public class ErrorLogDownMigrator : IDownMigrator
    {
        public string CreateCommand(string toDbName, string fromDbName, string cruiseID, string createdBy)
        {
            return
$@"INSERT INTO {toDbName}.ErrorLog (
    TableName,
    CN_Number,
    ColumnName,
    Level,
    Message,
    Program,
    Suppress
)
SELECT
    -- -1 * ((((t.Tree_CN << 4) + (tf.TreeField_CN & 15)) << 4) + 1) AS RowID,
    'Tree' AS TableName,
    t.Tree_CN AS CN_Number,
    te.Field AS ColumnName,
    te.Level,
    te.Message AS Message,
    'FScruiser' AS Program,
    te.IsResolved AS Suppress
FROM {fromDbName}.TreeError AS te
JOIN {fromDbName}.Tree AS t USING (TreeID)
WHERE te.CruiseID = '{cruiseID}'

UNION ALL

SELECT
    -- -1 *(((le.Log_CN << 4) << 4) + 2) AS RowID,
    'Log' AS TableName,
    l.Log_CN AS CN_Number,
    'Grade' AS ColumnName,
    'W' AS Level,
    le.Message AS Message,
    'FScruiser' AS Program,
    le.IsResolved AS Suppress
FROM {fromDbName}.LogGradeError AS le
JOIN {fromDbName}.Log AS l USING (LogID)
WHERE le.CruiseID = '{cruiseID}'

UNION ALL

SELECT
    -- -1 * (((pe.Plot_Stratum_CN << 4) << 4) + 3) AS RowID,
    'Plot' AS TableName,
    pe.Plot_Stratum_CN AS CN_Number,
    pe.Field AS ColumnName,
    pe.Level,
    pe.Message,
    'FScruiser' AS Program,
    pe.IsResolved AS Suppress
FROM {fromDbName}.PlotError AS pe
WHERE pe.CruiseID = '{cruiseID}'
;";
        }
    }
}