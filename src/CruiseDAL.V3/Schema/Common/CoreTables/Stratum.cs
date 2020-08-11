namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_STRATUM =
            "CREATE TABLE Stratum ( " +
                "Stratum_CN INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Code TEXT NOT NULL COLLATE NOCASE, " +
                "CruiseID TEXT NOT NULL COLLATE NOCASE," +
                "Description TEXT, " +
                "Method TEXT DEFAULT '' COLLATE NOCASE, " +
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
                "FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE, " +
                "UNIQUE(Code), " +
                "CHECK (length(Code) > 0) " +
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

    public partial class Migrations
    {
        public const string MIGRATE_STRATUM_FORMAT_STR =
            "INSERT INTO {0}.Stratum ( " +
                    "Code, " +
                    "CruiseID, " +
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
                    "Year, " +
                    "CreatedBy, " +
                    "CreatedDate, " +
                    "ModifiedBy, " +
                    "ModifiedDate, " +
                    "RowVersion " +
                ") " +
                "SELECT " +
                    "Code, " +
                    "{4}, " +
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
                    "Year, " +
                    "CreatedBy, " +
                    "CreatedDate, " +
                    "ModifiedBy, " +
                    "ModifiedDate, " +
                    "RowVersion " +
                "FROM {1}.Stratum;";
    }
}