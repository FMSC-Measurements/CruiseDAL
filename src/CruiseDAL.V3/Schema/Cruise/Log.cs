﻿using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class LogTableDefinition : ITableDefinition
    {
        public string TableName => "Log";

        public string CreateTable =>
@"CREATE TABLE Log (
    Log_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    LogID TEXT NOT NULL,
    TreeID TEXT NOT NULL,
    LogNumber TEXT NOT NULL,
    Grade TEXT COLLATE NOCASE,
    SeenDefect REAL Default 0.0,
    PercentRecoverable REAL Default 0.0,
    Length INTEGER Default 0,
    ExportGrade TEXT,
    SmallEndDiameter REAL Default 0.0,
    LargeEndDiameter REAL Default 0.0,
    GrossBoardFoot REAL Default 0.0,
    NetBoardFoot REAL Default 0.0,
    GrossCubicFoot REAL Default 0.0,
    NetCubicFoot REAL Default 0.0,
    BoardFootRemoved REAL Default 0.0,
    CubicFootRemoved REAL Default 0.0,
    DIBClass REAL Default 0.0,
    BarkThickness REAL Default 0.0,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    UNIQUE (LogID),
    UNIQUE (TreeID, LogNumber),

    CHECK (LogID LIKE '________-____-____-____-____________'),

    FOREIGN KEY (TreeID) REFERENCES Tree (TreeID) ON DELETE CASCADE
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE Log_Tombstone (
    LogID TEXT NOT NULL,
    TreeID TEXT NOT NULL,
    LogNumber TEXT NOT NULL,
    Grade TEXT COLLATE NOCASE,
    SeenDefect REAL,
    PercentRecoverable REAL,
    Length INTEGER,
    ExportGrade TEXT,
    SmallEndDiameter REAL,
    LargeEndDiameter REAL,
    GrossBoardFoot REAL,
    NetBoardFoot REAL,
    GrossCubicFoot REAL,
    NetCubicFoot REAL,
    BoardFootRemoved REAL,
    CubicFootRemoved REAL,
    DIBClass REAL,
    BarkThickness REAL,
    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
    Deleted_TS DATETIME
);

CREATE INDEX NIX_Log_Tombstone_TreeID_LogNumber ON Log_Tombstone 
(TreeID, LogNumber);

CREATE INDEX NIX_Log_Tombstone_LogID ON Log_Tombstone
(LogID);
";

        public string CreateIndexes =>
@"CREATE INDEX Log_LogNumber ON Log (LogNumber);
CREATE INDEX Log_TreeID ON Log (TreeID);";

        public IEnumerable<string> CreateTriggers => new[] 
        {
            CREATE_TRIGGER_LOG_ONUPDATE, 
            CREATE_TRIGGER_Log_OnDelete 
        };

        public const string CREATE_TRIGGER_LOG_ONUPDATE =
@"CREATE TRIGGER Log_OnUpdate 
AFTER UPDATE OF
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
    BarkThickness
ON Log
FOR EACH ROW
BEGIN
    UPDATE Log SET Modified_TS = CURRENT_TIMESTAMP WHERE Log_CN = old.Log_CN;
END;";

        public const string CREATE_TRIGGER_Log_OnDelete =
@"CREATE TRIGGER Log_OnDelete
BEFORE DELETE ON Log
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO Log_Tombstone (
        LogID,
        TreeID,
        LogNumber,
        Grade,
        SeenDefect,
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
        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS,
        Deleted_TS
    ) VALUES (
        OLD.LogID,
        OLD.TreeID,
        OLD.LogNumber,
        OLD.Grade,
        OLD.SeenDefect,
        OLD.SeenDefect,
        OLD.PercentRecoverable,
        OLD.Length,
        OLD.ExportGrade,
        OLD.SmallEndDiameter,
        OLD.LargeEndDiameter,
        OLD.GrossBoardFoot,
        OLD.NetBoardFoot,
        OLD.GrossCubicFoot,
        OLD.NetCubicFoot,
        OLD.BoardFootRemoved,
        OLD.CubicFootRemoved,
        OLD.DIBClass,
        OLD.BarkThickness,
        OLD.CreatedBy,
        OLD.Created_TS,
        OLD.ModifiedBy,
        OLD.Modified_TS,
        CURRENT_TIMESTAMP
    );
END;";
    }
}