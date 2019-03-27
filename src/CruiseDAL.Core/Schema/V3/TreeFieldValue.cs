namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public static readonly string[] TREEFIELDVALUE = new string[]
        {
            CREATE_TABLE_TREEFIELDVALUE,
            CREATE_VIEW_TreeFieldValue_TreeMeasurment,
            CREATE_VIEW_TREEFIELDVALUE_ALL,
        };

        public const string CREATE_TABLE_TREEFIELDVALUE =
    @"CREATE TABLE TreeFieldValue (
    TreeID TEXT NOT NULL,
    Field TEXT NOT NULL COLLATE NOCASE,
    ValueInt INTEGER,
    ValueReal REAL,
    ValueBool BOOLEAN,
    ValueText TEXT,
    CreatedDate DateTime DEFAULT(datetime('now', 'localtime')) ,
    FOREIGN KEY (Field) REFERENCES TreeField (Field),
    FOREIGN KEY (TreeID) REFERENCES Tree_V3 (TreeID) ON DELETE CASCADE
);";

        public const string CREATE_VIEW_TreeFieldValue_TreeMeasurment =
@"CREATE VIEW TreeFieldValue_TreeMeasurment AS
SELECT
    tm.TreeID,
    tf.Field,
    tf.DbType,
    (CASE tf.Field
        WHEN 'SeenDefectPrimary' THEN SeenDefectPrimary
        WHEN 'SeenDefectSecondary' THEN SeenDefectSecondary
        WHEN 'RecoverablePrimary' THEN RecoverablePrimary
        WHEN 'HiddenPrimary' THEN HiddenPrimary
        WHEN 'HeightToFirstLiveLimb' THEN HeightToFirstLiveLimb
        WHEN 'PoleLength' THEN PoleLength
        WHEN 'CrownRatio' THEN CrownRatio
        WHEN 'DBH' THEN DBH
        WHEN 'DRC' THEN DRC
        WHEN 'TotalHeight' THEN TotalHeight
        WHEN 'MerchHeightPrimary' THEN MerchHeightPrimary
        WHEN 'MerchHeightSecondary' THEN MerchHeightSecondary
        WHEN 'FormClass' THEN FormClass
        WHEN 'UpperStemDiameter' THEN UpperStemDiameter
        WHEN 'UpperStemHeight' THEN UpperStemHeight
        WHEN 'DBHDoubleBarkThickness' THEN DBHDoubleBarkThickness
        WHEN 'TopDIBPrimary' THEN TopDIBPrimary
        WHEN 'TopDIBSecondary' THEN TopDIBSecondary
        WHEN 'DiameterAtDefect' THEN DiameterAtDefect
        WHEN 'VoidPercent' THEN VoidPercent
        WHEN 'Slope' THEN Slope
        WHEN 'Aspect' THEN Aspect
        --WHEN 'XCoordinate' THEN XCoordinate
        --WHEN 'YCoordinate' THEN YCoordinate
        --WHEN 'ZCoordinate' THEN ZCoordinate
        ELSE NULL END) AS ValueReal,

    (CASE tf.Field
        WHEN 'IsFallBuckScale' THEN IsFallBuckScale
        ELSE NULL END) AS ValueBool,

    (CASE tf.Field
        WHEN 'Grade' THEN Grade
        WHEN 'ClearFace' THEN ClearFace
        WHEN 'DefectCode' THEN DefectCode
        WHEN 'Remarks' THEN Remarks
        WHEN 'MetaData' THEN MetaData
        WHEN 'Initials' THEN Initials
        ELSE NULL END) AS ValueText,

    NULL AS ValueInt,
    NULL AS CreatedDate

FROM TreeMeasurment AS tm
CROSS JOIN TreeField AS tf
WHERE tf.IsTreeMeasurmentField = 1;";

        public const string CREATE_VIEW_TreeFieldValue_TreeMeasurment_Filtered =
@"CREATE VIEW TreeFieldValue_TreeMeasurment_Filtered AS
SELECT
    tm.TreeID,
    tf.Field,
    tf.DbType,
    (CASE tf.Field
        WHEN 'SeenDefectPrimary' THEN SeenDefectPrimary
        WHEN 'SeenDefectSecondary' THEN SeenDefectSecondary
        WHEN 'RecoverablePrimary' THEN RecoverablePrimary
        WHEN 'HiddenPrimary' THEN HiddenPrimary
        WHEN 'HeightToFirstLiveLimb' THEN HeightToFirstLiveLimb
        WHEN 'PoleLength' THEN PoleLength
        WHEN 'CrownRatio' THEN CrownRatio
        WHEN 'DBH' THEN DBH
        WHEN 'DRC' THEN DRC
        WHEN 'TotalHeight' THEN TotalHeight
        WHEN 'MerchHeightPrimary' THEN MerchHeightPrimary
        WHEN 'MerchHeightSecondary' THEN MerchHeightSecondary
        WHEN 'FormClass' THEN FormClass
        WHEN 'UpperStemDiameter' THEN UpperStemDiameter
        WHEN 'UpperStemHeight' THEN UpperStemHeight
        WHEN 'DBHDoubleBarkThickness' THEN DBHDoubleBarkThickness
        WHEN 'TopDIBPrimary' THEN TopDIBPrimary
        WHEN 'TopDIBSecondary' THEN TopDIBSecondary
        WHEN 'DiameterAtDefect' THEN DiameterAtDefect
        WHEN 'VoidPercent' THEN VoidPercent
        WHEN 'Slope' THEN Slope
        WHEN 'Aspect' THEN Aspect
        --WHEN 'XCoordinate' THEN XCoordinate
        --WHEN 'YCoordinate' THEN YCoordinate
        --WHEN 'ZCoordinate' THEN ZCoordinate
        ELSE NULL END) AS ValueReal,

    (CASE tf.Field
        WHEN 'IsFallBuckScale' THEN IsFallBuckScale
        ELSE NULL END) AS ValueBool,

    (CASE tf.Field
        WHEN 'Grade' THEN Grade
        WHEN 'ClearFace' THEN ClearFace
        WHEN 'DefectCode' THEN DefectCode
        WHEN 'Remarks' THEN Remarks
        WHEN 'MetaData' THEN MetaData
        WHEN 'Initials' THEN Initials
        ELSE NULL END) AS ValueText,

    NULL AS ValueInt,
    NULL AS CreatedDate

FROM TreeMeasurment AS tm
JOIN Tree_V3 AS t USING (TreeID)
CROSS JOIN TreeFieldSetup_V3 AS tfs
JOIN TreeField AS tf USING (Field)
WHERE tf.IsTreeMeasurmentField = 1;";

        public const string CREATE_VIEW_TREEFIELDVALUE_ALL =
@"CREATE VIEW TreeFieldValue_All AS
SELECT
    TreeID,
    Field,
    ValueInt,
    ValueReal,
    ValueBool,
    ValueText,
    CreatedDate
FROM TreeFieldValue_TreeMeasurment
UNION ALL
SELECT
    TreeID,
    Field,
    ValueInt,
    ValueReal,
    ValueBool,
    ValueText,
    CreatedDate
FROM TreeFieldValue;";
    }
}