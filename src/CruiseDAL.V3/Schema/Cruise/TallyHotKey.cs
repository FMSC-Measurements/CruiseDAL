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

    CHECK(LiveDead IN ('L', 'D') OR LiveDead IS NULL),
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
@"CREATE INDEX 'TallyHotKey_SpeciesCode' ON 'TallyHotKey'('SpeciesCode', 'CruiseID');

CREATE INDEX TallyHotKey_StratumCode_SampleGroupCode_SpeciesCode_LiveDead_CruiseID
ON TallyHotKey
(StratumCode, SampleGroupCode, ifnull(SpeciesCode, '') COLLATE NOCASE, ifnull(LiveDead, '') COLLATE NOCASE);";

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();

    }

    public partial class Migrations
    {
        public const string MIGRATE_TALLYHOTKEY_FROM_COUNTTREE_FORMAT_STR =
@"WITH ctFlattened AS ( 
SELECT SampleGroup_CN, TreeDefaultValue_CN, max(Tally_CN) AS Tally_CN 
FROM {1}.CountTree 
GROUP BY SampleGroup_CN, ifnull(TreeDefaultValue_CN, '')) 

INSERT OR REPLACE INTO {0}.TallyHotKey ( 
    CruiseID,
    StratumCode, 
    SampleGroupCode, 
    SpeciesCode, 
    LiveDead, 
    HotKey 
)
SELECT 
    '{3}',
    st.Code AS StratumCode, 
    sg.Code AS SampleGroupCode, 
    tdv.Species, 
    tdv.LiveDead, 
    t.HotKey 
FROM ctFlattened 
JOIN {1}.SampleGroup AS sg USING (SampleGroup_CN) 
JOIN {1}.Stratum AS st USING (Stratum_CN) 
JOIN {1}.Tally AS t USING (Tally_CN) 
LEFT JOIN {1}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN)
; ";
    }
}