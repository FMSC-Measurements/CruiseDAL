using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    // add checks on fields
    public class TreeMeasurmentsTableDefinition_3_5_5 : ITableDefinition
    {

        public string TableName => "TreeMeasurment";

        public string CreateTable => GetCreateTable(TableName);

        public string GetCreateTable(string tableName)
        {
            return $@"CREATE TABLE {tableName} (
    TreeMeasurment_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    TreeID TEXT NOT NULL,

    SeenDefectPrimary REAL Default 0.0,
    SeenDefectSecondary REAL Default 0.0,
    RecoverablePrimary REAL Default 0.0,
    HiddenPrimary REAL Default 0.0,
    Grade TEXT,

    HeightToFirstLiveLimb REAL Default 0.0,
    PoleLength REAL Default 0.0,
    ClearFace TEXT,
    CrownRatio REAL Default 0.0,
    DBH REAL Default 0.0,

    DRC REAL Default 0.0,
    TotalHeight REAL Default 0.0,
    MerchHeightPrimary REAL Default 0.0,
    MerchHeightSecondary REAL Default 0.0,
    FormClass REAL Default 0.0,

    --UpperStemDOB REAL Default 0.0,

    UpperStemDiameter REAL Default 0.0,
    UpperStemHeight REAL Default 0.0,
    DBHDoubleBarkThickness REAL Default 0.0,
    TopDIBPrimary REAL Default 0.0,
    TopDIBSecondary REAL Default 0.0,

    DefectCode TEXT,
    DiameterAtDefect REAL Default 0.0,
    VoidPercent REAL Default 0.0,
    Slope REAL Default 0.0,
    Aspect REAL Default 0.0,

    Remarks TEXT,
    IsFallBuckScale BOOLEAN Default 0,

    MetaData TEXT,
    Initials TEXT,

    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    CHECK (SeenDefectPrimary IS NULL OR SeenDefectPrimary BETWEEN 0.0 AND 100.0),
    CHECK (SeenDefectSecondary IS NULL OR SeenDefectSecondary BETWEEN 0.0 AND 100.0),
    CHECK (RecoverablePrimary IS NULL OR RecoverablePrimary BETWEEN 0.0 AND 100.0),
    CHECK (HiddenPrimary IS NULL OR HiddenPrimary BETWEEN 0.0 AND 100.0),
    CHECK (Grade IS NULL OR Grade IN ('0', '1', '2', '3', '4', '5', '6', '7', '8', '9')),

    CHECK (HeightToFirstLiveLimb IS NULL OR HeightToFirstLiveLimb >= 0.0),
    CHECK (PoleLength IS NULL OR PoleLength >= 0.0),
    CHECK (DBH IS NULL OR DBH >= 0.0),

    CHECK (DRC IS NULL OR DRC >= 0.0),
    CHECK (TotalHeight IS NULL OR TotalHeight >= 0.0),
    CHECK (MerchHeightPrimary IS NULL OR MerchHeightPrimary >= 0.0),
    CHECK (MerchHeightSecondary IS NULL OR MerchHeightSecondary >= 0.0),
    CHECK (FormClass IS NULL OR FormClass >= 0.0),

    CHECK (UpperStemDiameter IS NULL OR UpperStemDiameter >= 0.0),
    CHECK (UpperStemHeight IS NULL OR UpperStemHeight >= 0.0),
    CHECK (DBHDoubleBarkThickness IS NULL OR DBHDoubleBarkThickness >= 0.0),
    CHECK (TopDIBPrimary IS NULL OR TopDIBPrimary >= 0.0),
    CHECK (TopDIBSecondary IS NULL OR TopDIBSecondary >= 0.0),

    CHECK (DiameterAtDefect IS NULL OR DiameterAtDefect >= 0.0),
    CHECK (VoidPercent IS NULL OR VoidPercent BETWEEN 0.0 AND 100.0),
    CHECK (Slope IS NULL OR Slope BETWEEN 0.0 AND 200.0), -- value of 200 would be a 90degree slope
    CHECK (Aspect IS NULL OR Aspect BETWEEN 0.0 AND 360.0),

    CHECK (IsFallBuckScale IS NULL OR IsFallBuckScale IN (0, 1)),

    UNIQUE (TreeID),

    FOREIGN KEY (TreeID) REFERENCES Tree (TreeID) ON DELETE CASCADE ON UPDATE CASCADE
)";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE TreeMeasurment_Tombstone (
    TreeID TEXT NOT NULL,

    SeenDefectPrimary REAL,
    SeenDefectSecondary REAL,
    RecoverablePrimary REAL,
    HiddenPrimary REAL,
    Grade TEXT,

    HeightToFirstLiveLimb REAL,
    PoleLength REAL,
    ClearFace TEXT,
    CrownRatio REAL,
    DBH REAL,

    DRC REAL,
    TotalHeight REAL,
    MerchHeightPrimary REAL,
    MerchHeightSecondary REAL,
    FormClass REAL,

    --UpperStemDOB REAL,

    UpperStemDiameter REAL,
    UpperStemHeight REAL,
    DBHDoubleBarkThickness REAL,
    TopDIBPrimary REAL,
    TopDIBSecondary REAL,

    DefectCode TEXT,
    DiameterAtDefect REAL,
    VoidPercent REAL,
    Slope REAL,
    Aspect REAL,

    Remarks TEXT,
    IsFallBuckScale BOOLEAN,

    MetaData TEXT,
    Initials TEXT,

    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
    Deleted_TS DATETIME
);

CREATE INDEX NIX_TreeMeasurment_Tombstone_TreeID ON TreeMeasurment_Tombstone
(TreeID);";

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => new[]
        {
            CREATE_TRIGGER_TREEMEASURMENTS_ONUPDATE,
            CREATE_TRIGGER_TreeMeasurment_OnDelete
        };

        public const string CREATE_TRIGGER_TREEMEASURMENTS_ONUPDATE =
@"CREATE TRIGGER TREEMEASURMENT_ONUPDATE
AFTER UPDATE OF
    SeenDefectPrimary,
    SeenDefectSecondary,
    RecoverablePrimary,
    HiddenPrimary, Grade,
    HeightToFirstLiveLimb,
    PoleLength,
    ClearFace,
    CrownRatio,
    DBH,
    DRC,
    TotalHeight,
    MerchHeightPrimary,
    MerchHeightSecondary,
    FormClass,
    --UpperStemDOB,
    UpperStemDiameter,
    UpperStemHeight,
    DBHDoubleBarkThickness,
    TopDIBPrimary,
    TopDIBSecondary,
    DefectCode,
    DiameterAtDefect,
    VoidPercent,
    Slope,
    Aspect,
    Remarks,
    MetaData,
    IsFallBuckScale,
    Initials
ON TreeMeasurment
FOR EACH ROW
BEGIN
    UPDATE TreeMeasurment SET Modified_TS = CURRENT_TIMESTAMP WHERE TreeMeasurment_CN = old.TreeMeasurment_CN;
END;";

        public const string CREATE_TRIGGER_TreeMeasurment_OnDelete =
@"CREATE TRIGGER TreeMeasurment_OnDelete
BEFORE DELETE ON TreeMeasurment
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO TreeMeasurment_Tombstone (
        TreeID,

        SeenDefectPrimary,
        SeenDefectSecondary,
        RecoverablePrimary,
        HiddenPrimary,
        Grade,

        HeightToFirstLiveLimb,
        PoleLength,
        ClearFace,
        CrownRatio,
        DBH,

        DRC,
        TotalHeight,
        MerchHeightPrimary,
        MerchHeightSecondary,
        FormClass,

        --UpperStemDOB,

        UpperStemDiameter,
        UpperStemHeight,
        DBHDoubleBarkThickness,
        TopDIBPrimary,
        TopDIBSecondary,

        DefectCode,
        DiameterAtDefect,
        VoidPercent,
        Slope,
        Aspect,

        Remarks,
        IsFallBuckScale,

        MetaData,

        Initials,

        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS,
        Deleted_TS
    ) VALUES (
        OLD.TreeID,

        OLD.SeenDefectPrimary,
        OLD.SeenDefectSecondary,
        OLD.RecoverablePrimary,
        OLD.HiddenPrimary,
        OLD.Grade,

        OLD.HeightToFirstLiveLimb,
        OLD.PoleLength,
        OLD.ClearFace,
        OLD.CrownRatio,
        OLD.DBH,

        OLD.DRC,
        OLD.TotalHeight,
        OLD.MerchHeightPrimary,
        OLD.MerchHeightSecondary,
        OLD.FormClass,

        --UpperStemDOB,

        OLD.UpperStemDiameter,
        OLD.UpperStemHeight,
        OLD.DBHDoubleBarkThickness,
        OLD.TopDIBPrimary,
        OLD.TopDIBSecondary,

        OLD.DefectCode,
        OLD.DiameterAtDefect,
        OLD.VoidPercent,
        OLD.Slope,
        OLD.Aspect,

        OLD.Remarks,
        OLD.IsFallBuckScale,

        OLD.MetaData,

        OLD.Initials,

        OLD.CreatedBy,
        OLD.Created_TS,
        OLD.ModifiedBy,
        OLD.Modified_TS,
        CURRENT_TIMESTAMP
    );
END;";
    }
}