using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.Schema
{
    public class TallyHotkeyTableDefinition : ITableDefinition
    {
        public string TableName => "TallyHotkey";

        public string CreateTable =>
@"CREATE TABLE TallyHotKey (
    TallyHotKey_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT COLLATE NOCASE,
    LiveDead TEXT COLLATE NOCASE,
    HotKey TEXT COLLATE NOCASE,

    UNIQUE (StratumCode, HotKey) ON CONFLICT REPLACE,

    CHECK (LiveDead IN ('L', 'D') OR LiveDead IS NULL),
    CHECK ((SpeciesCode IS NOT NULL AND LiveDead IS NOT NULL) OR (SpeciesCode IS NULL AND LiveDead IS NULL)),

    FOREIGN KEY (StratumCode, SampleGroupCode, CruiseID)
        REFERENCES SampleGroup (StratumCode, SampleGroupCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (SpeciesCode, CruiseID)
        REFERENCES Species (SpeciesCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead)
        REFERENCES Subpopulation (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes =>
@"CREATE INDEX NIX_TallyHotKey_SpeciesCode ON 'TallyHotKey'('SpeciesCode', 'CruiseID');

CREATE INDEX NIX_TallyHotKey_StratumCode_SampleGroupCode_SpeciesCode_LiveDead_CruiseID
ON TallyHotKey
(StratumCode, SampleGroupCode, ifnull(SpeciesCode, '') COLLATE NOCASE, ifnull(LiveDead, '') COLLATE NOCASE);";

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();
    }
}