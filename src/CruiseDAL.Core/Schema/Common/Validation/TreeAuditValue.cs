using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TREEAUDITVALUE =
            "CREATE TABLE TreeAuditValue( " +
                "TreeAuditValue_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "TreeAuditValueID TEXT NOT NULL," +
                "Field TEXT NOT NULL COLLATE NOCASE, " +
                "Min REAL Default 0.0, " +
                "Max REAL Default 0.0, " +
                "ValueSet TEXT, " +
                "Required BOOLEAN DEFAULT 0, " +
                "ErrorMessage TEXT, " +
                "UNIQUE (TreeAuditValueID) " +
            ");";
    }
}
