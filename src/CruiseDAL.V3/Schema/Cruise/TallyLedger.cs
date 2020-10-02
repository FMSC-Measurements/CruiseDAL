using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.Schema
{
    public class TallyLedgerTableDefinition : ITableDefinition
    {
        public string TableName => "TallyLedger";

        public string CreateTable =>
@"CREATE TABLE TallyLedger (
    TallyLedger_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    TallyLedgerID TEXT NOT NULL,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    TreeID TEXT,
    CuttingUnitCode TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
    PlotNumber INTEGER,
    SpeciesCode TEXT COLLATE NOCASE,
    LiveDead TEXT COLLATE NOCASE,
    TreeCount INTEGER NOT NULL,
    KPI INTEGER Default 0,
    STM BOOLEAN DEFAULT 0,
    ThreePRandomValue INTEGER Default 0,
    Signature TEXT COLLATE NOCASE,
    Reason TEXT,
    Remarks TEXT,
    EntryType TEXT COLLATE NOCASE,

    CreatedBy TEXT DEFAULT 'none',
    CreatedDate DATETIME DEFAULT (datetime('now', 'localtime')),
    IsDeleted BOOLEAN DEFAULT 0,

    CHECK (LiveDead IN ('L', 'D') OR LiveDead IS NULL),

    UNIQUE (TallyLedgerID),

    FOREIGN KEY (CuttingUnitCode, CruiseID) REFERENCES CuttingUnit (Code, CruiseID)
    FOREIGN KEY (SampleGroupCode, StratumCode, CruiseID) REFERENCES SampleGroup (SampleGroupCode, StratumCode, CruiseID),
    FOREIGN KEY (SpeciesCode,CruiseID) REFERENCES Species (SpeciesCode, CruiseID)
    FOREIGN KEY (PlotNumber, CuttingUnitCode, CruiseID) REFERENCES Plot (PlotNumber, CuttingUnitCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,

    -- everything below are tree fKey references. there are a few, but because some tree values can be null we need to have them as seperate references

    FOREIGN KEY (TreeID) REFERENCES Tree (TreeID) ON DELETE CASCADE,
    FOREIGN KEY (TreeID, CuttingUnitCode, StratumCode, SampleGroupCode) REFERENCES Tree (TreeID, CuttingUnitCode, StratumCode, SampleGroupCode) ON UPDATE CASCADE,
    FOREIGN KEY (TreeID, SpeciesCode) REFERENCES Tree (TreeID, SpeciesCode) ON UPDATE CASCADE,
    FOREIGN KEY (TreeID, LiveDead) REFERENCES Tree (TreeID, LiveDead) ON UPDATE CASCADE,
    FOREIGN KEY (TreeID, PlotNumber) REFERENCES Tree (TreeID, PlotNumber) ON UPDATE CASCADE
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null; // TODO

        public string CreateIndexes =>
@"CREATE INDEX 'TallyLedger_TreeID' ON 'TallyLedger'('TreeID');

CREATE INDEX 'TallyLedger_SampleGroupCode_StratumCode_CruiseID' ON 'TallyLedger'('SampleGroupCode', 'StratumCode', 'CruiseID');

CREATE INDEX 'TallyLedger_StratumCode_CruiseID' ON 'TallyLedger'('StratumCode', 'CruiseID');

CREATE INDEX 'TallyLedger_CuttingUnitCode_CruiseID' ON 'TallyLedger'('CuttingUnitCode', 'CruiseID');";

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();
    }
}