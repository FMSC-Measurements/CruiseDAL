using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V3
{
    public class Log_V3
    {
        public const string CREATE_TABLE_LOG_V3 =
            "CREATE TABLE Log_V3 ( " +
                "Log_CN INTIGER PRIMARY KEY AUTOINCREMENT, " +
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
                "CreatedBy TEXT DEFAULT 'none', " +
                "CreatedDate DateTime DEFAULT (datetime(current_timestamp, 'localtime')) , " +
                "ModifiedBy TEXT , " +
                "ModifiedDate DateTime , " +
                "RowVersion INTEGER DEFAULT 0, " +
                "UNIQUE (TreeID, LogNumber)" +
            ");";

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

    public partial class Updater
    {
        public const string INITIALIZE_LOG_V3_FROM_LOG =
            "INSERT INTO LOG_V3 " +
            "SELECT " +
            "Log_GUID AS LogID, " +
            "t.TreeID AS TreeID, " +
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
            "RowVersion" +
            "FROM Log " +
            "JOIN Tree_V3 AS t USING (Tree_CN);";
    }
}
