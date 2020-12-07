namespace CruiseDAL.Schema.V2Backports
{
    public class Log_V2_ViewDefinition : IViewDefinition
    {
        public string ViewName => "Log_V2";

        public string CreateView =>
@"CREATE VIEW Log_V2 AS
    SELECT
        l.Log_CN,
        l.LogID AS Log_GUID,
        t.Tree_CN,
        l.LogNumber,
        l.Grade,
        l.SeenDefect,
        l.PercentRecoverable,
        l.Length,
        l.ExportGrade,
        l.SmallEndDiameter,
        l.LargeEndDiameter,
        l.GrossBoardFoot,
        l.NetBoardFoot,
        l.GrossCubicFoot,
        l.NetCubicFoot,
        l.BoardFootRemoved,
        l.CubicFootRemoved,
        l.DIBClass,
        l.BarkThickness,
        l.CreatedBy,
        l.Created_TS AS CreateDate,
        l.ModifiedBy,
        l.Modified_TS AS ModifiedDate,
        0 AS RowVersion
    FROM Log AS l
    JOIN Tree AS t USING (TreeID);";
    }
}