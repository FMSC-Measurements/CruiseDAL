using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.V2Backports
{
    public partial class DDL
    {
        public static string CREATE_VIEW_LOG =
            "CREATE VIEW Log AS " +
            "SELECT Log_CN, " +
            "LogID AS Log_GUID, " +
            "t.Tree_CN, " +
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
            "FROM Log_V3 AS l " +
            "JOIN Tree_V3 AS t USING (TreeID);";
    }
}
