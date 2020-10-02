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
    FOREIGN KEY (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead) REFERENCES Subpopulation (CruiseID, StratumCode, SampleGroupCode, SpeciesCode, LiveDead)
);";

        public string InitializeTable => null;

        public string CreateTombstoneTable => null;

        public string CreateIndexes =>
@"CREATE UNIQUE INDEX TallyDescription_StratumCode_SampleGroupCode_SpeciesCode_LiveDead_CruiseID 
ON TallyDescription 
(CruiseID, StratumCode, SampleGroupCode, ifnull(SpeciesCode, '') COLLATE NOCASE, ifnull(LiveDead, '') COLLATE NOCASE);

CREATE INDEX 'TallyDescription_SpeciesCode_CruiseID' ON 'TallyDescription' ('SpeciesCode', 'CruiseID');";

        public IEnumerable<string> CreateTriggers => Enumerable.Empty<string>();

    }

    public partial class Migrations
    {
        public const string MIGRATE_TALLYDESCRIPTION_FROM_COUNTTREE_FORMAT_STR =
@"WITH ctFlattened AS ( 
SELECT SampleGroup_CN, TreeDefaultValue_CN, max(Tally_CN) AS Tally_CN 
FROM {1}.CountTree 
GROUP BY SampleGroup_CN, ifnull(TreeDefaultValue_CN, '')) 

INSERT OR REPLACE INTO {0}.TallyDescription ( 
    CruiseID, 
    StratumCode, 
    SampleGroupCode, 
    SpeciesCode, 
    LiveDead, 
    Description 
)
SELECT 
    '{3}', 
    st.Code AS StratumCode, 
    sg.Code AS SampleGroupCode,
    tdv.Species,
    tdv.LiveDead,
    t.Description
FROM ctFlattened
JOIN {1}.SampleGroup AS sg USING (SampleGroup_CN)
JOIN {1}.Stratum AS st USING (Stratum_CN)
JOIN {1}.Tally AS t USING (Tally_CN)
LEFT JOIN {1}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN); ";
    }
}