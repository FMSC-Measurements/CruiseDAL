using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class SampleGroupTableDefinition : ITableDefinition
    {
        public string TableName => "SampleGroup";

        public string CreateTable =>
@"CREATE TABLE SampleGroup (
    SampleGroup_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    CutLeave TEXT DEFAULT 'C' COLLATE NOCASE,
    UOM TEXT DEFAULT '' COLLATE NOCASE,
    PrimaryProduct TEXT DEFAULT '' COLLATE NOCASE,
    SecondaryProduct TEXT COLLATE NOCASE,
    BiomassProduct TEXT,
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
    CreatedBy TEXT DEFAULT '',
    CreatedDate DATETIME DEFAULT (datetime('now', 'localtime')),
    ModifiedBy TEXT,
    ModifiedDate DATETIME,
    RowVersion INTEGER DEFAULT 0,

    CHECK (length(SampleGroupCode) > 0)

    UNIQUE (StratumCode, SampleGroupCode, CruiseID),

    FOREIGN KEY (StratumCode, CruiseID) REFERENCES Stratum (StratumCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE
);";

        public string InitializeTable => null;

        public string CreateIndexes =>
@"CREATE INDEX SampleGroup_StratumCode_CruiseID ON SampleGroup (StratumCode, CruiseID);";

        public string CreateTombstoneTable =>
@"CREATE TABLE SampleGroup_Tombstone (
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
    CreatedDate DATETIME,
    ModifiedBy TEXT,
    ModifiedDate DATETIME,
    RowVersion INTEGER,

    UNIQUE (StratumCode, SampleGroupCode, CruiseID)
);";

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_SAMPLEGROUP_ONUPDATE, CREATE_TRIGGER_SampleGoup_OnDelete };

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
    UPDATE SampleGroup SET ModifiedDate = datetime('now', 'localtime') WHERE SampleGroup_CN = old.SampleGroup_CN;
    UPDATE SampleGroup SET RowVersion = old.RowVersion WHERE SampleGroup_CN = old.SampleGroup_CN;
END;";

        public const string CREATE_TRIGGER_SampleGoup_OnDelete =
@"CREATE TRIGGER SampleGroup_OnDelete
BEFORE DELETE ON SampleGroup
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO SampleGroup_Tombstone (
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
        CreatedDate,
        ModifiedBy,
        ModifiedDate,
        RowVersion
    ) VALUES (
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
        OLD.CreatedDate,
        OLD.ModifiedBy,
        OLD.ModifiedDate,
        OLD.RowVersion
    );
END;";
    }
}