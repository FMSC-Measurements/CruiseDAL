namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_STRATUM =
@"CREATE TABLE Stratum (
    Stratum_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Description TEXT,
    Method TEXT DEFAULT '' COLLATE NOCASE,
    BasalAreaFactor REAL DEFAULT 0.0,
    FixedPlotSize REAL DEFAULT 0.0,
    KZ3PPNT INTEGER DEFAULT 0,
    SamplingFrequency INTEGER DEFAULT 0,
    Hotkey TEXT,
    FBSCode TEXT,
    YieldComponent TEXT DEFAULT 'CL',
    VolumeFactor REAL DEFAULT 0.0,
    Month INTEGER DEFAULT 0,
    Year INTEGER DEFAULT 0,
    CreatedBy TEXT DEFAULT 'none',
    CreatedDate DateTime DEFAULT (datetime('now', 'localtime')),
    ModifiedBy TEXT,
    ModifiedDate DateTime ,
    RowVersion INTEGER DEFAULT 0,
    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,
    UNIQUE(StratumCode, CruiseID),
    CHECK (length(StratumCode) > 0)
);";

        public const string CREATE_TRIGGER_STRATUM_ONUPDATE =
@"CREATE TRIGGER Stratum_OnUpdate
AFTER UPDATE OF
    Code,
    Description,
    Method,
    BasalAreaFactor,
    FixedPlotSize,
    KZ3PPNT,
    SamplingFrequency,
    HotKey,
    FBSCode,
    YieldComponent,
    VolumeFactor,
    Month,
    Year
ON Stratum
FOR EACH ROW
BEGIN
    UPDATE Stratum SET ModifiedDate = datetime( 'now', 'localtime') WHERE Stratum_CN = old.Stratum_CN;
    UPDATE Stratum SET RowVersion = datetime( 'now', 'localtime') WHERE Stratum_CN = old.Stratum_CN;
END; ";

        public const string CREATE_TRIGGER_Stratum_OnDelete =
@"CREATE TRIGGER Stratum_OnDelete
BEFORE DELETE ON Stratum
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO Stratum_Tombstone (
        StratumCode,
        CruiseID,
        Description,
        Method,
        BasalAreaFactor,
        FixedPlotSize,
        KZ3PPNT,
        SamplingFrequency,
        Hotkey,
        FBSCode,
        YieldComponent,
        VolumeFactor,
        Month,
        Year,
        CreatedBy,
        CreatedDate,
        ModifiedBy,
        ModifiedDate,
        RowVersion
    ) VALUES (
        OLD.StratumCode,
        OLD.CruiseID,
        OLD.Description,
        OLD.Method,
        OLD.BasalAreaFactor,
        OLD.FixedPlotSize,
        OLD.KZ3PPNT,
        OLD.SamplingFrequency,
        OLD.Hotkey,
        OLD.FBSCode,
        OLD.YieldComponent,
        OLD.VolumeFactor,
        OLD.Month,
        OLD.Year,
        OLD.CreatedBy,
        OLD.CreatedDate,
        OLD.ModifiedBy,
        OLD.ModifiedDate,
        OLD.RowVersion
    );
END;";

        public const string CREATE_TOMBSTONE_TABLE_Stratum_Tombstone =
@"CREATE TABLE Stratum_Tombstone (
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Description TEXT,
    Method TEXT COLLATE NOCASE,
    BasalAreaFactor REAL,
    FixedPlotSize REAL,
    KZ3PPNT INTEGER,
    SamplingFrequency INTEGER,
    Hotkey TEXT,
    FBSCode TEXT,
    YieldComponent TEXT,
    VolumeFactor REAL,
    Month INTEGER,
    Year INTEGER,
    CreatedBy TEXT,
    CreatedDate DateTime,
    ModifiedBy TEXT,
    ModifiedDate DateTime ,
    RowVersion INTEGER,

    UNIQUE(StratumCode, CruiseID)
);";
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
                    "'{3}', " +
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