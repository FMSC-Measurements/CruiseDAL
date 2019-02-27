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
    }
}
