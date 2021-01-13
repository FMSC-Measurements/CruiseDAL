using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class TreeMeasurmentsTableDefinition : ITableDefinition
    {
        //        public const string CREATE_VIEW_TREEMEASURMENT =
        //@"CREATE VIEW TreeMeasurment AS
        //SELECT
        //    t.TreeID,
        //    ifnull( MAX( CASE WHEN Field = 'SeenDefectPrimary' THEN ValueReal END), 0.0) AS SeenDefectPrimary,
        //    ifnull( MAX( CASE WHEN Field = 'SeenDefectSecondary' THEN ValueReal END), 0.0) AS SeenDefectSecondary,
        //    ifnull( MAX( CASE WHEN Field = 'RecoverablePrimary' THEN ValueReal END), 0.0) AS RecoverablePrimary,
        //    ifnull( MAX( CASE WHEN Field = 'HiddenPrimary' THEN ValueReal END), 0.0) AS HiddenPrimary,
        //    ifnull( MAX( CASE WHEN Field = 'HeightToFirstLiveLimb' THEN ValueReal END), 0.0) AS HeightToFirstLiveLimb,
        //    ifnull( MAX( CASE WHEN Field = 'PoleLength' THEN ValueReal END), 0.0) AS PoleLength,
        //    ifnull( MAX( CASE WHEN Field = 'CrownRatio' THEN ValueReal END), 0.0) AS CrownRatio,
        //    ifnull( MAX( CASE WHEN Field = 'DBH' THEN ValueReal END), 0.0) AS DBH,
        //    ifnull( MAX( CASE WHEN Field = 'DRC' THEN ValueReal END), 0.0) AS DRC,
        //    ifnull( MAX( CASE WHEN Field = 'TotalHeight' THEN ValueReal END), 0.0) AS TotalHeight,
        //    ifnull( MAX( CASE WHEN Field = 'MerchHeightPrimary' THEN ValueReal END), 0.0) AS MerchHeightPrimary,
        //    ifnull( MAX( CASE WHEN Field = 'MerchHeightSecondary' THEN ValueReal END), 0.0) AS MerchHeightSecondary,
        //    ifnull( MAX( CASE WHEN Field = 'FormClass' THEN ValueReal END), 0.0) AS FormClass,
        //    ifnull( MAX( CASE WHEN Field = 'UpperStemDiameter' THEN ValueReal END), 0.0) AS UpperStemDiameter,
        //    ifnull( MAX( CASE WHEN Field = 'UpperStemHeight' THEN ValueReal END), 0.0) AS UpperStemHeight,
        //    ifnull( MAX( CASE WHEN Field = 'DBHDoubleBarkThickness' THEN ValueReal END), 0.0) AS DBHDoubleBarkThickness,
        //    ifnull( MAX( CASE WHEN Field = 'TopDIBPrimary' THEN ValueReal END), 0.0) AS TopDIBPrimary,
        //    ifnull( MAX( CASE WHEN Field = 'TopDIBSecondary' THEN ValueReal END), 0.0) AS TopDIBSecondary,
        //    ifnull( MAX( CASE WHEN Field = 'DiameterAtDefect' THEN ValueReal END), 0.0) AS DiameterAtDefect,
        //    ifnull( MAX( CASE WHEN Field = 'VoidPercent' THEN ValueReal END), 0.0) AS VoidPercent,
        //    ifnull( MAX( CASE WHEN Field = 'Slope' THEN ValueReal END), 0.0) AS Slope,
        //    ifnull( MAX( CASE WHEN Field = 'Aspect' THEN ValueReal END), 0.0) AS Aspect,
        //    ifnull( MAX( CASE WHEN Field = 'XCoordinate' THEN ValueReal END), 0.0) AS XCoordinate,
        //    ifnull( MAX( CASE WHEN Field = 'YCoordinate' THEN ValueReal END), 0.0) AS YCoordinate,
        //    ifnull( MAX( CASE WHEN Field = 'ZCoordinate' THEN ValueReal END), 0.0) AS ZCoordinate,

        //    ifnull( MAX( CASE WHEN Field = 'IsFallBuckScale' THEN ValueBool END), 0) AS IsFallBuckScale,

        //    MAX( CASE WHEN Field = 'Grade' THEN ValueText END) AS Grade,
        //    MAX( CASE WHEN Field = 'ClearFace' THEN ValueText END) AS ClearFace,
        //    MAX( CASE WHEN Field = 'DefectCode' THEN ValueText END) AS DefectCode,
        //    MAX( CASE WHEN Field = 'Remarks' THEN ValueText END) AS Remarks,
        //    MAX( CASE WHEN Field = 'MetaData' THEN ValueText END) AS MetaData
        //FROM TreeFieldValue AS tfv
        //GROUP BY TreeID;";

        public string TableName => "TreeMeasurment";

        public string CreateTable =>
@"CREATE TABLE TreeMeasurment (
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

    UNIQUE (TreeID),

    FOREIGN KEY (TreeID) REFERENCES Tree (TreeID) ON DELETE CASCADE ON UPDATE CASCADE
)";

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

CREATE INDEX TreeMeasurment_Tombstone_TreeID ON TreeMeasurment_Tombstone 
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