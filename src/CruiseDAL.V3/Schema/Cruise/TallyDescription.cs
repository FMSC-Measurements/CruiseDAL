using System.Collections.Generic;
using System.Linq;

namespace CruiseDAL.Schema
{
    public class TallyDescriptionTableDefinition : ITableDefinition
    {
        public string TableName => "TallyDescription";

        public string CreateTable =>
@"CREATE TABLE TallyDescription (
    TallyDescription_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT COLLATE NOCASE,
    LiveDead TEXT COLLATE NOCASE,
    Description TEXT NOT NULL COLLATE NOCASE,

    CHECK (LiveDead IN ('L', 'D') OR LiveDead IS NULL),
    CHECK ((SpeciesCode IS NOT NULL AND LiveDead IS NOT NULL) OR (SpeciesCode IS NULL AND LiveDead IS NULL)),

    FOREIGN KEY (StratumCode, SampleGroupCode, CruiseID) REFERENCES SampleGroup (StratumCode, SampleGroupCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (SpeciesCode, CruiseID) REFERENCES Species (SpeciesCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE ,
    FOREIGN KEY (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead) REFERENCES Subpopulation (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead) ON DELETE CASCADE
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes =>
@"CREATE UNIQUE INDEX UIX_TallyDescription_StratumCode_SampleGroupCode_SpeciesCode_LiveDead_CruiseID
ON TallyDescription
(CruiseID, StratumCode, SampleGroupCode, ifnull(SpeciesCode, '') COLLATE NOCASE, ifnull(LiveDead, '') COLLATE NOCASE);

CREATE INDEX NIX_TallyDescription_SpeciesCode_CruiseID ON TallyDescription ('SpeciesCode', 'CruiseID');";

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();
    }
}