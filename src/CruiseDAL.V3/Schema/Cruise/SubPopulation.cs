using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class SubPopulationTableDefinition : ITableDefinition
    {
        public string TableName => "SubPopulation";

        public string CreateTable => GetCreateTable(TableName);

        public string GetCreateTable(string tableName)
        {
            return $@"CREATE TABLE {tableName} (
    Subpopulation_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    SubPopulationID TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT NOT NULL COLLATE NOCASE,
    LiveDead TEXT NOT NULL COLLATE NOCASE,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    UNIQUE (SubPopulationID),
    UNIQUE (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead),

    CHECK (SubPopulationID LIKE '________-____-____-____-____________'),
    CHECK (LiveDead IN ('L', 'D')),

    FOREIGN KEY (StratumCode, SampleGroupCode, CruiseID) REFERENCES SampleGroup (StratumCode, SampleGroupCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (SpeciesCode, CruiseID) REFERENCES Species (SpeciesCode, CruiseID) ON UPDATE CASCADE
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable =>
@"CREATE TABLE SubPopulation_Tombstone (
    SubPopulationID TEXT NOT NULL COLLATE NOCASE,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT NOT NULL COLLATE NOCASE,
    LiveDead TEXT NOT NULL COLLATE NOCASE,
    CreatedBy TEXT,
    Created_TS DATETIME,
    ModifiedBy TEXT,
    Modified_TS DATETIME,
    Deleted_TS DATETIME,

    UNIQUE(SubPopulationID)
);

CREATE INDEX NIX_SubPopulation_Tombstone_CruiseID_StratumCode_SampleGroupCode_SpeciesCode_LiveDead ON SubPopulation_Tombstone
(CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead);";

        public string CreateIndexes =>
@"CREATE INDEX NIX_Subpopulation_SpeciesCode_CruiseID ON Subpopulation (SpeciesCode, CruiseID);

CREATE INDEX NIX_Subpopulation_StratumCode_SampleGroupCode_CruiseID ON Subpopulation (StratumCode, SampleGroupCode,  CruiseID);";

        public IEnumerable<string> CreateTriggers => new[] { CREATE_TRIGGER_SubPopulation_OnUpdate, CREATE_TRIGGER_SubPopulation_OnDelete };

        public const string CREATE_TRIGGER_SubPopulation_OnUpdate =
@"CREATE TRIGGER SubPopulation_OnUpdate
AFTER UPDATE OF
    LiveDead
ON SubPopulation
FOR EACH ROW
BEGIN
    UPDATE SubPopulation SET Modified_TS = CURRENT_TIMESTAMP WHERE Subpopulation_CN = old.Subpopulation_CN;
END;";

        public const string CREATE_TRIGGER_SubPopulation_OnDelete =
@"CREATE TRIGGER SubPopulation_OnDelete
BEFORE DELETE ON SubPopulation
FOR EACH ROW
BEGIN
    INSERT OR REPLACE INTO SubPopulation_Tombstone (
        SubPopulationID,
        CruiseID,
        StratumCode,
        SampleGroupCode,
        SpeciesCode,
        LiveDead,
        CreatedBy,
        Created_TS,
        ModifiedBy,
        Modified_TS,
        Deleted_TS
    ) VALUES (
        OLD.SubPopulationID,
        OLD.CruiseID,
        OLD.StratumCode,
        OLD.SampleGroupCode,
        OLD.SpeciesCode,
        OLD.LiveDead,
        OLD.CreatedBy,
        OLD.Created_TS,
        OLD.ModifiedBy,
        OLD.Modified_TS,
        CURRENT_TIMESTAMP
    );
END;";
    }
}