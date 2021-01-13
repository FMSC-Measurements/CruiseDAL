using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CruiseDAL.Migrators
{
    public class LogGradeAuditRuleMigrator : IMigrator
    {
        public string MigrateToV3(string toDbName, string fromDbName, string cruiseID, string saleID)
        {
            return
$@"INSERT INTO {toDbName}.LogGradeAuditRule (
    CruiseID,
    SpeciesCode,
    DefectMax,
    Grade
)
WITH RECURSIVE splitGrades(SpeciesCode, DefectMax, Grade, ValidGrades) AS (
    SELECT 
        Species AS SpeciesCode, 
        DefectMax, 
        '', 
        replace(ValidGrades, ' ', '') 
    FROM {fromDbName}.LogGradeAuditRule -- select values from original table removing all white space
    UNION ALL
    SELECT
        SpeciesCode,
        DefectMax,
            substr(ValidGrades, 0, ifnull(nullif(instr(ValidGrades, ','), 0), 
            length(ValidGrades) + 1)), -- grab value upto the next comma, if no comma return whole string
            substr(ValidGrades, ifnull(nullif(instr(ValidGrades, ','), 0), length(ValidGrades) + 1)+1) -- send rest of the original text after our comma to next itteration
       FROM splitGrades 
            WHERE length(ValidGrades) > 0 -- end loop when length of remaining text is 0
)

SELECT
    '{cruiseID}',
    nullif(sg.SpeciesCode, 'ANY') AS SpeciesCode, -- in version 2 'ANY' was used to indicate that a rule applied to all species values
    sg.DefectMax,
    sg.Grade
FROM splitGrades AS sg
WHERE length(sg.Grade) > 0;";
        }
    }
}
