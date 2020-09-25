namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_SAMPLEGROUP =
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

        public const string CREATE_INDEX_SampleGroup_StratumCode_CruiseID =
@"CREATE INDEX SampleGroup_StratumCode_CruiseID ON SampleGroup (StratumCode, CruiseID);";

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

        public const string CREATE_TOMBSTONE_TABLE_SampleGroup_Tombstone =
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
    }

    public partial class Migrations
    {
        public const string MIGRATE_SAMPLEGROUP_FROM_SAMPLEGROUP =
            "INSERT INTO {0}.SampleGroup ( " +
                    "SampleGroup_CN, " +
                    "CruiseID, " +
                    "SampleGroupCode, " +
                    "StratumCode, " +
                    "CutLeave, " +
                    "UOM, " +
                    "PrimaryProduct, " +
                    "SecondaryProduct, " +
                    "BiomassProduct, " +
                    "DefaultLiveDead, " +
                    "SamplingFrequency, " +
                    "InsuranceFrequency, " +
                    "KZ, " +
                    "BigBAF, " +
                    "TallyBySubPop," +
                    "UseExternalSampler," +
                    "TallyMethod, " +
                    "Description, " +
                    "MinKPI, " +
                    "MaxKPI, " +
                    "SmallFPS, " +
                    "CreatedBy, " +
                    "CreatedDate, " +
                    "ModifiedBy, " +
                    "ModifiedDate, " +
                    "RowVersion " +
                ") " +
                "SELECT " +
                    "sg.SampleGroup_CN, " +
                    "'{3}', " +
                    "sg.Code AS SampleGroupCode, " +
                    "st.Code AS StratumCode, " +
                    "sg.CutLeave, " +
                    "sg.UOM, " +
                    "sg.PrimaryProduct, " +
                    "sg.SecondaryProduct, " +
                    "sg.BiomassProduct, " +
                    "sg.DefaultLiveDead, " +
                    "sg.SamplingFrequency, " +
                    "sg.InsuranceFrequency, " +
                    "sg.KZ, " +
                    "sg.BigBAF, " +
                    "(EXISTS (" +
                            "SELECT * FROM {1}.CountTree AS ct " +
                            "WHERE ct.SampleGroup_CN = sg.SampleGroup_CN AND ct.TreeDefaultValue_CN NOT NULL" +
                        ")) AS TallyBySubPop, " +
                    "(CASE WHEN sg.SampleSelectorType = 'ClickerSelecter' THEN 1 ELSE 0 END) AS UseExternalSampler, " +
                    "sg.TallyMethod, " +
                    "sg.Description, " +
                    "sg.MinKPI, " +
                    "sg.MaxKPI, " +
                    "sg.SmallFPS, " +
                    "sg.CreatedBy, " +
                    "sg.CreatedDate, " +
                    "sg.ModifiedBy, " +
                    "sg.ModifiedDate, " +
                    "sg.RowVersion " +
                "FROM {1}.SampleGroup AS sg " +
                "JOIN {1}.Stratum AS st USING (Stratum_CN);";
    }

    //public partial class Updater
    //{
    //    public const string INITIALIZE_SAMPLEGROUP_V3_FROM_SAMPLEGROUP =
    //        "INSERT INTO SampleGroup_V3 " +
    //        "SELECT " +
    //        "sg.SampleGroup_CN, " +
    //        "sg.Code AS SampleGroupCode, " +
    //        "st.Code AS StratumCode, " +
    //        "sg.CutLeave, " +
    //        "sg.UOM, " +
    //        "sg.PrimaryProduct, " +
    //        "sg.SecondaryProduct, " +
    //        "sg.BiomassProduct, " +
    //        "sg.DefaultLiveDead, " +
    //        "sg.SamplingFrequency, " +
    //        "sg.InsuranceFrequency, " +
    //        "sg.KZ, " +
    //        "sg.BigBAF, " +
    //        "sg.TallyMethod, " +
    //        "sg.Description, " +
    //        "sg.SampleSelectorType, " +
    //        "sg.MinKPI, " +
    //        "sg.MaxKPI, " +
    //        "sg.SmallFPS, " +
    //        "sg.CreatedBy, " +
    //        "sg.CreatedDate, " +
    //        "sg.ModifiedBy, " +
    //        "sg.ModifiedDate, " +
    //        "sg.RowVersion " +
    //        "FROM SampleGroup AS sg " +
    //        "JOIN Stratum AS st USING (Stratum_CN);";
    //}
}