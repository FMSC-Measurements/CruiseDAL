namespace CruiseDAL.Schema
{
    public partial class DDL
    {


        public const string CREATE_TABLE_LOG_V3 =
            "CREATE TABLE Log_V3 ( " +
                "Log_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "LogID TEXT NOT NULL, " +
                "TreeID TEXT NOT NULL, " +
                "LogNumber TEXT NOT NULL, " +
                "Grade TEXT COLLATE NOCASE, " +
                "SeenDefect REAL Default 0.0, " +
                "PercentRecoverable REAL Default 0.0, " +
                "Length INTEGER Default 0, " +
                "ExportGrade TEXT, " +
                "SmallEndDiameter REAL Default 0.0, " +
                "LargeEndDiameter REAL Default 0.0, " +
                "GrossBoardFoot REAL Default 0.0, " +
                "NetBoardFoot REAL Default 0.0, " +
                "GrossCubicFoot REAL Default 0.0, " +
                "NetCubicFoot REAL Default 0.0, " +
                "BoardFootRemoved REAL Default 0.0, " +
                "CubicFootRemoved REAL Default 0.0, " +
                "DIBClass REAL Default 0.0, " +
                "BarkThickness REAL Default 0.0, " +
                "CreatedBy TEXT DEFAULT '', " +
                "CreatedDate DateTime DEFAULT (datetime('now', 'localtime')), " +
                "ModifiedBy TEXT , " +
                "ModifiedDate DateTime , " +
                "RowVersion INTEGER DEFAULT 0, " +

                "UNIQUE (TreeID, LogNumber), " +

                "FOREIGN KEY (TreeID) REFERENCES Tree_V3 (TreeID) ON DELETE CASCADE " +
            ");";

        public const string CREATE_INDEX_Log_V3_TreeID =
            @"CREATE INDEX Log_V3_TreeID ON Log_V3 (TreeID);";

        public const string CREATE_TRIGGER_LOG_V3_ONUPDATE =
            "CREATE TRIGGER Log_V3_OnUpdate " +
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
            "ON Log_V3 " +
            "FOR EACH ROW " +
            "BEGIN " +
                "UPDATE Log_V3 SET RowVersion = old.RowVersion + 1 WHERE Log_CN = old.Log_CN; " +
                "UPDATE Log_V3 SET ModifiedDate = datetime('now', 'localtime') WHERE Log_CN = old.Log_CN; " +
            "END;";
    }

    public partial class Migrations
    {
        public const string MIGRATE_LOG_V3_FROM_LOG =
            "INSERT INTO {0}.Log_V3 ( " +
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
                    "l.Log_GUID AS LogID, " +
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
                "JOIN {0}.Tree_V3 AS t USING (Tree_CN);";
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