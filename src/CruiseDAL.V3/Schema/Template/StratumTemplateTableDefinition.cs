using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class StratumTemplateTableDefinition : ITableDefinition
    {
        public string TableName => "StratumTemplate";

        public string CreateTable =>
@"CREATE TABLE StratumTemplate (
    StratumTemplate_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    StratumTemplateName TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT COLLATE NOCASE,
    Method TEXT COLLATE NOCASE,
    BasalAreaFactor REAL,
    FixedPlotSize REAL,
    KZ3PPNT INTEGER,
    SamplingFrequency INTEGER,
    Hotkey TEXT,
    FBSCode TEXT,
    YieldComponent TEXT,
    FixCNTField TEXT COLLATE NOCASE,

    FOREIGN KEY (CruiseID) REFERENCES Cruise (CruiseID) ON DELETE CASCADE,
    FOREIGN KEY (FixCNTField) REFERENCES TreeField (Field),
    FOREIGN KEY (Method) REFERENCES LK_CruiseMethod (Method),

    UNIQUE (StratumTemplateName, CruiseID)
);
    ";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes => null;

        public IEnumerable<string> CreateTriggers => null;
    }
}