using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_ERRORLOG =
            "CREATE TABLE ErrorLog ( " +
                "TableName TEXT NOT NULL,  " +
                "CN_Number INTEGER NOT NULL, " +
                "ColumnName TEXT NOT NULL, " +
                "Level TEXT NOT NULL, " +
                "Message TEXT, " +
                "Program TEXT, " +
                "Suppress BOOLEAN Default 0, " +
                "UNIQUE(TableName, CN_Number, ColumnName, Level)" +
            ");";
    }
}
