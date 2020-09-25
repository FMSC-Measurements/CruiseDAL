namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_LOG =
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
    CreatedBy TEXT DEFAULT '',
    CreatedDate DateTime DEFAULT (datetime('now', 'localtime')),
    ModifiedBy TEXT ,
    ModifiedDate DateTime ,
    RowVersion INTEGER DEFAULT 0,

    UNIQUE (LogID),
    UNIQUE (TreeID, LogNumber),

    CHECK (LogID LIKE '________-____-____-____-____________'),

    FOREIGN KEY (TreeID) REFERENCES Tree (TreeID) ON DELETE CASCADE
);";

        public const string CREATE_INDEX_Log_LogNumber =
            "CREATE INDEX Log_LogNumber ON Log (LogNumber);";

        public const string CREATE_INDEX_Log_TreeID =
            @"CREATE INDEX Log_TreeID ON Log (TreeID);";

        public const string CREATE_TRIGGER_LOG_ONUPDATE =
            "CREATE TRIGGER Log_OnUpdate " +
            "AFTER UPDATE OF " +
                "LogNumber, " +
                "Grade, " +
                "SeenDefect, " +
                "PercentRecoverable, " +
                "Length, " +
                "ExportGrade, " +
                "SmallEndDiameter, " +
                "LargeEndDiameter, " +
                "GrossBoardFoot, " +
                "NetBoardFoot, " +
                "GrossCubicFoot, " +
                "NetCubicFoot, " +
                "BoardFootRemoved, " +
                "CubicFootRemoved, " +
                "DIBClass, " +
                "BarkThickness " +
            "ON Log " +
            "FOR EACH ROW " +
            "BEGIN " +
                "UPDATE Log SET RowVersion = old.RowVersion + 1 WHERE Log_CN = old.Log_CN; " +
                "UPDATE Log SET ModifiedDate = datetime('now', 'localtime') WHERE Log_CN = old.Log_CN; " +
            "END;";

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
        GrossCubicFoot
        NetCubicFoot,
        BoardFootRemoved,
        CubicFootRemoved,
        DIBClass,
        BarkThickness,
        CreatedBy,
        CreatedDate,
        ModifiedBy,
        ModifiedDate
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
        OLD.GrossCubicFoot
        OLD.NetCubicFoot,
        OLD.BoardFootRemoved,
        OLD.CubicFootRemoved,
        OLD.DIBClass,
        OLD.BarkThickness,
        OLD.CreatedBy,
        OLD.CreatedDate,
        OLD.ModifiedBy,
        OLD.ModifiedDate
    );
END;";

        public const string CREATE_TOMBSTONE_TABLE_Log_Tombstone =
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
    CreatedDate DateTime,
    ModifiedBy TEXT,
    ModifiedDate DateTime,

    UNIQUE (LogID),
    UNIQUE (TreeID, LogNumber),

    CHECK (LogID LIKE '________-____-____-____-____________')
);";
    }

    public partial class Migrations
    {
        public const string MIGRATE_LOG_FROM_LOG =
            "INSERT INTO {0}.Log ( " +
                    "Log_CN, " +
                    "LogID, " +
                    "TreeID, " +
                    "LogNumber, " +
                    "Grade, " +
                    "SeenDefect, " +
                    "PercentRecoverable, " +
                    "Length, " +
                    "ExportGrade, " +
                    "SmallEndDiameter, " +
                    "LargeEndDiameter, " +
                    "GrossBoardFoot, " +
                    "NetBoardFoot, " +
                    "GrossCubicFoot, " +
                    "NetCubicFoot, " +
                    "BoardFootRemoved, " +
                    "CubicFootRemoved, " +
                    "DIBClass, " +
                    "BarkThickness, " +
                    "CreatedBy, " +
                    "CreatedDate, " +
                    "ModifiedBy, " +
                    "ModifiedDate, " +
                    "RowVersion " +
                ") " +
                "SELECT " +
                    "l.Log_CN, " +
                    "ifnull( " +
                        "(CASE typeof(l.Log_GUID) COLLATE NOCASE " + // ckeck the type of Log_GUID
                            "WHEN 'TEXT' THEN " + // if text
                                "(CASE WHEN l.Log_GUID LIKE '________-____-____-____-____________' " + // check to see if it is a properly formated guid
                                    "THEN nullif(l.Log_GUID, '00000000-0000-0000-0000-000000000000') " + // if not a empty guid return that value otherwise return null for now
                                    "ELSE NULL END) " + // if it is not a properly formatted guid return Log_GUID
                            "ELSE NULL END)" + // if value is not a string return null
                        ", (hex( randomblob(4)) || '-' || hex( randomblob(2)) " +
                             "|| '-' || '4' || substr(hex(randomblob(2)), 2) || '-' " +
                             "|| substr('AB89', 1 + (abs(random()) % 4), 1) || " +
                             "substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6)))) AS LogID, " +
                    "t.TreeID AS TreeID, " +
                    "l.LogNumber, " +
                    "l.Grade, " +
                    "l.SeenDefect, " +
                    "l.PercentRecoverable, " +
                    "l.Length, " +
                    "l.ExportGrade, " +
                    "l.SmallEndDiameter, " +
                    "l.LargeEndDiameter, " +
                    "l.GrossBoardFoot, " +
                    "l.NetBoardFoot, " +
                    "l.GrossCubicFoot, " +
                    "l.NetCubicFoot, " +
                    "l.BoardFootRemoved, " +
                    "l.CubicFootRemoved, " +
                    "l.DIBClass, " +
                    "l.BarkThickness, " +
                    "l.CreatedBy, " +
                    "l.CreatedDate, " +
                    "l.ModifiedBy, " +
                    "l.ModifiedDate, " +
                    "l.RowVersion " +
                "FROM {1}.Log as l " +
                "JOIN {0}.Tree AS t USING (Tree_CN);";
    }

    //public partial class Updater
    //{
    //    public const string INITIALIZE_LOG_V3_FROM_LOG =
    //        "INSERT INTO LOG_V3 " +
    //        "SELECT " +
    //            "l.Log_CN, " +
    //            "l.Log_GUID AS LogID, " +
    //            "t.TreeID AS TreeID, " +
    //            "l.LogNumber, " +
    //            "l.Grade, " +
    //            "l.SeenDefect, " +
    //            "l.PercentRecoverable, " +
    //            "l.Length, " +
    //            "l.ExportGrade, " +
    //            "l.SmallEndDiameter, " +
    //            "l.LargeEndDiameter, " +
    //            "l.GrossBoardFoot, " +
    //            "l.NetBoardFoot, " +
    //            "l.GrossCubicFoot, " +
    //            "l.NetCubicFoot, " +
    //            "l.BoardFootRemoved, " +
    //            "l.CubicFootRemoved, " +
    //            "l.DIBClass, " +
    //            "l.BarkThickness, " +
    //            "l.CreatedBy, " +
    //            "l.CreatedDate, " +
    //            "l.ModifiedBy, " +
    //            "l.ModifiedDate, " +
    //            "l.RowVersion " +
    //        "FROM Log as l " +
    //        "JOIN Tree_V3 AS t USING (Tree_CN);";
    //}
}