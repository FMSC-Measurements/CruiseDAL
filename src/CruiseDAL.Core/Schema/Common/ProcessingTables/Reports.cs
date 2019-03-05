using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_REPORTS =
            "CREATE TABLE Reports( " +
                "ReportID TEXT NOT NULL, " +
                "Selected BOOLEAN Default 0, " +
                "Title TEXT, " +
                "UNIQUE (ReportID)" +
            ");";
    }
}
