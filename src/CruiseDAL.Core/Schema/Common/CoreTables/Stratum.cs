using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema.Common
{
    public partial class DDL
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

        public const string CREATE_TRIGGER_STRATUM_ONUPDATE =
            "CREATE TRIGGER Stratum_OnUpdate " +
            "AFTER UPDATE OF " +
                "Code, " +
                "Description, " +
                "Method, " +
                "BasalAreaFactor, " +
                "FixedPlotSize, " +
                "KZ3PPNT, " +
                "SamplingFrequency, " +
                "HotKey, " +
                "FBSCode, " +
                "YieldComponent, " +
                "VolumeFactor, " +
                "Month, " +
                "Year " +
            "ON Stratum " +
            "FOR EACH ROW " +
            "BEGIN " +
                "UPDATE Stratum SET ModifiedDate = datetime( 'now', 'localtime') WHERE Stratum_CN = old.Stratum_CN; " +
                "UPDATE Stratum SET RowVersion = datetime( 'now', 'localtime') WHERE Stratum_CN = old.Stratum_CN; " +
            "END; ";
    }
}
