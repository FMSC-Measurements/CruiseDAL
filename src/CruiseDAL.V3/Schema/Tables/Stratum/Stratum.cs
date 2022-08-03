using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class StratumTableDefinition : ITableDefinition
    {
        public string TableName => "Stratum";

        public string CreateTable => GetCreateTable(TableName);

        public string GetCreateTable(string tableName)
        {
            return $@"CREATE TABLE {tableName} (
    Stratum_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    StratumID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    Description TEXT,
    Method TEXT COLLATE NOCASE,
    BasalAreaFactor REAL DEFAULT 0.0,
    FixedPlotSize REAL DEFAULT 0.0,
    KZ3PPNT INTEGER DEFAULT 0,
    SamplingFrequency INTEGER DEFAULT 0,
    Hotkey TEXT COLLATE NOCASE,
    FBSCode TEXT COLLATE NOCASE,
    YieldComponent TEXT DEFAULT 'CL' COLLATE NOCASE,

    FixCNTField TEXT COLLATE NOCASE,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,
    FOREIGN KEY (FixCNTField) REFERENCES TreeField (Field),
    FOREIGN KEY (Method) REFERENCES LK_CruiseMethod (Method),

    UNIQUE(StratumID),
    UNIQUE(StratumCode, CruiseID),

    CHECK (YieldComponent IN ('CL', 'CD', 'NL', 'ND')),
    CHECK (StratumID LIKE '________-____-____-____-____________'),
    CHECK (length(StratumCode) > 0)
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE Stratum_Tombstone (
    StratumID TEXT NOT NULL COLLATE NOCASE,
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
    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
    Deleted_TS DATETIME,

    UNIQUE(StratumID)
);

CREATE INDEX NIX_Stratum_Tombstone_CruiseID_StratumCode ON Stratum_Tombstone
(CruiseID, StratumCode);";

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_STRATUM_ONUPDATE, CREATE_TRIGGER_Stratum_OnDelete };

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
    YieldComponent
ON Stratum
FOR EACH ROW
BEGIN
    UPDATE Stratum SET Modified_TS = CURRENT_TIMESTAMP WHERE Stratum_CN = old.Stratum_CN;
END; ";

        public const string CREATE_TRIGGER_Stratum_OnDelete =
@"CREATE TRIGGER Stratum_OnDelete
BEFORE DELETE ON Stratum
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO Stratum_Tombstone (
        StratumID,
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
        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS,
        Deleted_TS
    ) VALUES (
        OLD.StratumID,
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
        OLD.CreatedBy,
        OLD.Created_TS,
        OLD.ModifiedBy,
        OLD.Modified_TS,
        CURRENT_TIMESTAMP
    );
END;";
    }
}