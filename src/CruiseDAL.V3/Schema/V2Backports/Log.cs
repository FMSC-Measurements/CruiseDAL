namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_LOG =
            "CREATE VIEW Log AS " +
            "SELECT " +
            "l.Log_CN, " +
            "l.LogID AS Log_GUID, " +
            "t.Tree_CN, " +
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
            "FROM Log_V3 AS l " +
            "JOIN Tree_V3 AS t USING (TreeID);";
    }
}