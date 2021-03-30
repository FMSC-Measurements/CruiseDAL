using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CruiseDAL.Schema
{
    public class StratumDefaultTableDefinition : ITableDefinition
    {
        public string TableName => "StratumDefault";

        public string CreateTable =>
@"CREATE TABLE StratumDefault (
    StratumDefault_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    StratumDefaultID TEXT NOT NULL COLLATE NOCASE,
    Region  TEXT COLLATE NOCASE,
    Forest TEXT COLLATE NOCASE,
    District TEXT COLLATE NOCASE,
    StratumCode TEXT COLLATE NOCASE,
    Description TEXT COLLATE NOCASE,
    Method TEXT COLLATE NOCASE,
    BasalAreaFactor REAL,
    FixedPlotSize REAL,
    KZ3PPNT INTEGER,
    SamplingFrequency INTEGER,
    Hotkey TEXT,
    FBSCode TEXT,
    YieldComponent TEXT,
    FixCNTField TEXT COLLATE NOCASE,

    FOREIGN KEY (FixCNTField) REFERENCES TreeField (Field), 
    FOREIGN KEY (Method) REFERENCES LK_CruiseMethod (Method), 
    FOREIGN KEY (Region) REFERENCES LK_Region (Region),
    FOREIGN KEY (Region, Forest) REFERENCES LK_Forest (Region, Forest),
    FOREIGN KEY (Region, Forest, District) REFERENCES LK_District (Region, Forest, District),

    UNIQUE (StratumdefaultID)
);
    ";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => 
@"CREATE INDEX UIX_StratumDefault_Region_Forest_District_StratumCode_Discription_Method 
    ON StratumDefault
    (ifnull(Region, ''), ifnull(Forest, ''), ifnull(District, ''), ifnull(StratumCode, ''), ifnull(Description, ''), ifnull(Method, ''));";

        public IEnumerable<string> CreateTriggers => null;
    }
}
