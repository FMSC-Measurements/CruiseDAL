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
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),

    CHECK (LiveDead IN ('L', 'D') OR LiveDead IS NULL),

    UNIQUE (TallyLedgerID),

    FOREIGN KEY (CuttingUnitCode, CruiseID) REFERENCES CuttingUnit (CuttingUnitCode, CruiseID)
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

        public string CreateTombstoneTable =>
@"CREATE TABLE TallyLedger_Tombstone (
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
    KPI INTEGER,
    STM BOOLEAN,
    ThreePRandomValue INTEGER,
    Signature TEXT COLLATE NOCASE,
    Reason TEXT,
    Remarks TEXT,
    EntryType TEXT COLLATE NOCASE,

    CreatedBy TEXT,
    Created_TS DATETIME,
    Deleted_TS DATETIME
);

CREATE INDEX TallyLedger_Tombstone_TallyLedgerID ON TallyLedger_Tombstone
(TallyLedgerID);";

        public string CreateIndexes =>
@"CREATE INDEX 'TallyLedger_TreeID' ON 'TallyLedger'('TreeID');

CREATE INDEX 'TallyLedger_SampleGroupCode_StratumCode_CruiseID' ON 'TallyLedger'('SampleGroupCode', 'StratumCode', 'CruiseID');

CREATE INDEX 'TallyLedger_StratumCode_CruiseID' ON 'TallyLedger'('StratumCode', 'CruiseID');

CREATE INDEX 'TallyLedger_CuttingUnitCode_CruiseID' ON 'TallyLedger'('CuttingUnitCode', 'CruiseID');

CREATE INDEX TallyLedger_Created_TS ON TallyLedger (Created_TS);";

        public IEnumerable<string> CreateTriggers => new[]
        {
            CREATE_TRIGGER_TallyLedger_OnDelete,
        };

        public const string CREATE_TRIGGER_TallyLedger_OnDelete =
@"CREATE TRIGGER TallyLedger_OnDelete 
BEFORE DELETE ON TallyLedger 
FOR EACH ROW 
BEGIN
    INSERT OR REPLACE INTO TallyLedger_Tombstone (
        TallyLedgerID,
        CruiseID,
        TreeID,
        CuttingUnitCode,
        StratumCode,
        SampleGroupCode,
        PlotNumber,
        SpeciesCode,
        LiveDead,
        TreeCount,
        KPI,
        STM,
        ThreePRandomValue,
        Signature,
        Reason,
        Remarks,
        EntryType,

        CreatedBy,
        Created_TS,
        Deleted_TS
    ) VALUES (
        OLD.TallyLedgerID,
        OLD.CruiseID,
        OLD.TreeID,
        OLD.CuttingUnitCode,
        OLD.StratumCode,
        OLD.SampleGroupCode,
        OLD.PlotNumber,
        OLD.SpeciesCode,
        OLD.LiveDead,
        OLD.TreeCount,
        OLD.KPI,
        OLD.STM,
        OLD.ThreePRandomValue,
        OLD.Signature,
        OLD.Reason,
        OLD.Remarks,
        OLD.EntryType,

        OLD.CreatedBy,
        OLD.Created_TS,
        CURRENT_TIMESTAMP
    );
END;";
    }
}