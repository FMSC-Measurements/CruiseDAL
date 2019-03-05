using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_LOGMATRIX =
            "CREATE TABLE LogMatrix( " +
                "ReportNumber TEXT, " +
                "GradeDescription TEXT, " +
                "LogSortDescription TEXT, " +
                "Species TEXT, " +
                "LogGrade1 TEXT, " +
                "LogGrade2 TEXT, " +
                "LogGrade3 TEXT, " +
                "LogGrade4 TEXT, " +
                "LogGrade5 TEXT, " +
                "LogGrade6 TEXT, " +
                "SEDlimit TEXT, " +
                "SEDminimum DOUBLE Default 0.0, " +
                "SEDmaximum DOUBLE Default 0.0" +
            ");";
    }
}
