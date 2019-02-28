using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_LOGGRADEAUDITRULE =
            "CREATE TABLE LogGradeAuditRule( " +
                "Species TEXT, " +
                "DefectMax REAL Default 0.0, " +
                "ValidGrades TEXT);";
    }
}
