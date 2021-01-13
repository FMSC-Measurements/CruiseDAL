using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.Schema
{
    public class TreeFieldTableDefinition : ITableDefinition
    {
        public string TableName => "TreeField";

        public string CreateTable =>
@"CREATE TABLE TreeField (
    TreeField_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    Field TEXT COLLATE NOCASE,
    DefaultHeading TEXT NOT NULL,
    DbType TEXT NOT NULL COLLATE NOCASE,
    IsTreeMeasurmentField BOOLEAN DEFAULT 0,
    UNIQUE (Field),
    CHECK (DbType IN ('REAL', 'TEXT', 'BOOLEAN', 'INTEGER'))
);";

        public string InitializeTable =>
@"INSERT INTO TreeField ( Field, DefaultHeading, DbType, IsTreeMeasurmentField)
VALUES
    ('SeenDefectPrimary', 'Seen Defect Primary', 'REAL', 1),
    ('SeenDefectSecondary', 'Seen Defect Secondary', 'REAL', 1),
    ('RecoverablePrimary', 'Recoverable Primary', 'REAL', 1),
    ('HiddenPrimary', 'Hidden Primary', 'REAL', 1),
    ('Grade', 'Grade', 'TEXT', 1),
    ('HeightToFirstLiveLimb', 'Height FLL', 'REAL', 1),
    ('PoleLength', 'Pole Length', 'REAL', 1),
    ('ClearFace', 'Clear Face', 'TEXT', 1),
    ('CrownRatio', 'Crown Ratio', 'REAL', 1),
    ('DBH', 'DBH', 'REAL', 1),
    ('DRC', 'DRC', 'REAL', 1),
    ('TotalHeight', 'Total Height', 'REAL', 1),
    ('MerchHeightPrimary', 'Merch Height Primary', 'REAL', 1),
    ('MerchHeightSecondary', 'Merch Height Secondary', 'REAL', 1),
    ('FormClass', 'Form Class', 'REAL', 1),
    ('UpperStemDiameter', 'Upper Stem Diameter', 'REAL', 1),
    ('UpperStemHeight', 'Upper Stem Height', 'REAL', 1),
    ('DBHDoubleBarkThickness', 'DBH Double Bark Thickness', 'REAL', 1),
    ('TopDIBPrimary', 'Top DIB Primary', 'REAL', 1),
    ('TopDIBSecondary', 'Top DIB Secondary', 'REAL', 1),
    ('DefectCode', 'Defect Code', 'TEXT', 1),
    ('DiameterAtDefect', 'Diameter At Defect', 'REAL', 1),
    ('VoidPercent', 'Void Percent', 'REAL', 1),
    ('Slope', 'Slope', 'REAL', 1),
    ('Aspect', 'Aspect', 'REAL', 1),
    ('Remarks', 'Remarks', 'TEXT', 1),
    ('MetaData', 'Meta Data', 'REAL', 1),
    ('IsFallBuckScale', 'Is Fall Buck Scale', 'BOOLEAN', 1),
    ('Initials', 'Initials', 'TEXT', 1)
;";

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();

    }
}