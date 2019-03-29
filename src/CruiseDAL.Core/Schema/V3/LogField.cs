namespace CruiseDAL.Schema
{
    public partial class DDL
    {


        public const string CREATE_TABLE_LOGFIELD =
@"CREATE TABLE LogField (
    Field TEXT NOT NULL COLLATE NOCASE,
    DbType TEXT NOT NULL COLLATE NOCASE,
    CHECK (length(Field) > 0),
    UNIQUE (Field)
);";

        public const string INITIALIZE_TABLE_LOGFIELD =
@"INSERT INTO LogField
    (Field, DbType)
VALUES
    ('Grade', 'TEXT'),
    ('ExportGrade','TEXT'),
    ('SeenDefect', 'REAL'),
    ('PercentRecoverable', 'REAL'),
    ('SmallEndDiameter', 'REAL'),
    ('LargeEndDiameter', 'REAL'),
    ('GrossBoardFoot', 'REAL'),
    ('NetBoardFoot', 'REAL'),
    ('GrossCubicFoot', 'REAL'),
    ('NetCubicFoot', 'REAL'),
    ('BoardFootRemoved', 'REAL'),
    ('CubicFootRemoved', 'REAL'),
    ('DIBClass', 'REAL'),
    ('BarkThickness', 'REAL')
;";

        //        public const string CREATE_VIEW_UNPIVOTLOG =
        //@"CREATE VIEW UnpivotLog AS
        //SELECT
        //    l.LogID,
        //    l.TreeID,
        //    lf.Field,
        //    lf.DbType,
        //    (CASE lf.Field
        //        WHEN 'Grade' THEN l.Grade
        //        WHEN 'ExportGrade' THEN l.ExportGrade
        //        ELSE NULL END) AS ValueText,
        //    (CASE lf.Field
        //        WHEN 'SeenDefect' THEN l.SeenDefect
        //        WHEN 'PercentRecoverable' THEN l.PercentRecoverable
        //        WHEN 'SmallEndDiameter' THEN l.SmallEndDiameter
        //        WHEN 'LargeEndDiameter' THEN l.LargeEndDiameter
        //        WHEN 'GrossBoardFoot' THEN l.GrossBoardFoot
        //        WHEN 'NetBoardFoot' THEN l.NetBoardFoot
        //        WHEN 'GrossCubicFoot' THEN l.GrossCubicFoot
        //        WHEN 'NetCubicFoot' THEN l.NetCubicFoot
        //        WHEN 'BoardFootRemoved' THEN l.BoardFootRemoved
        //        WHEN 'CubicFootRemoved' THEN l.CubicFootRemoved
        //        WHEN 'DIBClass' THEN l.DIBClass
        //        WHEN 'BarkThickness' THEN l.BarkThickness
        //        ELSE NULL END) AS ValueReal,
        //    (CASE lf.Field
        //        WHEN 'Length' THEN l.Length
        //        ELSE NULL END) AS ValueInt
        //FROM Log_V3 AS l
        //CROSS JOIN LogFields AS lf;";
    }
}