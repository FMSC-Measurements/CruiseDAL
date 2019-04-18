namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public static readonly string[] TREEFIELD = new string[]
        {
            CREATE_TABLE_TREEFIELD,
            INITIALIZE_TABLE_TREEFIELD,
        };

        public const string CREATE_TABLE_TREEFIELD =
@"CREATE TABLE TreeField (
    TreeField_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    Field TEXT COLLATE NOCASE,
    DbType TEXT NOT NULL COLLATE NOCASE,
    IsTreeMeasurmentField BOOLEAN DEFAULT 0,
    UNIQUE (Field)
);";

        public const string INITIALIZE_TABLE_TREEFIELD =
@"INSERT INTO TreeField ( Field, DbType, IsTreeMeasurmentField)
VALUES
    ('SeenDefectPrimary', 'REAL', 1),
    ('SeenDefectSecondary', 'REAL', 1),
    ('RecoverablePrimary', 'REAL', 1),
    ('HiddenPrimary', 'REAL', 1),
    ('Grade', 'TEXT', 1),
    ('HeightToFirstLiveLimb', 'REAL', 1),
    ('PoleLength', 'REAL', 1),
    ('ClearFace', 'TEXT', 1),
    ('CrownRatio', 'REAL', 1),
    ('DBH', 'REAL', 1),
    ('DRC', 'REAL', 1),
    ('TotalHeight', 'REAL', 1),
    ('MerchHeightPrimary', 'REAL', 1),
    ('MerchHeightSecondary', 'REAL', 1),
    ('FormClass', 'REAL', 1),
    ('UpperStemDiameter', 'REAL', 1),
    ('UpperStemHeight', 'REAL', 1),
    ('DBHDoubleBarkThickness', 'REAL', 1),
    ('TopDIBPrimary', 'REAL', 1),
    ('TopDIBSecondary', 'REAL', 1),
    ('DefectCode', 'TEXT', 1),
    ('DiameterAtDefect', 'REAL', 1),
    ('VoidPercent', 'REAL', 1),
    ('Slope', 'REAL', 1),
    ('Aspect', 'REAL', 1),
    ('Remarks', 'TEXT', 1),
    ('MetaData', 'REAL', 1),
    ('IsFallBuckScale', 'BOOLEAN', 1),
    ('Initials', 'TEXT', 1)
;";
    }
}