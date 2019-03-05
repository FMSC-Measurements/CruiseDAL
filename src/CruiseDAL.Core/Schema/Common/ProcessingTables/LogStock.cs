using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_LOGSTOCK =
            "CREATE TABLE LogStock( " +
                "LogStock_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Tree_CN INTEGER NOT NULL, " +
                "LogNumber TEXT NOT NULL, " +
                "Grade TEXT, " +
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
                "BoardUtil REAL Default 0.0, " +
                "CubicUtil REAL Default 0.0, " +
                "CreatedBy TEXT DEFAULT 'none', " +
                "CreatedDate DateTime DEFAULT (datetime( 'now', 'localtime')), " +
                "ModifiedBy TEXT, " +
                "ModifiedDate DateTime , " +
                "UNIQUE(Tree_CN, LogNumber), " +
                "FOREIGN KEY (Tree_CN) REFERENCES Tree (Tree_CN) ON DELETE CASCADE" +
            ");";

        public const string CREATE_TRIGGER_LOGSTOCK_ONUPDATE =
            "CREATE TRIGGER LogStock_OnUpdate " +
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
                "BarkThickness, " +
                "BoardUtil, " +
                "CubicUtil " +
            "ON LogStock " +
            "FOR EACH ROW " +
            "BEGIN " +
                "UPDATE LogStock SET ModifiedDate = datetime( 'now', 'localtime') WHERE LogStock_CN = new.LogStock_CN; " +
                "UPDATE LogStock SET RowVersion = new.RowVersion + 1 WHERE LogStock_CN = new.LogStock_CN; " +
            "END; ";
    }
}
