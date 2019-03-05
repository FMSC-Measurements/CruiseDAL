using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
    {
        public const string CREATE_TABLE_CUTTINGUNIT =
            "CREATE TABLE CuttingUnit( " +
                "CuttingUnit_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Code TEXT NOT NULL COLLATE NOCASE, " +
                "Area REAL DEFAULT 0.0, " +
                "Description TEXT, " +
                "LoggingMethod TEXT, " +
                "PaymentUnit TEXT, " +
                "TallyHistory TEXT, " +
                "Rx TEXT, " +
                "CreatedBy TEXT DEFAULT 'none', " +
                "CreatedDate DateTime DEFAULT (datetime('now', 'localtime')), " +
                "ModifiedBy TEXT, " +
                "ModifiedDate DateTime , " +
                "RowVersion INTEGER DEFAULT 0, " +
                "UNIQUE(Code)" +
            ");";
    }
}
