using System.Collections.Generic;

namespace CruiseDAL.Schema
{
    public class FixCNTTallyPopulationTableDefinition : ITableDefinition
    {
        public string TableName => "FixCNTTallyPopulation";

        public string CreateTable => GetCreateTable(TableName);

        public string GetCreateTable(string tableName)
        {
            return $@"CREATE TABLE {tableName} (
    FixCNTTallyPopulation_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT NOT NULL COLLATE NOCASE,
    LiveDead TEXT NOT NULL COLLATE NOCASE,
    IntervalSize INTEGER Default 0,
    Min INTEGER Default 0,
    Max INTEGER Default 0,
    CreatedBy TEXT DEFAULT 'none',
    Created_TS DATETIME DEFAULT (CURRENT_TIMESTAMP),
    ModifiedBy TEXT,
    Modified_TS DATETIME,

    UNIQUE (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead),

    FOREIGN KEY (StratumCode, SampleGroupCode, SpeciesCode, LiveDead, CruiseID) REFERENCES SubPopulation (StratumCode, SampleGroupCode, SpeciesCode, LiveDead, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE
);";
        }

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes =>
@"CREATE INDEX NIX_FixCNTTallyPopulation_StratumCode_CruiseID ON FixCNTTallyPopulation (StratumCode, CruiseID);

CREATE INDEX NIX_FixCNTTallyPopulation_StratumCode_SampleGroupCode_SpeciesCode_LiveDead_CruiseID ON FixCNTTallyPopulation (StratumCode, SampleGroupCode, SpeciesCode, LiveDead, CruiseID);";

        public IEnumerable<string> CreateTriggers => new[]
        {
            CREATE_TRIGGER_FixCNTTallyPopulation_OnUpdate,
        };

        public const string CREATE_TRIGGER_FixCNTTallyPopulation_OnUpdate =
@"CREATE TRIGGER FixCNTTallyPopulation_OnUpdate
AFTER UPDATE OF
    IntervalSize,
    Min,
    Max
ON FixCNTTallyPopulation
FOR EACH ROW
BEGIN
    UPDATE FixCNTTallyPopulation SET Modified_TS = CURRENT_TIMESTAMP WHERE FixCNTTallyPopulation_CN = old.FixCNTTallyPopulation_CN;
END;";
    }
}