namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TALLYHOTKEY =
@"CREATE TABLE TallyHotKey (
    TallyHotKey_CN INTEGER PRIMARY KEY AUTOINCREMENT,
    StratumCode TEXT NOT NULL COLLATE NOCASE,
    SampleGroupCode TEXT NOT NULL COLLATE NOCASE,
    Species TEXT DEFAULT '' COLLATE NOCASE,
    LiveDead TEXT DEFAULT '' COLLATE NOCASE,
    HotKey TEXT COLLATE NOCASE,

    UNIQUE (StratumCode, HotKey) ON CONFLICT REPLACE,
    --UNIQUE (StratumCode, SampleGroupCode, Species, LiveDead) ON CONFLICT REPLACE,

    FOREIGN KEY (StratumCode, SampleGroupCode) REFERENCES SampleGroup_V3 (StratumCode, SampleGroupCode) ON DELETE CASCADE,
    FOREIGN KEY (Species) REFERENCES Species (Species) ON DELETE CASCADE ON UPDATE CASCADE
);";

        public const string CREATE_INDEX_TallyHotKey_Species =
            @"CREATE INDEX 'TallyHotKey_Species' ON 'TallyHotKey'('Species');";

        public const string CREATE_INDEX_TallyHotKey_StratumCode_SampleGroupCode_Species_LiveDead =
@"CREATE UNIQUE INDEX TallyHotKey_StratumCode_SampleGroupCode_Species_LiveDead
ON TallyHotKey
(StratumCode, SampleGroupCode, ifnull(Species, ''), ifnull(LiveDead, ''));";
    }

    public partial class Migrations
    {
        public const string MIGRATE_TALLYHOTKEY_FROM_COUNTTREE_FORMAT_STR =
            "WITH ctFlattened AS ( " +
            "SELECT SampleGroup_CN, TreeDefaultValue_CN, max(Tally_CN) AS Tally_CN " +
            "FROM {1}.CountTree " +
            "GROUP BY SampleGroup_CN, ifnull(TreeDefaultValue_CN, '')) " +

            "INSERT OR REPLACE INTO {0}.TallyHotKey ( " +
                "StratumCode, " +
                "SampleGroupCode, " +
                "Species, " +
                "LiveDead, " +
                "HotKey " +
            ")" +
            "SELECT " +
                "st.Code AS StratumCode, " +
                "sg.Code AS SampleGroupCode, " +
                "tdv.Species, " +
                "tdv.LiveDead, " +
                "t.HotKey " +
            "FROM ctFlattened " +
            "JOIN {1}.SampleGroup AS sg USING (SampleGroup_CN) " +
            "JOIN {1}.Stratum AS st USING (Stratum_CN) " +
            "JOIN {1}.Tally AS t USING (Tally_CN) " +
            "LEFT JOIN {1}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN)" +
            "; ";
    }
}