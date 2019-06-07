namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_VIEW_LOGGRADEERROR =
@"CREATE VIEW LogGradeError AS
SELECT
    l.Log_CN,
    l.LogID,
    LogGradeAuditRule_CN,
    'Species ' || t.Species || ', log grade ' || lgar.Grade || 'max defect is ' || lgar.DefectMax AS Message,
    0 AS IsResolved,
    null AS Resolution,
    null AS ResolutionInitials
FROM Log_V3 AS l
JOIN Tree_V3 AS t USING (TreeID)
JOIN LogGradeAuditRule_V3 AS lgar ON
    ifnull(lgar.Species, '') = '' OR t.Species = lgar.Species
    AND ifnull(l.Grade, '') = lgar.Grade
    AND round(l.SeenDefect, 2) > round(lgar.DefectMax, 2);";
    }
}