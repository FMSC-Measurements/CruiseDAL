namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TALLYHOTKEY =
@"CREATE TABLE TallyHotKey (
    TallyHotKey_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    CruiseID TEXT NOT NULL COLLATE NOCASE,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
    SpeciesCode TEXT COLLATE NOCASE,
    LiveDead TEXT COLLATE NOCASE,
    HotKey TEXT COLLATE NOCASE,

    UNIQUE (StratumCode, HotKey) ON CONFLICT REPLACE,
    --UNIQUE (StratumCode, SampleGroupCode, Species, LiveDead) ON CONFLICT REPLACE,

    CHECK(LiveDead IN ('L', 'D') OR LiveDead IS NULL),

    FOREIGN KEY (StratumCode, SampleGroupCode, CruiseID) REFERENCES SampleGroup (StratumCode, SampleGroupCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (SpeciesCode) REFERENCES Species (SpeciesCode) ON DELETE CASCADE ON UPDATE CASCADE
);";

        public const string CREATE_INDEX_TallyHotKey_SpeciesCode =
            @"CREATE INDEX 'TallyHotKey_SpeciesCode' ON 'TallyHotKey'('SpeciesCode');";

        public const string CREATE_INDEX_TallyHotKey_StratumCode_SampleGroupCode_SpeciesCode_LiveDead_CruiseID =
@"CREATE UNIQUE INDEX TallyHotKey_StratumCode_SampleGroupCode_SpeciesCode_LiveDead_CruiseID
ON TallyHotKey
(CruiseID, StratumCode, SampleGroupCode, ifnull(SpeciesCode, '') COLLATE NOCASE, ifnull(LiveDead, '') COLLATE NOCASE);";
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