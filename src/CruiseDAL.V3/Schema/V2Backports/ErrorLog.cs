namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_ERRORLOG_LEGACY =
@"CREATE TABLE tbl_ErrorLog (
    RowID INTEGER PRIMARY KEY AUTOINCREMENT,
    TableName TEXT NOT NULL,
    CN_Number INTEGER NOT NULL,
    ColumnName TEXT NOT NULL,
    Level TEXT NOT NULL,
    Message TEXT,
    Program TEXT,
    Suppress BOOLEAN Default 0,
    UNIQUE(TableName, CN_Number, ColumnName, Level)
);";

        public const string CREATE_VIEW_ERRORLOG =
@"CREATE VIEW ErrorLog AS
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
JOIN Tree_V3 AS t USING (TreeID)
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
JOIN Log_V3 AS l USING (LogID)

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

        public const string CREATE_TRIGGER_ERRORLOG_INSERT =
@"CREATE TRIGGER ErrorLog_Insert
INSTEAD OF INSERT ON ErrorLog
BEGIN
INSERT INTO tbl_ErrorLog (RowID, TableName, CN_Number, ColumnName, Level, Message, Program, Suppress)
VALUES (new.RowID, new.TableName, new.CN_Number, new.ColumnName, new.Level, new.Message, new.Program, new.Suppress);
END;";

        public const string CREATE_TRIGGER_ERRORLOG_UPDATE =
@"CREATE TRIGGER ErrorLog_Update
INSTEAD OF UPDATE ON ErrorLog
WHEN new.RowID > 0
BEGIN
UPDATE tbl_ErrorLog SET TableName = new.TableName, CN_Number = new.CN_Number, Level = new.Level, Message = new.Message, Program = new.Program, Suppress = new.Suppress
WHERE RowID = new.RowID;
END;";

        public const string CREATE_TRIGGER_ERRORLOG_DELETE =
@"CREATE TRIGGER ErrorLog_Delete
INSTEAD OF DELETE ON ErrorLog
WHEN old.RowID > 0
BEGIN
DELETE FROM tbl_ErrorLog WHERE RowID = old.RowID;
END;";
    }

    public partial class Migrations
    {
        public const string MIGRATE_ERRORLOG_FORMAT_STR =
            "INSERT INTO {0}.tbl_ErrorLog ( " +
                    "RowID, " +
                    "TableName, " +
                    "CN_Number, " +
                    "ColumnName, " +
                    "Level, " +
                    "Message, " +
                    "Program, " +
                    "Suppress " +
                ") " +
                "SELECT " +
                    "RowID, " +
                    "TableName, " +
                    "CN_Number, " +
                    "ColumnName, " +
                    "Level, " +
                    "Message, " +
                    "Program, " +
                    "Suppress " +
                "FROM {1}.ErrorLog " +
                "WHERE Level != 'W';";
    }
}