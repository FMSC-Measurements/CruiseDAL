using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public class Stratum
    {
        public const string CREATE_TABLE_STRATUM =
            "CREATE TABLE Stratum ( " +
                "Stratum_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Code TEXT NOT NULL COLLATE NOCASE, " +
                "Description TEXT, " +
                "Method TEXT DEFAULT '', " +
                "BasalAreaFactor REAL DEFAULT 0.0, " +
                "FixedPlotSize REAL DEFAULT 0.0, " +
                "KZ3PPNT INTEGER DEFAULT 0, " +
                "SamplingFrequency INTEGER DEFAULT 0, " +
                "Hotkey TEXT, " +
                "FBSCode TEXT, " +
                "YieldComponent TEXT DEFAULT 'CL', " +
                "VolumeFactor REAL DEFAULT 0.0, " +
                "Month INTEGER DEFAULT 0, " +
                "Year INTEGER DEFAULT 0, " +
                "CreatedBy TEXT DEFAULT 'none', " +
                "CreatedDate DateTime DEFAULT (datetime('now', 'localtime')), " +
                "ModifiedBy TEXT, " +
                "ModifiedDate DateTime , " +
                "RowVersion INTEGER DEFAULT 0, " +
                "UNIQUE(Code) " +
            ");";
    }
}
