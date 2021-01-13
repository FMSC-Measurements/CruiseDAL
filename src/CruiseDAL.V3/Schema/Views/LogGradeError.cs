namespace CruiseDAL.Schema
{
    public class LogGradeErrorViewDefinition : IViewDefinition
    {
        public string ViewName => "LogGradeError";

        public string CreateView =>
@"CREATE VIEW LogGradeError AS
SELECT
    l.Log_CN,
    l.LogID,
    LogGradeAuditRule_CN,
    'Species ' || t.SpeciesCode || ', log grade ' || lgar.Grade || 'max defect is ' || lgar.DefectMax AS Message,
    0 AS IsResolved,
    null AS Resolution,
    null AS ResolutionInitials
FROM Log AS l
JOIN Tree AS t USING (TreeID)
JOIN LogGradeAuditRule AS lgar ON
    t.CruiseID = lgar.CruiseID
    AND ifnull(lgar.SpeciesCode, '') IN  (t.SpeciesCode, '')
    AND ifnull(l.Grade, '') = lgar.Grade
    AND round(l.SeenDefect, 2) > round(lgar.DefectMax, 2);";


    }
}