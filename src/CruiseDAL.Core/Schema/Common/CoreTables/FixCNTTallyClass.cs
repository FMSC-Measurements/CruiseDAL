using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_FIXCNTTALLYCLASS =
            "CREATE TABLE FixCNTTallyClass( " +
                "FixCNTTallyClass_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Stratum_CN INTEGER REFERENCES Stratum NOT NULL, " +
                "FieldName INTEGER Default 0);";
    }
}
