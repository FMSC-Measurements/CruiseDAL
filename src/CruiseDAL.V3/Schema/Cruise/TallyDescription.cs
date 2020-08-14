namespace CruiseDAL.Schema
{
    public partial class DDL
    {
        public const string CREATE_TABLE_TALLYDESCRIPTION =
            "CREATE TABLE TallyDescription ( " +
                "TallyDescription_CN INTEGER PRIMARY KEY AUTOINCREMENT" +
                "CruiseID TEXT NOT NULL COLLATE NOCASE, " +
                "StratumCode TEXT NOT NULL COLLATE NOCASE, " +
                "SampleGroupCode TEXT NOT NULL COLLATE NOCASE, " +
                "Species TEXT COLLATE NOCASE, " +
                "LiveDead TEXT COLLATE NOCASE, " +
                "Description TEXT, " +

                "CHECK (LiveDead IN ('L', 'D') OR LiveDead IS NULL)," +

                //"UNIQUE (StratumCode, SampleGroupCode, Species, LiveDead)  ON CONFLICT REPLACE, " +
                //"UNIQUE (StratumCode, Description), " +
                "FOREIGN KEY (StratumCode, SampleGroupCode, CruiseID) REFERENCES SampleGroup (StratumCode, SampleGroupCode, CruiseID) ON DELETE CASCADE ON UPDATE CASCADE, " +
                "FOREIGN KEY (Species) REFERENCES SpeciesCode (Species) ON DELETE CASCADE ON UPDATE CASCADE " +
            ");";

        public const string CREATE_INDEX_TallyDescription_StratumCode_SampleGroupCode_Species_LiveDead_CruiseID =
            "CREATE UNIQUE INDEX TallyDescription_StratumCode_SampleGroupCode_Species_LiveDead_CruiseID " +
            "ON TallyDescription " +
            "(CruiseID, StratumCode, SampleGroupCode, ifnull(Species, '') COLLATE NOCASE, ifnull(LiveDead, '') COLLATE NOCASE);";

        public const string CREATE_INDEX_TallyDescription_Species =
            @"CREATE INDEX 'TallyDescription_Species' ON 'TallyDescription'('Species');";
    }

    public partial class Migrations
    {
        public const string MIGRATE_TALLYDESCRIPTION_FROM_COUNTTREE_FORMAT_STR =
            "WITH ctFlattened AS ( " +
            "SELECT SampleGroup_CN, TreeDefaultValue_CN, max(Tally_CN) AS Tally_CN " +
            "FROM {1}.CountTree " +
            "GROUP BY SampleGroup_CN, ifnull(TreeDefaultValue_CN, '')) " +

            "INSERT OR REPLACE INTO {0}.TallyDescription ( " +
                "CruiseID, " +
                "StratumCode, " +
                "SampleGroupCode, " +
                "Species, " +
                "LiveDead, " +
                "Description " +
            ")" +
            "SELECT " +
                "'{3}', " +
                "st.Code AS StratumCode, " +
                "sg.Code AS SampleGroupCode, " +
                "tdv.Species, " +
                "tdv.LiveDead, " +
                "t.Description " +
            "FROM ctFlattened " +
            "JOIN {1}.SampleGroup AS sg USING (SampleGroup_CN) " +
            "JOIN {1}.Stratum AS st USING (Stratum_CN) " +
            "JOIN {1}.Tally AS t USING (Tally_CN) " +
            "LEFT JOIN {1}.TreeDefaultValue AS tdv USING (TreeDefaultValue_CN)" +
            "; ";
    }
}