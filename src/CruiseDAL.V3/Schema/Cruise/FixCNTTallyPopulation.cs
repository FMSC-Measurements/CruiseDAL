using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.Schema
{
    public class FixCNTTallyPopulationTableDefinition : ITableDefinition
    {
        public string TableName => "FixCNTTallyPopulation";

        public string CreateTable =>
@"CREATE TABLE FixCNTTallyPopulation (
    FixCNTTallyPopulation_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT NOT NULL COLLATE NOCASE,
    LiveDead TEXT NOT NULL COLLATE NOCASE,
    IntervalSize INTEGER Default 0,
    Min INTEGER Default 0,
    Max INTEGER Default 0,

    UNIQUE (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead),

    FOREIGN KEY (StratumCode, SampleGroupCode, SpeciesCode, LiveDead, CruiseID) REFERENCES SubPopulation (StratumCode, SampleGroupCode, SpeciesCode, LiveDead, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes =>
@"CREATE INDEX FixCNTTallyPopulation_StratumCode_CruiseID ON FixCNTTallyPopulation (StratumCode, CruiseID);

CREATE INDEX 'FixCNTTallyPopulation_StratumCode_SampleGroupCode_SpeciesCode_LiveDead_CruiseID' ON FixCNTTallyPopulation (StratumCode, SampleGroupCode, SpeciesCode, LiveDead, CruiseID);";

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();
    }
}