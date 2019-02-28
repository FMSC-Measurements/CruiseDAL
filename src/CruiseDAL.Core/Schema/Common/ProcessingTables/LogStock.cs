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
                "CreatedDate DateTime DEFAULT (datetime(current_timestamp, 'localtime')) , " +
                "ModifiedBy TEXT, " +
                "ModifiedDate DateTime , " +
                "UNIQUE(Tree_CN, LogNumber), " +
                "FOREIGN KEY (Tree_CN) REFERENCES Tree (Tree_CN) ON DELETE CASCADE" +
            ");";
    }
}
