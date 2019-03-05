using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_LOGFIELDSETUPDEFAULT =
            "CREATE TABLE LogFieldSetupDefault ( " +
                "LogFieldSetupDefault_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Field TEXT NOT NULL, " +
                "FieldName TEXT, " +
                "FieldOrder INTEGER Default 0, " +
                "ColumnType TEXT, " +
                "Heading TEXT, " +
                "Width REAL Default 0.0, " +
                "Format TEXT, " +
                "Behavior TEXT, " +
                "UNIQUE(Field));";
    }
}
