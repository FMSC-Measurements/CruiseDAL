using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class SampleGroupDefaultTableDefinition : ITableDefinition
    {
        public string TableName => "SampleGroupDefault";

        public string CreateTable => GetCreateTable(TableName);


        public string GetCreateTable(string tableName)
        {
return $@"CREATE TABLE {tableName} (
    SampleGroupDefault_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    SampleGroupDefaultID TEXT NOT NULL COLLATE NOCASE,
    Region  TEXT COLLATE NOCASE,
    Forest TEXT COLLATE NOCASE,
    District TEXT COLLATE NOCASE,
    SampleGroupCode TEXT COLLATE NOCASE,
    CutLeave TEXT COLLATE NOCASE,
    UOM TEXT COLLATE NOCASE,
    PrimaryProduct TEXT COLLATE NOCASE,
    SecondaryProduct TEXT COLLATE NOCASE,
    BiomassProduct TEXT COLLATE NOCASE,
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

    FOREIGN KEY (Region) REFERENCES LK_Region (Region),
    FOREIGN KEY (Region, Forest) REFERENCES LK_Forest (Region, Forest),
    FOREIGN KEY (Region, Forest, District) REFERENCES LK_District (Region, Forest, District),
    FOREIGN KEY (UOM) REFERENCES LK_UOM (UOM),
    FOREIGN KEY (PrimaryProduct) REFERENCES LK_Product (Product),
    FOREIGN KEY (SecondaryProduct) REFERENCES LK_Product (Product),

    UNIQUE (SampleGroupDefaultID)
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes =>
@"CREATE INDEX UIX_SampleGroupDefault_Region_Forest_District_SampleGroupCode_PrimaryProduct
    ON SampleGroupDefault
    (ifnull(Region, ''), ifnull(Forest, ''), ifnull(District, ''), ifnull(SampleGroupCode, ''), ifnull(PrimaryProduct, ''));";

        public IEnumerable<string> CreateTriggers => null;
    }
}