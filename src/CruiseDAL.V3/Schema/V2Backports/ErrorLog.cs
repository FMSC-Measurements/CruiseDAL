namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_ERRORLOG =
@"CREATE VIEW ErrorLog_V2 AS
SELECT
    RowID,
    TableName,
    CN_Number,
    ColumnName,
    Level,
    Message,
    Program,
    Suppress
FROM tbl_ErrorLog
UNION ALL
SELECT
    -1 * ((((t.Tree_CN << 4) + (tf.TreeField_CN & 15)) << 4) + 1) AS RowID,
    'Tree' AS TableName,
    t.Tree_CN AS CN_Number,
    te.Field,
    te.Level,
    te.Message AS Message,
    'FScruiser' AS Program,
    te.IsResolved AS Suppress
FROM TreeError AS te
JOIN TreeField AS tf ON te.Field = tf.Field
JOIN Tree AS t USING (TreeID)
UNION ALL
SELECT
    -1 *(((le.Log_CN << 4) << 4) + 2) AS RowID,
    'Log' AS TableName,
    l.Log_CN AS CN_Number,
    'Grade' AS Field,
    'W' AS Level,
    le.Message AS Message,
    'FScruiser' AS Program,
    le.IsResolved AS Suppress
FROM LogGradeError AS le
JOIN Log AS l USING (LogID)

UNION ALL 
SELECT 
    -1 * (((pe.Plot_Stratum_CN << 4) << 4) + 3) AS RowID,
    'Plot' AS TableName,
    pe.Plot_Stratum_CN AS CN_Number, 
    pe.Field, 
    pe.Level, 
    pe.Message,
    'FScruiser' AS Program,
    pe.IsResolved AS Suppress
FROM PlotError AS pe;";
    }
}