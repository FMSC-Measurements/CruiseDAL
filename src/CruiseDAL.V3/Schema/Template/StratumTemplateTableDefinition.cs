using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class StratumTemplateTableDefinition : ITableDefinition
    {
        public string TableName => "StratumTemplate";

        public string CreateTable => GetCreateTable(TableName);


        public string GetCreateTable(string tableName)
        {
return $@"CREATE TABLE {tableName} (
    StratumTemplate_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    StratumTemplateName TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT COLLATE NOCASE,
    Method TEXT COLLATE NOCASE,
    BasalAreaFactor REAL,
    FixedPlotSize REAL,
    KZ3PPNT INTEGER,
    SamplingFrequency INTEGER,
    Hotkey TEXT,
    FBSCode TEXT,
    YieldComponent TEXT,
    FixCNTField TEXT COLLATE NOCASE,

    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    CHECK (YieldComponent IN ('CL', 'CD', 'NL', 'ND') OR YieldComponent IS NULL),
    CHECK (length(StratumCode) > 0 OR StratumCode IS NULL),

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,
    FOREIGN KEY (FixCNTField) REFERENCES TreeField (Field),
    FOREIGN KEY (Method) REFERENCES LK_CruiseMethod (Method),

    UNIQUE (StratumTemplateName, CruiseID)
);
";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE StratumTemplate_Tombstone (
    StratumTemplateName TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT COLLATE NOCASE,
    Method TEXT COLLATE NOCASE,
    BasalAreaFactor REAL,
    FixedPlotSize REAL,
    KZ3PPNT INTEGER,
    SamplingFrequency INTEGER,
    Hotkey TEXT,
    FBSCode TEXT,
    YieldComponent TEXT,
    FixCNTField TEXT COLLATE NOCASE,

    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
    Deleted_TS DATETIME
);

CREATE INDEX NIX_StratumTemplate_Tombstone_CruiseID_StratumTemplateName ON StratumTemplate_Tombstone 
(CruiseID, StratumTemplateName);
";

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_StratumTemplate_OnDelete, CREATE_TRIGGER_StratumTemplate_OnUpdate, };

        public const string CREATE_TRIGGER_StratumTemplate_OnUpdate =
@"CREATE TRIGGER StratumTemplate_OnUpdate
AFTER UPDATE OF 
    StratumCode,
    Method,
    BasalAreaFactor,
    FixedPlotSize,
    KZ3PPNT,
    SamplingFrequency,
    Hotkey,
    FBSCode,
    YieldComponent,
    FixCNTField
ON StratumTemplate
FOR EACH ROW
BEGIN
    UPDATE StratumTemplate SET Modified_TS = CURRENT_TIMESTAMP WHERE StratumTemplate_CN = OLD.StratumTemplate_CN;
END;
";

        public const string CREATE_TRIGGER_StratumTemplate_OnDelete =
@"CREATE TRIGGER StratumTemplate_OnDelete 
BEFORE DELETE ON StratumTemplate 
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO StratumTemplate_Tombstone (
        StratumTemplateName,
        CruiseID,
        StratumCode,
        Method,
        BasalAreaFactor,
        FixedPlotSize,
        KZ3PPNT,
        SamplingFrequency,
        Hotkey,
        FBSCode,
        YieldComponent,
        FixCNTField,
        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS,
        Deleted_TS
    ) VALUES (
        OLD.StratumTemplateName,
        OLD.CruiseID,
        OLD.StratumCode,
        OLD.Method,
        OLD.BasalAreaFactor,
        OLD.FixedPlotSize,
        OLD.KZ3PPNT,
        OLD.SamplingFrequency,
        OLD.Hotkey,
        OLD.FBSCode,
        OLD.YieldComponent,
        OLD.FixCNTField,
        OLD.CreatedBy,
        OLD.Created_TS,
        OLD.ModifiedBy,
        OLD.Modified_TS,
        CURRENT_TIMESTAMP
    );
END;
";
    }
}