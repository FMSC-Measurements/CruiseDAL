using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class SampleGroupTableDefinition : ITableDefinition
    {
        public string TableName => "SampleGroup";

        public string CreateTable =>
@"CREATE TABLE SampleGroup (
    SampleGroup_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    SampleGroupID  TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    CutLeave TEXT DEFAULT 'C' COLLATE NOCASE,
    UOM TEXT COLLATE NOCASE,
    PrimaryProduct TEXT COLLATE NOCASE,
    SecondaryProduct TEXT COLLATE NOCASE,
    BiomassProduct TEXT COLLATE NOCASE,
    DefaultLiveDead TEXT DEFAULT 'L' COLLATE NOCASE,
    SamplingFrequency INTEGER Default 0,
    InsuranceFrequency INTEGER Default 0,
    KZ INTEGER Default 0,
    BigBAF INTEGER Default 0,
    TallyBySubPop BOOLEAN DEFAULT 0,
    UseExternalSampler BOOLEAN DEFAULT 0,
    TallyMethod TEXT COLLATE NOCASE,
    Description TEXT,
    MinKPI INTEGER Default 0,
    MaxKPI INTEGER Default 0,
    SmallFPS REAL DEFAULT 0.0,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    CHECK (SampleGroupID LIKE '________-____-____-____-____________'),
    CHECK (length(SampleGroupCode) > 0)
    CHECK (TallyBySubPop IN (0, 1)),
    CHECK (UseExternalSampler IN (0, 1)),

    UNIQUE (SampleGroupID),
    UNIQUE (StratumCode, SampleGroupCode, CruiseID),

    FOREIGN KEY (StratumCode, CruiseID) REFERENCES Stratum (StratumCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (UOM) REFERENCES LK_UOM (UOM),
    FOREIGN KEY (PrimaryProduct) REFERENCES LK_Product (Product),
    FOREIGN KEY (SecondaryProduct) REFERENCES LK_Product (Product)
);";

        public string InitializeTable => null;

        public string CreateIndexes =>
@"CREATE INDEX NIX_SampleGroup_StratumCode_CruiseID ON SampleGroup (StratumCode, CruiseID);";

        public string CreateTombstoneTable =>
@"CREATE TABLE SampleGroup_Tombstone (
    SampleGroupID  TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    CutLeave TEXT COLLATE NOCASE,
    UOM TEXT COLLATE NOCASE,
    PrimaryProduct TEXT COLLATE NOCASE,
    SecondaryProduct TEXT COLLATE NOCASE,
    BiomassProduct TEXT,
    DefaultLiveDead TEXT COLLATE NOCASE,
    SamplingFrequency INTEGER Default 0,
    InsuranceFrequency INTEGER Default 0,
    KZ INTEGER,
    BigBAF INTEGER,
    TallyBySubPop BOOLEAN,
    UseExternalSampler BOOLEAN,
    TallyMethod TEXT COLLATE NOCASE,
    Description TEXT,
    MinKPI INTEGER,
    MaxKPI INTEGER,
    SmallFPS REAL,
    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
    Deleted_TS DATETIME,

    UNIQUE(SampleGroupID)
);

CREATE INDEX NIX_SampleGroup_Tombstone_CruiseID_SampleGroupCode_StratumCode ON SampleGroup_Tombstone 
(CruiseID, SampleGroupCode, StratumCode);";

        public IEnumerable<string> CreateTriggers => new[] 
        { 
            CREATE_TRIGGER_SAMPLEGROUP_ONUPDATE, 
            CREATE_TRIGGER_SampleGoup_OnDelete 
        };

        public const string CREATE_TRIGGER_SAMPLEGROUP_ONUPDATE =
@"CREATE TRIGGER SampleGroup_OnUpdate
AFTER UPDATE OF
    SampleGroupCode,
    StratumCode,
    CutLeave,
    UOM,
    PrimaryProduct,
    SecondaryProduct,
    BiomassProduct,
    DefaultLiveDead,
    SamplingFrequency,
    InsuranceFrequency,
    KZ,
    BigBAF,
    TallyBySubPop,
    TallyMethod,
    Description,
    MinKPI,
    MaxKPI,
    SmallFPS
ON SampleGroup
FOR EACH ROW
BEGIN
    UPDATE SampleGroup SET Modified_TS = CURRENT_TIMESTAMP WHERE SampleGroup_CN = old.SampleGroup_CN;
END;";

        public const string CREATE_TRIGGER_SampleGoup_OnDelete =
@"CREATE TRIGGER SampleGroup_OnDelete
BEFORE DELETE ON SampleGroup
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO SampleGroup_Tombstone (
        SampleGroupID,
        CruiseID,
        SampleGroupCode,
        StratumCode,
        CutLeave,
        UOM,
        PrimaryProduct,
        SecondaryProduct,
        BiomassProduct,
        DefaultLiveDead,
        SamplingFrequency,
        InsuranceFrequency,
        KZ,
        BigBAF,
        TallyBySubPop,
        UseExternalSampler,
        TallyMethod,
        Description,
        MinKPI,
        MaxKPI,
        SmallFPS,
        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS,
        Deleted_TS
    ) VALUES (
        OLD.SampleGroupID,
        OLD.CruiseID,
        OLD.SampleGroupCode,
        OLD.StratumCode,
        OLD.CutLeave,
        OLD.UOM,
        OLD.PrimaryProduct,
        OLD.SecondaryProduct,
        OLD.BiomassProduct,
        OLD.DefaultLiveDead,
        OLD.SamplingFrequency,
        OLD.InsuranceFrequency,
        OLD.KZ,
        OLD.BigBAF,
        OLD.TallyBySubPop,
        OLD.UseExternalSampler,
        OLD.TallyMethod,
        OLD.Description,
        OLD.MinKPI,
        OLD.MaxKPI,
        OLD.SmallFPS,
        OLD.CreatedBy,
        OLD.Created_TS,
        OLD.ModifiedBy,
        OLD.Modified_TS,
        CURRENT_TIMESTAMP
    );
END;";
    }
}