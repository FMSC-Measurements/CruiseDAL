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
    'Tree' AS TableName,
    t.Tree_CN AS CN_Number, 
    te.Field AS ColumnName,
    te.Level,
    te.Message AS Message,
    'FScruiser' AS Program,
    te.IsResolved AS Suppress
FROM
    (
        SELECT 
            *,
            ROW_NUMBER() OVER (PARTITION BY TreeID, Field ORDER BY IsResolved ASC) AS Rank
        FROM {fromDbName}.TreeError 
        WHERE CruiseID = '{cruiseID}'
    ) AS te
JOIN {fromDbName}.Tree AS t USING (TreeID)
WHERE Rank = 1

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
    ifnull(pe.Plot_Stratum_CN, 0) AS CN_Number, -- need to put in 0 if null because in V2 CN_Number is required, but if error is due to missing Plot_Stratum CN is null
    ifnull(pe.Field, 'none') AS ColumnName,
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